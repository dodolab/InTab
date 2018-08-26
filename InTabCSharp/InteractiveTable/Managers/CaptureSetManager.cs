using System;
using InteractiveTable.GUI.CaptureSet;
using Emgu.CV;
using System.Threading;
using Emgu.CV.Structure;
using InteractiveTable.Controls;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manager for capture window
    /// </summary>
   public class CaptureSetManager
    {
       private CaptureWindow captureSetWindow; 
       private CaptureSetController captureSetControl;
       private Boolean captureFromCam = true; //  if false, the frame will be loaded from a file
       private Thread WorkerThread; 
       private DateTime timeDat; // last capture time
       private int timeInt; // current capture time

       private double dilatation = 0;
       private double erode = 0;
       private double gamma = 0;
       private bool invert, normalize, blackWhite, blur, binary;


       public CaptureSetManager()
       {

       }

       public Boolean CaptureFromCam
       {
           get { return captureFromCam; }
           set { this.captureFromCam = value; }
       }

       public CaptureSetController CaptureSetControl
       {
           get { return captureSetControl; }
           set { this.captureSetControl = value; }
       }

       public CaptureWindow CaptureSetWindow
       {
           get { return captureSetWindow; }
           set { this.captureSetWindow = value; }
       }
        
       public void Initialize()
       {
 
           try
           {
             captureFromCam = false;
             captureSetControl.ApplyCamSettings();
             
           }
           catch (NullReferenceException ex)
           {
               System.Windows.MessageBox.Show(ex.Message);
           }

           captureSetControl.ApplySettings();
           ProcessFrame(); // process first image

           // set working thread
           WorkerThread = new Thread(new ThreadStart(Application_Idle));
           WorkerThread.IsBackground = true;
           WorkerThread.Priority = ThreadPriority.Lowest; 
           WorkerThread.SetApartmentState(ApartmentState.STA); // set STA so that we may access UI from this thread
           WorkerThread.Start();
       }


       /// <summary>
       /// Rendering loop that processes detection
       /// </summary>
       private void Application_Idle()
       {
           while (true)
           {
               timeDat = DateTime.Now;
               ProcessFrame();
               timeInt = (DateTime.Now - timeDat).Milliseconds;
               captureSetWindow.TimeDelay = timeInt;
           }
       }

       /// <summary>
       /// Stops the processing thread
       /// </summary>
       public void StopThread()
       {
           WorkerThread.Abort();
       }

       /// <summary>
       /// Processes current frame
       /// </summary>
       private void ProcessFrame()
       {
           try
           {
               Image<Bgr, byte> tempFrame = null;

               if (captureFromCam)
                   captureSetWindow.Frame = CameraManager.GetImage();
               else
               {
                   captureSetWindow.Frame = captureSetWindow.ImageFrame.Clone();
               }

               // get values of properties from the UI
               captureSetWindow.Dispatcher.BeginInvoke(new Action(() =>
               {
                   dilatation = captureSetWindow.dilatationSlider.Value;
                   erode = captureSetWindow.erodeSlider.Value;
                   gamma = captureSetWindow.gammaSlider.Value;
                   invert = (bool)captureSetWindow.invertCheck.IsChecked;
                   normalize = (bool)captureSetWindow.normalizeCheck.IsChecked;
                   blackWhite = (bool)captureSetWindow.showBlackWhiteCheck.IsChecked;
                   blur = (bool)captureSetWindow.blurCheck.IsChecked;
                   binary = (bool)captureSetWindow.showBinaryCheck.IsChecked;
               }));

               // apply other settings
               if (dilatation != 0) captureSetWindow.Frame._Dilate((int)dilatation);
               if (erode != 0) captureSetWindow.Frame._Erode((int)erode);
               if (gamma != 1) captureSetWindow.Frame._GammaCorrect((double)gamma);
               if (invert) captureSetWindow.Frame._Not();
               if (normalize) captureSetWindow.Frame._EqualizeHist();


               tempFrame = captureSetWindow.Frame.Clone();
                
               if (blackWhite) tempFrame = tempFrame.Convert<Gray, Byte>().Convert<Bgr, byte>();

                if (blur)
               {
                   tempFrame = tempFrame.PyrDown().PyrUp();
               }
               captureSetWindow.Processor.ProcessImage(captureSetWindow.Frame);


               if (binary)
                   captureSetWindow.captureBox.Image = captureSetWindow.Processor.binarizedFrame;
               else
                   captureSetWindow.captureBox.Image = tempFrame;

           }
           catch (Exception ex)
           {
                // no-op here
           }
       }
    }
}
