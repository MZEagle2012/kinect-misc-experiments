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

namespace KinectViewer
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private DepthImagePixel[] depthPixels;
        private byte[] colorPixels;
        private WriteableBitmap depthBitmap;
        private bool isClosing = false;

        public MainWindow()
        {
            InitializeComponent();
            StartKinect();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isClosing = true;

            if (sensor == null)
            {
                return;
            }
            sensor.DepthFrameReady -= Sensor_DepthFrameReady;
            sensor.SkeletonFrameReady -= Sensor_SkeletonFrameReady;

            if (sensor.SkeletonStream.IsEnabled)
            {
                sensor.SkeletonStream.Disable();
            }

            if (sensor.DepthStream.IsEnabled)
            {
                sensor.DepthStream.Disable();
            }

            if (sensor.IsRunning)
            {
                sensor.Stop();
            }

            sensor = null;
        }

        private void DepthToColor(int depth, out byte b, out byte g, out byte r, out byte a)
        {
            int minDepth = 800;
            int maxDepth = 2500;

            a = 255;

            if (depth == 0)
            {
                b = 0;
                g = 0;
                r = 0;
                return;
            }

            if (depth <= minDepth)
            {
                b = 0;
                g = 0;
                r = 255;
                return;
            }

            if (depth >= maxDepth)
            {
                b = 255;
                g = 0;
                r = 0;
                return;
            }

            double ratio = (double)(depth - minDepth) / (maxDepth - minDepth);

            if (ratio < 0.25)
            {
                b = 0;
                g = (byte)(ratio / 0.25 * 255);
                r = 255;
            }
            else if (ratio < 0.50)
            {
                b = 0;
                g = 255;
                r = (byte)((1.0 - (ratio - 0.25) / 0.25) * 255);
            }
            else if (ratio < 0.75)
            {
                b = (byte)(((ratio - 0.50) / 0.25) * 255);
                g = 255;
                r = 0;
            }
            else
            {
                b = 255;
                g = (byte)((1.0 - (ratio - 0.75) / 0.25) * 255);
                r = 0;
            }
        }

        private void DrawJoint(Joint joint)
        {
            DepthImagePoint depthPoint =
                sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(
                    joint.Position,
                    DepthImageFormat.Resolution640x480Fps30
                );

            double x =
                depthPoint.X *
                SkeletonCanvas.ActualWidth /
                sensor.DepthStream.FrameWidth;

            double y =
                depthPoint.Y *
                SkeletonCanvas.ActualHeight /
                sensor.DepthStream.FrameHeight;

            Ellipse circle = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(circle, x - circle.Width / 2);
            Canvas.SetTop(circle, y - circle.Height / 2);

            SkeletonCanvas.Children.Add(circle);
        }

        private void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (isClosing)
            {
                return;
            }

            using (DepthImageFrame frame = e.OpenDepthImageFrame())
            {
                if (frame == null)
                {
                    return;
                }

                frame.CopyDepthImagePixelDataTo(depthPixels);

                for (int i = 0; i < depthPixels.Length; i++)
                {
                    byte b, g, r, a;
                    DepthToColor(depthPixels[i].Depth, out b, out g, out r, out a);

                    int colorIndex = i * 4;

                    colorPixels[colorIndex] = b;
                    colorPixels[colorIndex + 1] = g;
                    colorPixels[colorIndex + 2] = r;
                    colorPixels[colorIndex + 3] = a;
                }

                depthBitmap.WritePixels(
                    new Int32Rect(
                        0,
                        0,
                        depthBitmap.PixelWidth,
                        depthBitmap.PixelHeight
                    ),
                    colorPixels,
                    depthBitmap.PixelWidth * 4,
                    0
                );
            }
        }

        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                {
                    return;
                }

                Skeleton[] skeletons =
                    new Skeleton[frame.SkeletonArrayLength];

                frame.CopySkeletonDataTo(skeletons);

                SkeletonCanvas.Children.Clear();

                foreach (Skeleton skeleton in skeletons)
                {
                    if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    {
                        continue;
                    }

                    Joint head = skeleton.Joints[JointType.Head];

                    DrawJoint(head);

                    TrackingText.Text =
                        $"Head\n" +
                        $"X: {head.Position.X:F2} m\n" +
                        $"Y: {head.Position.Y:F2} m\n" +
                        $"Z: {head.Position.Z:F2} m";
                }
            }
        }

        private void StartKinect()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No Kinect detected.");
                return;
            }

            sensor = KinectSensor.KinectSensors[0];

            sensor.DepthStream.Enable(
                DepthImageFormat.Resolution640x480Fps30
            );

            depthPixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];

            colorPixels = new byte[sensor.DepthStream.FramePixelDataLength * 4];

            depthBitmap = new WriteableBitmap(
                sensor.DepthStream.FrameWidth,
                sensor.DepthStream.FrameHeight,
                96,
                96,
                PixelFormats.Bgra32,
                null
                );

            DepthImage.Source = depthBitmap;

            sensor.DepthFrameReady += Sensor_DepthFrameReady;

            sensor.SkeletonStream.Enable();

            sensor.DepthFrameReady += Sensor_DepthFrameReady;
            sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;

            sensor.Start();
        }
    }
}
