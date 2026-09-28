using System;
using Microsoft.Kinect;

namespace KinectLearning
{
    internal class KinectTest
    {
        static void Sensor_DepthFrameReady(
    object sender,
    DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame frame = e.OpenDepthImageFrame())
            {
                if (frame == null)
                {
                    return;
                }

                DepthImagePixel[] pixels =
                    new DepthImagePixel[frame.PixelDataLength];

                frame.CopyDepthImagePixelDataTo(pixels);

                byte[] grayscale =
                    new byte[frame.PixelDataLength];

                for (int i = 0; i < pixels.Length; i++)
                {
                    int depth = pixels[i].Depth;

                    if (depth == 0)
                    {
                        grayscale[i] = 0;
                    }
                    else
                    {
                        int intensity = 255 - (depth / 16);

                        if (intensity < 0)
                            intensity = 0;

                        if (intensity > 255)
                            intensity = 255;

                        grayscale[i] = (byte)intensity;
                    }
                }

                Console.WriteLine(
                    $"Generated grayscale frame with {grayscale.Length} pixels."
                );
            }
        }
        public static void Run()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                Console.WriteLine("No Kinect sensor detected.");
                return;
            }

            KinectSensor sensor = KinectSensor.KinectSensors[0];

            Console.WriteLine($"Sensor status: {sensor.Status}");

            Console.WriteLine("Kinect started.");

            sensor.DepthStream.Enable();

            sensor.DepthFrameReady += Sensor_DepthFrameReady;

            sensor.Start();

            Console.ReadLine();

            sensor.Stop();
        }
    }
}
