using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

namespace CSSpring2022MTM2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary> Active Kinect sensor </summary>
        private KinectSensor kinectSensor = null;

        /// <summary> Array for the bodies (TODO: needed?) (Kinect will track up to 6 bodies) </summary>
        private Body[] bodies = null;

        /// <summary> Reader for body frames </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary> Current status text to display </summary>
        private string statusText = null;

        /// <summary> KinectBodyView handles drawing Kinect bodies to a View box in the UI </summary>
        private KinectBodyView kinectBodyView = null;

        /// <summary> List of gesture detectors; there will be one per body </summary>
        //private List<GestureDetector> gestureDetectorList = null;

        public MainWindow()
        {
            // Connect to Kinect Sensor
            this.kinectSensor = KinectSensor.GetDefault();

            // TODO: Add code for detecting KinectSensor availability for status bar

            // Open sensor
            this.kinectSensor.Open();

            // TODO: Set Status Text

            // TODO: BodyFrameReader

            // Initialize BodyView object
            this.kinectBodyView = new KinectBodyView(this.kinectSensor);

            // TODO: GestureDetector

            // Initialize the Main Window
            InitializeComponent();

            // Set DataContext objects for UI
            this.DataContext = this;
            this.kinectBodyViewbox.DataContext = this.kinectBodyView;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // Is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
    } // class MainWindow
} // namespace CSSpring2022MTM2
