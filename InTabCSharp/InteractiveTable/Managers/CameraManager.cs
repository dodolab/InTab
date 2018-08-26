using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Class that holds captured images from camera
    /// </summary>
    public class CameraManager
    {
        /// <summary>
        /// Webcam pointer
        /// </summary>
        private static Emgu.CV.Capture camera;
        /// <summary>
        /// Index of current webcam in use
        /// </summary>
        private static int ACTUAL_CAMERA_INDEX;
        /// <summary>
        /// Last captured frame
        /// </summary>
        private static Image<Bgr, byte> lastFrame;
        /// <summary>
        /// Time the last frame was captured
        /// </summary>
        private static DateTime lastFrameTime;
        
        public static void Reinitialize()
        {
            ACTUAL_CAMERA_INDEX = CaptureSettings.Instance().DEFAULT_CAMERA_INDEX;
            camera = new Emgu.CV.Capture(ACTUAL_CAMERA_INDEX);
        }

        /// <summary>
        /// Captures a frame from camera
        /// </summary>
        private static void CaptureFrame()
        {
            try
            {
                if (CaptureSettings.Instance().MOTION_DETECTION)
                {
                    Image<Bgr, byte> originalFrame = lastFrame.Clone();
                    lastFrame = camera.QueryFrame();
                    CalculateDiff(originalFrame, lastFrame);
                }
                else
                {
                    lastFrame = camera.QueryFrame();
                }
                    lastFrameTime = DateTime.Now;
            }
            catch
            {
               Console.WriteLine("Unable to capture camera image");
            }
        }

        /// <summary>
        /// Disposes current camera
        /// </summary>
        public static void DisposeCamera()
        {
            if (camera != null) camera.Dispose(); 
        }

        /// <summary>
        /// Gets a copy of last captured image
        /// </summary>
        /// <returns></returns>
        public static Image<Bgr, byte> GetImage()
        {
            if (ACTUAL_CAMERA_INDEX != CaptureSettings.Instance().DEFAULT_CAMERA_INDEX) Reinitialize();
            if (lastFrame == null || (DateTime.Now - lastFrameTime).TotalMilliseconds > 25) CaptureFrame();
            // just for sure...to avoid errors
            if (lastFrame == null) lastFrame = new Image<Bgr, byte>(50, 50);

            return lastFrame.Clone();
        }


        // Last difference between two image
        public static double lastDiff { get; set; }
        // Current difference between two images
        public static double actualDiff { get; set; }
        // number of frames without any motion detection
        public static int peaceCounter { get; set; }
        // if true, there is no motion in the camera
        public static bool isInPeace { get; set; }
        // number of frames the camera is capturing a motion
        public static int unpeaceCounter { get; set; }

        /// <summary>
        /// Calculates a difference descriptor between two images in order to detect motion
        /// </summary>
        private static void CalculateDiff(Image<Bgr, byte> originalFrame, Image<Bgr,byte> lastFrame)
        {
            Image<Gray, Byte> frameG = originalFrame.Convert<Gray, Byte>();
            Image<Gray, Byte> lastFrameG = lastFrame.Convert<Gray, Byte>();

            // Perform thresholding to remove noise and boost "new introductions"
            Image<Gray, Byte> threshOrig = new Image<Gray, byte>(originalFrame.Width, originalFrame.Height);
            CvInvoke.cvThreshold(frameG, threshOrig, 20, 255, THRESH.CV_THRESH_BINARY);

            Image<Gray, Byte> threshLast = new Image<Gray, byte>(lastFrame.Width, lastFrame.Height);
            CvInvoke.cvThreshold(lastFrameG, threshLast, 20, 255, THRESH.CV_THRESH_BINARY);

            // Perform erosion to remove camera noise
            Image<Gray, Byte> erodedOrig = new Image<Gray, byte>(originalFrame.Width, originalFrame.Height);
            CvInvoke.cvErode(threshOrig, erodedOrig, IntPtr.Zero, 2);

            Image<Gray, Byte> erodedLast = new Image<Gray, byte>(lastFrame.Width, lastFrame.Height);
            CvInvoke.cvErode(threshLast, erodedLast, IntPtr.Zero, 2);

            double l2_norm = (erodedOrig - erodedLast).Norm;
            lastDiff = actualDiff;
            actualDiff = l2_norm;            

            if (l2_norm < 2000 || Math.Abs(actualDiff - lastDiff) > CaptureSettings.Instance().MOTION_TOLERANCE)
            {
                isInPeace = false;
                unpeaceCounter++;
                peaceCounter = -1;
            }
            else
            {
                isInPeace = true;
                unpeaceCounter = -1;
                peaceCounter++;
            }
        }
    }
}
