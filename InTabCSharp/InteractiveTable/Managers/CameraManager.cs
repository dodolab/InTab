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
    /// Trida, ktera v sobe drzi odkaz na kameru;
    /// jediny zpusob, jak pristupovat k zachycenym obrazkum z kamery, je pomoci teto tridy!!
    /// </summary>
    public class CameraManager
    {
        /// <summary>
        /// webova kamera
        /// </summary>
        private static Emgu.CV.Capture camera;
        /// <summary>
        /// Index udavajici, ktera kamera se bude pouzivat. Pouziva se zde proto,
        /// aby se kamera reinicializovala, pokud se zmeni nastaveni tohoto indexu
        /// </summary>
        private static int ACTUAL_CAMERA_INDEX;
        /// <summary>
        /// Naposledy zachyceny obrazek
        /// </summary>
        private static Image<Bgr, byte> lastFrame;
        /// <summary>
        /// Cas posledniho rozpoznani
        /// </summary>
        private static DateTime lastFrameTime;

        /// <summary>
        /// Reinicializuje kameru
        /// </summary>
        public static void Reinitialize()
        {
            ACTUAL_CAMERA_INDEX = CaptureSettings.Instance().DEFAULT_CAMERA_INDEX;
            camera = new Emgu.CV.Capture(ACTUAL_CAMERA_INDEX);
          

        }

        /// <summary>
        /// Zachyti obrazek z kamery
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
               Console.WriteLine("doslo k vyjimce pri ziskavani obrazu z kamery");
            }
        }

        /// <summary>
        /// Zrusi zachytavani z kamery
        /// </summary>
        public static void DisposeCamera()
        {
            if (camera != null) camera.Dispose(); 
        }

        /// <summary>
        /// Vrati naposledy zachyceny obrazek, resp. jeho kopii
        /// </summary>
        /// <returns></returns>
        public static Image<Bgr, byte> GetImage()
        {
            if (ACTUAL_CAMERA_INDEX != CaptureSettings.Instance().DEFAULT_CAMERA_INDEX) Reinitialize();
            if (lastFrame == null || (DateTime.Now - lastFrameTime).TotalMilliseconds > 25) CaptureFrame();
            // pro jistotu...
            if (lastFrame == null) lastFrame = new Image<Bgr, byte>(50, 50);

            return lastFrame.Clone();
        }


        // posledni zmena mezi 2 obrazky
        public static double lastDiff { get; set; }
        // aktualni zmena mezi 2 obrazky
        public static double actualDiff { get; set; }
        // pocet pruchodu, kdy se v obrazku nic nehybe
        public static int peaceCounter { get; set; }
        // pokud true, v obrazku se nic nehybe
        public static bool isInPeace { get; set; }
        // pocet pruchodu, kdy se v obrazku neco hybe
        public static int unpeaceCounter { get; set; }

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
            Console.WriteLine(l2_norm);
            lastDiff = actualDiff;
            actualDiff = l2_norm;

            

            if (l2_norm < 2000 || Math.Abs(actualDiff - lastDiff) > CaptureSettings.Instance().MOTION_TOLERANCE)
            {
                Console.WriteLine("UNPEACE");
                isInPeace = false;
                unpeaceCounter++;
                peaceCounter = -1;
            }
            else
            {
                Console.WriteLine("PEACE");
                isInPeace = true;
                unpeaceCounter = -1;
                peaceCounter++;
            }

        }
    }
}
