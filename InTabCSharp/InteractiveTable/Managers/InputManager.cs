using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTable.Core.Graphics;
using InteractiveTable.GUI.Other;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.Threading;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.Capture;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manager that transforms points from the camera space to the interactive space
    /// </summary>
   public class InputManager
    {


       private TableManager tableManager; 
       private RealTableManager realTableManager; 
       private ContourFilter processor; 
       private InteractiveWindow interactiveWindow; 
       private Thread workThread; 
       private Image<Bgr, byte> captured_frame; // captured frames
       private HashSet<FoundRock> foundRocks = new HashSet<FoundRock>(); // detected stones
        
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

           // set default values

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
       /// Executes processing thread
       /// </summary>
       public bool RunThread()
       {
           if (processor == null) LoadProcessor();

           if (CommonAttribService.DEFAULT_TEMPLATES == null)
           {
               MessageBox.Show("There are no templates configured. To work with AR, set templates first!");
               return false;
           }
           if (CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Count == 0)
           {
               MessageBox.Show("Templates don't have any stones assigned. Assign some in Contour settings.");
               return false;
           }
           else
           {
               try
               {
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
                   MessageBox.Show("An error occurred during camera initialization: "+e.Message+";;;"+e.StackTrace);
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
       /// Returns true if the working thread is running
       /// </summary>
       /// <returns></returns>
       public bool IsRunning()
       {
           return (workThread != null && workThread.IsAlive);
       }

       /// <summary>
       /// Stops the working thread
       /// </summary>
       public void StopThread()
       {
           if(workThread != null) workThread.Abort();
       }

       /// <summary>
       /// Processing loop, captures an image and processes it
       /// </summary>
       private void Application_Idle()
       {
           while (true)
           {
               try
               {
                   Image<Bgr, byte> tempFrame = captured_frame = CameraManager.GetImage();
                   if (CaptureSettings.Instance().DEFAULT_DILATATION != 0) tempFrame._Dilate((int)CaptureSettings.Instance().DEFAULT_DILATATION);
                   if (CaptureSettings.Instance().DEFAULT_ERODE != 0) tempFrame._Erode((int)CaptureSettings.Instance().DEFAULT_ERODE);
                   if (CaptureSettings.Instance().DEFAULT_GAMMA != 1) tempFrame._GammaCorrect((double)CaptureSettings.Instance().DEFAULT_GAMMA);
                   if (CaptureSettings.Instance().DEFAULT_INVERT_ENABLED) tempFrame._Not();
                   if (CaptureSettings.Instance().DEFAULT_NORMALIZE_ENABLED) tempFrame._EqualizeHist();

                   // fix rotation
                   if (CalibrationSettings.Instance().CALIBRATION_ROTATION != 0)
                   {
                       tempFrame = tempFrame.Rotate(CalibrationSettings.Instance().CALIBRATION_ROTATION, new Bgr(0, 0, 0), true);
                   }

                   processor.ProcessImage(tempFrame); 
                   if (interactiveWindow != null && interactiveWindow.IsVisible) interactiveWindow.DrawImage(tempFrame, processor);
                   InputLogic();


               }
               catch {
                    // no-op here
               }
           }
       }



       /// <summary>
       /// Takes all detected contours, transforms their positions into the coordinate system of the table,
       /// taking also into accoutn perspective distortion. At last, it creates descriptors of detected
       /// stones and passes everything to the merging processor that will increase the simulation step
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

                       double mx = found_pos_x; // X-axis coord in the trapezoid
                       double my = captured_frame.Height - found_pos_y; // Y-axis coord in the trapezoid

                       double ax = CalibrationSettings.Instance().CALIBRATION_POINT_A.X; // X-axis coord of A-point in trapezoid

                       // estimation of other points according to ( a = (2*r1r2)/(r1+1))
                       double r1 = CalibrationSettings.Instance().CALIBRATION_PERSPECTIVE;
                       double r2 = CalibrationSettings.Instance().CALIBRATION_POINT_C.X - CalibrationSettings.Instance().CALIBRATION_POINT_A.X;
                       double bx = ax + (2 * (r1 * r2)) / (r1 + 1); // X-axis coord of B-point in trapezoid
                       double cx = CalibrationSettings.Instance().CALIBRATION_POINT_C.X; // X-axis coord of C-point in trapezoid
                       double dx = ax + bx - cx; // X-axis coord of D-point in trapezoid

                       double cy = captured_frame.Height - CalibrationSettings.Instance().CALIBRATION_POINT_C.Y; // Y-axis coord of C-point in trapezoid
                       double ay = captured_frame.Height - CalibrationSettings.Instance().CALIBRATION_POINT_A.Y; // Y-axis coord of A-point in trapezoid

                       double pom = (my - ay) / (cy - ay); // ratio between the bottom base and the upper base
                       double r1m = mx - (ax + (dx - ax) * pom); // distance of the M point from the left side
                       double r1r2 = pom * (cx - bx - dx + ax) + bx - ax; // length of a line that crosses the M-point and both sides

                       double ab = Math.Abs(bx - ax); // length of AB line
                       double cd = Math.Abs(cx - dx); // length of CD line
                       double alfa = r1r2 / ab; // perspective distortion
                       double maximum = captured_frame.Height / (cd / ab); // max distortion
                       double real = (captured_frame.Height) * (pom / alfa); // relative distortion to the M-point

                       // projection to a rectangle the frame represents
                       double mxx = (ax < bx) ? captured_frame.Width * (r1m / r1r2) : captured_frame.Width - captured_frame.Width * r1m / r1r2;
                       double mxy = (ay > cy) ? (captured_frame.Height * real / maximum) : captured_frame.Height - (captured_frame.Height * real / maximum);


                       // recalculation from camera space to table space
                       mxx *= CommonAttribService.ACTUAL_TABLE_WIDTH / ((double)captured_frame.Width);
                       mxy *= CommonAttribService.ACTUAL_TABLE_HEIGHT / ((double)captured_frame.Height);
                       foundRocks.Add(CreateFoundRock(found.template.name, new FPoint(mxx - CommonAttribService.ACTUAL_TABLE_WIDTH / 2, CommonAttribService.ACTUAL_TABLE_HEIGHT / 2 - mxy),processor.templates, found.rate, found.scale, found.angle));
                   }
                   catch
                   {
                       MessageBox.Show("An error occurred during calibration. Try to set the calibration again!");
                       StopThread();
                   }
                   #endregion

                  
               }
               // merge both systems between two time steps
               realTableManager.MergeSystems(foundRocks);
           }
       }

       /// <summary>
       /// Create a stone entity based on the information about detected contour
       /// </summary>
       /// <param name="contureName">contour name</param>
       /// <param name="position">contour position</param>
       /// <param name="templates">list of known templates</param>
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
