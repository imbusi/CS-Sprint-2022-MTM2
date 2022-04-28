namespace CSSpring2022MTM2
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // The standard kinectSensor object
        private KinectSensor kinectSensor = null;

        // Array of Body objects; array will be initialized to six bodies when data arrives
        private Body[] bodies = null;

        // Reads body data for each frame incoming from the Kinect
        private BodyFrameReader bodyFrameReader = null;

        /// <summary> Current status text to display </summary>
        private string statusText = null;

        // Vizualization tool; uses class from Kinect examples
        private KinectBodyView kinectBodyView = null;

        // Set of gesture detectors and their corresponding visualizers
        // There will be one detector/resultview per body (6 total of each)
        private List<GestureDetector> gestureDetectorList = null;
        private List<GestureResultView> gestureViewList = null;

        /// <summary>
        /// Initializer for the main application window.
        /// </summary>
        public MainWindow()
        {
            // Connect to Kinect Sensor
            this.kinectSensor = KinectSensor.GetDefault();

            // TODO: Add code for detecting KinectSensor availability for status bar

            // Open sensor
            this.kinectSensor.Open();

            // TODO: Set Status Text

            // Get the BodyFrameReader object & add frame event handler
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

            // Initialize BodyView object
            this.kinectBodyView = new KinectBodyView(this.kinectSensor);

            // Initialize the GestureDetector and GestureResultView collections
            this.gestureDetectorList = new List<GestureDetector>();
            this.gestureViewList = new List<GestureResultView>();

            // Initialize the Main Window
            InitializeComponent();

            // Set DataContext objects for the UI
            this.DataContext = this;
            this.kinectBodyViewbox.DataContext = this.kinectBodyView;

            // Generate 6 new Detector and ResultView objects
            int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; i++)
            {
                // Generate 6 new detector objects
                GestureResultView result = new GestureResultView(i, false);
                GestureDetector detector = new GestureDetector(this.kinectSensor, result);
                this.gestureDetectorList.Add(detector);
                this.gestureViewList.Add(result);
            }

            // Connect the list of GestureResultView objects to the TabView UI
            this.GestureViewBox.ItemsSource = this.gestureViewList;
            this.GestureViewBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Handler for when the application window is closed.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.FrameArrived -= this.Reader_BodyFrameArrived;
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the body frame data arriving from the sensor and updates the associated gesture detector object for each body
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        // Initialize the body array, which always contains 6 bodies
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                // Send this frame to the visualizer class
                this.kinectBodyView.UpdateBodyFrame(this.bodies);

                if (this.bodies != null)
                {
                    // Loop through all bodies and update detectors
                    int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
                    for (int i = 0; i < maxBodies; ++i)
                    {
                        Body body = this.bodies[i];
                        ulong trackingId = body.TrackingId;

                        // If the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != this.gestureDetectorList[i].TrackingId)
                        {
                            this.gestureDetectorList[i].TrackingId = trackingId;

                            // If the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // If the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                            this.gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                    }
                }
            }
        } // private void Reader_BodyFrameArrived
    } // class MainWindow
} // namespace CSSpring2022MTM2
