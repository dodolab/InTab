using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.GUI.CaptureSet;
using Emgu.CV;
using InteractiveTable.Core.Graphics;
using System.Threading;
using Emgu.CV.Structure;
using InteractiveTable.Controls;
using InteractiveTable.Settings;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manazer pro nastaveni v okne pro nastaveni kontur
    /// </summary>
   public class CaptureSetManager
    {
       private CaptureWindow captureSetWindow; 
       private CaptureSetController captureSetControl;
       private Boolean captureFromCam = true; // pokud false, bude se nacitat misto kamery obrazek ze souboru
       private Thread WorkerThread; // pracovni vlakno
       private DateTime timeDat; // datum posledniho zachytavani
       private int timeInt; // aktualni zachytavaci cas

       // promenne uchovavajici synchronizacni pristup do uzivatelskeho nastaveni
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

       /// <summary>
       /// Hlavni inicializace, nastavi pracovni vlakno
       /// </summary>
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
           ProcessFrame(); // zpracuj prvni obrazek

           // nastavi pacovni vlakno
           WorkerThread = new Thread(new ThreadStart(Application_Idle));
           WorkerThread.IsBackground = true;
           WorkerThread.Priority = ThreadPriority.Lowest; // nejnizsi priorita
           WorkerThread.SetApartmentState(ApartmentState.STA); // aby se vlakno dostalo k UI komponentam
           WorkerThread.Start();
       }


       /// <summary>
       /// Hlavni vykreslovaci smycka, zpracovava kontury
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
       /// Zastavi vypocetni vlakno
       /// </summary>
       public void StopThread()
       {
           WorkerThread.Abort();
       }

       /// <summary>
       /// Zpracuje aktualne zachyceny obrazek
       /// </summary>
       private void ProcessFrame()
       {
           try
           {
               // nastaveni vlastnosti kamery (pokud se zmenily)
               Image<Bgr, byte> tempFrame = null;

               if (captureFromCam)
                   captureSetWindow.Frame = CameraManager.GetImage();
               else
               {
                   captureSetWindow.Frame = captureSetWindow.ImageFrame.Clone();
               }

               // ulozeni promennych z VIEW (provede se asynchronne)
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


               // obrazek se prizpusobi podle nastaveni slideru
               if (dilatation != 0) captureSetWindow.Frame._Dilate((int)dilatation);
               if (erode != 0) captureSetWindow.Frame._Erode((int)erode);
               if (gamma != 1) captureSetWindow.Frame._GammaCorrect((double)gamma);
               if (invert) captureSetWindow.Frame._Not();
               if (normalize) captureSetWindow.Frame._EqualizeHist();

               // nyni vse, co bude na obrazovce, uz se nebude zpracovavat, protoze
               // se to zpracuje jeste jednou a v efektivnejsim poradi, proto je potreba
               // obrazek zkopirovat
               tempFrame = captureSetWindow.Frame.Clone();

               // nastaveni ostatnich vlastnosti obrazku, obrazek bude pote vykreslen
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
              // Console.WriteLine(ex.Message);
           }
       }
    }
}
