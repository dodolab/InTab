using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Graphics;
using InteractiveTable.GUI.Other;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.Threading;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.Core.ClientServer;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manazer starajici se o rozpoznavac transformaci bodu ze
    /// soustavy obrazu do soustavy stolu
    /// </summary>
   public class InputManager
    {


       private TableManager tableManager; // odkaz na manazera
       private RealTableManager realTableManager; // odkaz na real table manazera
       private ContourFilter processor; // processor na filtrovani kontur
       private InteractiveWindow interactiveWindow; // odkaz na interaktivni okno
       private Thread workThread; // timer na pozadi
       private Image<Bgr, byte> captured_frame; // zachyceny obrazek
       private HashSet<FoundRock> foundRocks = new HashSet<FoundRock>(); // nalezene kameny

       /// <summary>
       /// Konstruktor
       /// Nastavi veskere hodnoty z uzivatelskeho nastaveni
       /// </summary>
       /// <param name="tableManager"></param>
       public InputManager(TableManager tableManager)
       {
           this.tableManager = tableManager;


           if (CommonAttribService.DEFAULT_TEMPLATES != null)
           {
               LoadProcessor();

           }
       }

       private void LoadProcessor()
       {
           processor = new ContourFilter();
           processor.templates = CommonAttribService.DEFAULT_TEMPLATES;

           // nastavi defaultni hodnoty processoru

           processor.finder.maxRotateAngle = CaptureSettings.Instance().DEFAULT_ROTATE_ANGLE * Math.PI / 180;
           processor.minContourArea = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_AREA;
           processor.minContourLength = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_LENGTH;
           processor.finder.maxACFDescriptorDeviation = CaptureSettings.Instance().DEFAULT_MAX_ACF;
           processor.finder.minACF = CaptureSettings.Instance().DEFAULT_MIN_ACF;
           processor.finder.minICF = CaptureSettings.Instance().DEFAULT_MIN_ICF;
           processor.noiseFilter = CaptureSettings.Instance().DEFAULT_NOISE_ENABLED;
           processor.cannyThreshold = CaptureSettings.Instance().DEFAULT_NOISE_FILTER;
           processor.adaptiveThresholdBlockSize = CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_BLOCKSIZE;
           processor.adaptiveThresholdParameter = CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_PARAMETER;
           processor.blur = CaptureSettings.Instance().DEFAULT_BLUR_ENABLED;

           realTableManager = new RealTableManager(tableManager.TableDepositor);
           realTableManager.Init(CommonAttribService.DEFAULT_TEMPLATES);
       }

       public TableManager TableManager
       {
           get { return tableManager; }
           set { this.tableManager = value; }
       }

       public InteractiveWindow InteractiveWindow
       {
           get { return interactiveWindow; }
           set { interactiveWindow = value; }
       }

       public RealTableManager RealTableManager
       {
           get { return realTableManager; }
       }



       /// <summary>
       /// Spusti vlakno a jeste prenastavi hodnoty image processoru, kdyby se nahodou zmenily
       /// </summary>
       public bool RunThread()
       {
           if (processor == null) LoadProcessor();

           if (CommonAttribService.DEFAULT_TEMPLATES == null)
           {
               MessageBox.Show("Nejsou zde žádné šablony! Nastavte nejprve šablony");
               return false;
           }
           if (CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Count == 0)
           {
               MessageBox.Show("Nastavené šablony nemají přiřazené kameny! Přiřaďte jim nejprve kameny ve správci kontur.");
               return false;
           }
           else
           {
               try
               {
                   // zmena nastaveni (pro jistotu)
                   processor.finder.maxRotateAngle = CaptureSettings.Instance().DEFAULT_ROTATE_ANGLE * Math.PI / 180;
                   processor.minContourArea = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_AREA;
                   processor.minContourLength = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_LENGTH;
                   processor.finder.maxACFDescriptorDeviation = CaptureSettings.Instance().DEFAULT_MAX_ACF;
                   processor.finder.minACF = CaptureSettings.Instance().DEFAULT_MIN_ACF;
                   processor.finder.minICF = CaptureSettings.Instance().DEFAULT_MIN_ICF;
                   processor.noiseFilter = CaptureSettings.Instance().DEFAULT_NOISE_ENABLED;
                   processor.cannyThreshold = CaptureSettings.Instance().DEFAULT_NOISE_FILTER;
                   processor.adaptiveThresholdBlockSize = CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_BLOCKSIZE;
                   processor.adaptiveThresholdParameter = CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_PARAMETER;
                   processor.blur = CaptureSettings.Instance().DEFAULT_BLUR_ENABLED;
               }
               catch(Exception e)
               {
                   MessageBox.Show("Vznikl problém při inicializaci kamery!!; EX: "+e.Message+";;;"+e.StackTrace);
                   return false;
               }
               realTableManager.Init(CommonAttribService.DEFAULT_TEMPLATES);

               workThread = new Thread(Application_Idle);
               workThread.IsBackground = true;
               workThread.Start();
               return true;
           }
       }

       /// <summary>
       /// Vrati true, pokud vlakno bezi
       /// </summary>
       /// <returns></returns>
       public bool IsRunning()
       {
           return (workThread != null && workThread.IsAlive);
       }

       /// <summary>
       /// Zastavi pracovni vlakno
       /// </summary>
       public void StopThread()
       {
           if(workThread != null) workThread.Abort();
       }

       /// <summary>
       /// Casova smycka - ziska obrazek z kamery a zpracuje ho
       /// </summary>
       private void Application_Idle()
       {
           while (true)
           {
               try
               {
                   // ziska obrazek
                   Image<Bgr, byte> tempFrame = captured_frame = CameraManager.GetImage();
                   // obrazek se prizpusobi podle nastaveni slideru
                   if (CaptureSettings.Instance().DEFAULT_DILATATION != 0) tempFrame._Dilate((int)CaptureSettings.Instance().DEFAULT_DILATATION);
                   if (CaptureSettings.Instance().DEFAULT_ERODE != 0) tempFrame._Erode((int)CaptureSettings.Instance().DEFAULT_ERODE);
                   if (CaptureSettings.Instance().DEFAULT_GAMMA != 1) tempFrame._GammaCorrect((double)CaptureSettings.Instance().DEFAULT_GAMMA);
                   if (CaptureSettings.Instance().DEFAULT_INVERT_ENABLED) tempFrame._Not();
                   if (CaptureSettings.Instance().DEFAULT_NORMALIZE_ENABLED) tempFrame._EqualizeHist();

                   // osetreni rotace
                   if (CalibrationSettings.Instance().CALIBRATION_ROTATION != 0)
                   {
                       tempFrame = tempFrame.Rotate(CalibrationSettings.Instance().CALIBRATION_ROTATION, new Bgr(0, 0, 0), true);
                   }

                   processor.ProcessImage(tempFrame); // zpracovani obrazku processorem
                   if (interactiveWindow != null && interactiveWindow.IsVisible) interactiveWindow.DrawImage(tempFrame, processor);
                   InputLogic();


               }
               catch {
                  // Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!! VYJIMKA PRI VYKRESLOVANI");
               }
           }
       }



       /// <summary>
       /// Vezme nalezene kontury, transformuje jejich pozice do souradneho systemu stolu
       /// vcetne perspektivniho zkresleni a nakonec vytvori deskriptory nalezenych kamenu
       /// a vsechno preda mergovaci soustave, ktera ty stavy slouci
       /// </summary>
       private void InputLogic()
       {
           foundRocks.Clear();

           if (tableManager.IsRunning)
           {

               foreach (FoundTemplateDesc found in processor.foundTemplates)
               {
                   System.Drawing.Rectangle foundRect = found.sample.contour.SourceBoundingRect;

                   #region hledani transformovaneho bodu
                   
                   try
                   {
                       double found_pos_x = foundRect.Left + foundRect.Size.Width / 2;
                       double found_pos_y = foundRect.Top + foundRect.Size.Height / 2;

                       double mx = found_pos_x; // X-ova souradnice bodu v lichobezniku
                       double my = captured_frame.Height - found_pos_y; // Y-ova souradnice bodu v lichobezniku

                       double ax = CalibrationSettings.Instance().CALIBRATION_POINT_A.X; // X-ova souradnice bodu A v lichobezniku

                       // ODHAD OSTATNICH BODU PODLE ZADANYCH HODNOT ( a = (2*r1r2)/(r1+1))
                       double r1 = CalibrationSettings.Instance().CALIBRATION_PERSPECTIVE;
                       double r2 = CalibrationSettings.Instance().CALIBRATION_POINT_C.X - CalibrationSettings.Instance().CALIBRATION_POINT_A.X;
                       double bx = ax + (2 * (r1 * r2)) / (r1 + 1); // X-ova souradnice bodu B v lichobezniku
                       double cx = CalibrationSettings.Instance().CALIBRATION_POINT_C.X; // X-ova souradnice bodu C v lichobezniku
                       double dx = ax + bx - cx; // X-ova souradnice bodu D v lichobezniku

                       double cy = captured_frame.Height - CalibrationSettings.Instance().CALIBRATION_POINT_C.Y; // Y-ova souradnice bodu C v lichobezniku
                       double ay = captured_frame.Height - CalibrationSettings.Instance().CALIBRATION_POINT_A.Y; // Y-ova souradnice bodu A v lichobezniku

                       double pom = (my - ay) / (cy - ay); // pomer vzdalenosti BOD-DOLNI ZAKLADNA a HORNI-DOLNI zakladna
                       double r1m = mx - (ax + (dx - ax) * pom); // vzdalenost bodu M od leveho ramene
                       double r1r2 = pom * (cx - bx - dx + ax) + bx - ax; // delka usecky protinajici bod M a prochazejici rameny

                       double ab = Math.Abs(bx - ax); // delka usecky AB
                       double cd = Math.Abs(cx - dx); // delka usecky CD
                       double alfa = r1r2 / ab; // perspektivni uhel zkresleni
                       double maximum = captured_frame.Height / (cd / ab); // maximalni zkresleni
                       double real = (captured_frame.Height) * (pom / alfa); // zkresleni vzhledem k bodu M

                       // prepocet souradnic bodu na obdelnik, ktery predstavuje fotografie
                       double mxx = (ax < bx) ? captured_frame.Width * (r1m / r1r2) : captured_frame.Width - captured_frame.Width * r1m / r1r2;
                       double mxy = (ay > cy) ? (captured_frame.Height * real / maximum) : captured_frame.Height - (captured_frame.Height * real / maximum);


                       // PREPOCET Z OBRAZKOVEHO OBDELNIKA NA STOLNI OBDELNIK
                       mxx *= CommonAttribService.ACTUAL_TABLE_WIDTH / ((double)captured_frame.Width);
                       mxy *= CommonAttribService.ACTUAL_TABLE_HEIGHT / ((double)captured_frame.Height);
                       foundRocks.Add(CreateFoundRock(found.template.name, new FPoint(mxx - CommonAttribService.ACTUAL_TABLE_WIDTH / 2, CommonAttribService.ACTUAL_TABLE_HEIGHT / 2 - mxy),processor.templates, found.rate, found.scale, found.angle));
                   }
                   catch
                   {
                       MessageBox.Show("Nastal problém při přepočtu kalibrace. Nastavte znovu kalibraci!");
                       StopThread();
                   }
                   #endregion

                  
               }
               // slouceni obou systemu --> HLAVNI LOGIKA ROZPOZNAVANI
               realTableManager.MergeSystems(foundRocks);
           }
       }

       /// <summary>
       /// Vytvori objekt FoundRock na zaklade informaci o konture
       /// </summary>
       /// <param name="contureName">Nazev kontury</param>
       /// <param name="position">Pozice kontury</param>
       /// <param name="templates">List znamych sablon</param>
       /// <returns></returns>
       private FoundRock CreateFoundRock(String contureName, FPoint position, Templates templates, double radius, double scale, double angle)
       {
           FoundRockType type = FoundRockType.GRAVITON;
           ContourRock rck = (templates.rockSettings.Where(name => name.contour_name.Equals(contureName))).First();
           if (rck.rock is Generator) type = FoundRockType.GENERATOR;
           if (rck.rock is BlackHole) type = FoundRockType.BLACKHOLE;
           if (rck.rock is Graviton) type = FoundRockType.GRAVITON;
           if (rck.rock is Magneton) type = FoundRockType.MAGNETON;

           return new FoundRock(contureName, type, position, radius, scale, angle);
       }

    }
}
