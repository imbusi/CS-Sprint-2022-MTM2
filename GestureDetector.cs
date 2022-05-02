//------------------------------------------------------------------------------
// GestureDetector Class
//  Based on the example GestureDetector from 'DiscreteGestureBasics-WPF'.
//
//  Implementation modified to support multiple database files and updated 
//  GestureResultView class interface.
//------------------------------------------------------------------------------

namespace CSSpring2022MTM2
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;

    // GestureDetector listens to the VisualGestureBuilderFrame frame events from the KinectService and
    // updates each gesture in the GestureResultView object.
    public class GestureDetector : IDisposable
    {
        // Array of filenames from which to include gesture data
        private readonly string[] databaseFiles = { @"Database\499Gestures.gbd", @"Database\Seated.gbd", @"Database\499Gestures_2.gbd" };

        // List of discrete gesture names to identify
        private List<string> gestureNames = null;

        // The Gesture frame source which is tied to a body tracking ID
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        // The Gesture frame reader which will be subscribed to for frame updates
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /// <summary>
        /// Class constructor. Subscribes to the kinectSensor's VisualGestureBuilder frame data & loads gestures to the
        /// GestureResultView object.
        /// </summary>
        /// <param name="kinectSensor">The defaulty kinectSensor to connect to.</param>
        /// <param name="gestureResultView">The GestureResultView object used for this body ID.</param>
        /// <exception cref="ArgumentNullException">Exceptions if either argument is null.</exception>
        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }

            if (gestureResultView == null)
            {
                throw new ArgumentNullException("gestureResultView");
            }

            this.GestureResultView = gestureResultView;

            // Create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            // Open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }

            // Load all gestures from /Database/ folder
            this.GestureNames = new List<string>();
            foreach (String dbFile in databaseFiles)
            {
                using (VisualGestureBuilderDatabase db = new VisualGestureBuilderDatabase(dbFile))
                {
                    foreach (Gesture gesture in db.AvailableGestures)
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                        this.GestureNames.Add(gesture.Name);
                    }
                }
            }
            // Add temporary gesture tags while the total number of gestures is < 8
            for (int i = this.GestureNames.Count; i < 8; i++) this.GestureNames.Add("temp");

            // Update the GestureResultView 
            this.GestureResultView.LoadGestures(GestureNames);
        }

        /// <summary> Gets the GestureResultView object which stores the detector results for display in the UI </summary>
        public GestureResultView GestureResultView { get; private set; }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        /// <summary>
        /// Gets the list of Gesture names for this detector. This is assumed to not change
        /// after the constructor is called.
        /// </summary>
        public List<string> GestureNames
        {
            get { return this.gestureNames; }

            private set
            {
                if (this.gestureNames != value)
                {
                    this.gestureNames = value;
                }
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources for the class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.vgbFrameReader != null)
                {
                    this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this.vgbFrameReader.Dispose();
                    this.vgbFrameReader = null;
                }

                if (this.vgbFrameSource != null)
                {
                    this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    this.vgbFrameSource.Dispose();
                    this.vgbFrameSource = null;
                }
            }
        }

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                // Only try to read valid frames
                if (frame != null)
                {
                    // Get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                    if (discreteResults != null)
                    {
                        // Identify each gesture in the set. 
                        // TODO: O(n*g) operation; can this be improved?
                        for (int i = 0; i < this.GestureNames.Count; i++)
                        {
                            foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                            {
                                if (gesture.Name.Equals(this.GestureNames[i]))
                                {
                                    DiscreteGestureResult result = null;
                                    discreteResults.TryGetValue(gesture, out result);

                                    if (result != null)
                                    {
                                        // Update the GestureResultView object with new gesture result values
                                        this.GestureResultView.UpdateGestureResult(true, i, result.Detected, result.Confidence);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            // update the GestureResultView object to show the 'Not Tracked' image in the UI
            for (int i = 0; i < this.GestureNames.Count; i++)
            {
                this.GestureResultView.UpdateGestureResult(false, i, false, 0.0f);
            }
        }
    }
}
