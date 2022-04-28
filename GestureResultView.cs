//------------------------------------------------------------------------------
// GestureResultView Class
//  Based on the example GestureResultView from 'DiscreteGestureBasics-WPF'.
//
//  Implementation modified to support tracking of multiple gestures, including
//  an overhaul to the front-end UI elements. This modification supports up to
//  8 gestures, & reports the "detected" and "confidence" variables for each.
//  This implementation also highlights only the gesture with the highest
//  confidence.
//------------------------------------------------------------------------------

namespace CSSpring2022MTM2
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class GestureResultView : INotifyPropertyChanged
    {
        // Array of brush colors to use for a tracked body; array position corresponds to the body colors used in the KinectBodyView class
        private readonly Brush[] trackedColors = new Brush[] { Brushes.Red, Brushes.Orange, Brushes.Green, Brushes.Blue, Brushes.Indigo, Brushes.Violet };

        // Color shown in the tab number of the TabView UI; indicates that this body is being tracked
        private Brush bodyColor = Brushes.Gray;

        //Brush color to use for the gesture with the highest confidence
        private readonly Brush activeGestureColor = Brushes.Lime;

        // The body index (0-5) associated with the current gesture detector
        private int bodyIndex = 0;

        // True, if the body is currently being tracked
        private bool isTracked = false;

        // Track a set of gestures by recording their names, whether it is detected, and the confidence
        private ObservableCollection<string> gestureNames = null;
        private ObservableCollection<bool> detections = null;
        private ObservableCollection<float> confidences = null;

        // The brush colors corresponding to each gesture in the above lists
        private ObservableCollection<Brush> gestureColors = null;

        /// <summary>
        /// Class Constructor. Initializes the collections for tracking gestures.
        /// </summary>
        /// <param name="bodyIndex">The index associated with the corresponding GestureDetector object.</param>
        /// <param name="isTracked">Whether this body is currently being tracked.</param>
        public GestureResultView(int bodyIndex, bool isTracked)
        {
            this.BodyIndex = bodyIndex;
            this.IsTracked = isTracked;

            // Allocate the collections for monitoring gestures
            this.GestureNames = new ObservableCollection<string>();
            this.Detections = new ObservableCollection<bool>();
            this.Confidences = new ObservableCollection<float>();
            this.GestureColors = new ObservableCollection<Brush>();
        }

        /// <summary>
        /// Loads a set of gestures. Sets the 'detected' value to false, 'confidence' value to 0,
        /// and the display color to Gray. This function assumes that the order of gestures does
        /// not change.
        /// </summary>
        /// <param name="gestureNames">The list of gesture names, provided by the GestureDetector class.</param>
        public void LoadGestures(List<string> gestureNames)
        {
            foreach (string name in gestureNames)
            {
                this.GestureNames.Add(name);
                this.Detections.Add(false);
                this.Confidences.Add(0.0f);
                this.GestureColors.Add(Brushes.Gray);
            }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> 
        /// Gets the body index associated with the current gesture detector result 
        /// </summary>
        public int BodyIndex
        {
            get
            {
                return this.bodyIndex;
            }

            private set
            {
                if (this.bodyIndex != value)
                {
                    this.bodyIndex = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets the body color corresponding to the body index for the result.
        /// Used only in the TabView tab UI to indicate that this body is being tracked.
        /// </summary>
        public Brush BodyColor
        {
            get
            {
                return this.bodyColor;
            }

            private set
            {
                if (this.bodyColor != value)
                {
                    this.bodyColor = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the body associated with the gesture detector is currently being tracked.
        /// </summary>
        public bool IsTracked
        {
            get
            {
                return this.isTracked;
            }

            private set
            {
                if (this.IsTracked != value)
                {
                    this.isTracked = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the collection of gesture names being tracked by this object
        /// </summary>
        public ObservableCollection<string> GestureNames
        {
            get { return this.gestureNames; }
            
            private set 
            { 
                if (this.gestureNames != value)
                {
                    this.gestureNames = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the collection of detection booleans, which indicate whether each gesture is being tracked.
        /// </summary>
        public ObservableCollection<bool> Detections
        {
            get { return this.detections; }
            
            private set 
            { 
                if (this.detections != value)
                {
                    this.detections = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the collection of confidence values, which range between 0-1 for each gesture being tracked.
        /// </summary>
        public ObservableCollection<float> Confidences
        {
            get { return this.confidences; }
            
            private set 
            { 
                if (this.confidences != value)
                {
                    this.confidences = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the brush colors for each gesture being tracked. This returns Brush.Gray for each gesture
        /// except for the gesture with the highest confidence.
        /// </summary>
        public ObservableCollection<Brush> GestureColors
        { 
            get { return this.gestureColors; } 

            private set
            {
                if (this.gestureColors != value)
                {
                    this.gestureColors = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Updates the gesture confidences for a given frame of data.
        /// </summary>
        /// <param name="isBodyTrackingIdValid">Used to validate body tracking.</param>
        /// <param name="idx">The index of the gesture to be updated.</param>
        /// <param name="isGestureDetected">True if this gesture is currently detected.</param>
        /// <param name="detectionConfidence">The confidence for the gesture being detected.</param>
        public void UpdateGestureResult(bool isBodyTrackingIdValid, int idx, bool isGestureDetected, float detectionConfidence)
        {
            this.IsTracked = isBodyTrackingIdValid;
            this.Confidences[idx] = 0.0f;

            if (!this.IsTracked)
            {
                // Stopped tracking
                this.Detections[idx] = false;
                this.BodyColor = Brushes.Gray;
            }
            else
            {
                // Tracking started or continuing, update gesture
                this.Detections[idx] = isGestureDetected;
                this.BodyColor = this.trackedColors[this.BodyIndex];

                if (this.Detections[idx])
                {
                    this.Confidences[idx] = detectionConfidence;
                }

                // Get the gesture with the highest confidence
                float maxConfidence = 0.0f;
                int maxIdx = 0;
                for (int i = 0; i < this.GestureNames.Count; i++)
                {
                    this.GestureColors[i] = Brushes.Gray;
                    if (this.Confidences[i] > maxConfidence)
                    {
                        maxConfidence = this.Confidences[i];
                        maxIdx = i;
                    }
                }

                // Color the highest confidence green
                if (maxConfidence > 0.0f) this.GestureColors[maxIdx] = activeGestureColor;
            }
        }

        /// <summary>
        /// Notifies UI that a property has changed
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param> 
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
