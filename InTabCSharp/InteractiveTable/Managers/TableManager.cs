using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.GUI.Table;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Settings;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Core.ClientServer;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manazer pro delegaci vykonne logiky simulacniho okna
    /// </summary>
   public class TableManager
    {
       private TablePanel tablePanel;
       private TableOutput table3D;
       private System.Windows.Forms.Timer threadTimer; // casovac pro hlavni vykonne vlakno
       private TableDepositor tableDepositor;
       private InputManager inputManager;
       private Boolean timer_running = true; // indikator, zda vlakno bude bezet nebo ne
       private DateTime last_render = DateTime.Now; // cas posledniho renderu
       private ServerSender serverSender = new ServerSender();

       public TableManager()
       {
           this.tableDepositor = new TableDepositor();

           inputManager = new InputManager(this);
       }

       public InputManager InputManager
       {
           get { return inputManager; }
           set { this.inputManager = value; }
       }

       public TableDepositor TableDepositor
       {
           get { return tableDepositor; }
           set { tableDepositor = value; }
       }

       public TablePanel TablePanel
       {
           get { return tablePanel; }
           set { this.tablePanel = value; }
       }

       public TableOutput Table3D
       {
           get { return table3D; }
           set { this.table3D = value; }
       }


       /// <summary>
       /// Spusti hlavni vlakno, kter bude kontrolovat
       /// vstup z kamery, prekreslovat herni plochu a volat
       /// fyzikalni logiku
       /// </summary>
       public void RunThread()
       {
           if (!timer_running)
           {
               timer_running = true;
           }
           else
           {
               if (threadTimer == null)
               {
                   threadTimer = new System.Windows.Forms.Timer();
                   threadTimer.Interval = 7;
                   threadTimer.Tick += new EventHandler(Render);
                   threadTimer.Start();
               }
           }
       }


       /// <summary>
       /// Pozastavi vlakno
       /// </summary>
       public void PauseThread()
       {
           timer_running = false;
       }

      /// <summary>
      /// Zastavi vlakno a restartuje soustavu
      /// </summary>
       public void StopThread()
       {
           if (threadTimer != null) threadTimer.Stop();
           threadTimer = null;
           tableDepositor.ClearTable();

           HashSet<System.Windows.Controls.Image> image_to_delete = new HashSet<System.Windows.Controls.Image>();
           foreach (System.Windows.UIElement uie in tablePanel.mainGrid.Children)
           {
               if (uie is System.Windows.Controls.Image) image_to_delete.Add((System.Windows.Controls.Image)uie);
           }
           foreach (System.Windows.Controls.Image img in image_to_delete) tablePanel.mainGrid.Children.Remove(img);

       }

       /// <summary>
       /// Restartuje celou soustavu a znovu spusti
       /// </summary>
       public void Restart()
       {
           StopThread();
           timer_running = true;
           RunThread();
       }

       /// <summary>
       /// Vrati true, pokud vlakno bezi
       /// </summary>
       public Boolean IsRunning
       {
           get { return timer_running; }
       }

       // indikator kvuli tomu, aby se obrazek posilal jen kdyz doslo ke zmene
       private bool sentImage = false;

       private DateTime lastSendTime = DateTime.Now;



       /// <summary>
       /// Transformuje soustavu a preda ji k vykresleni
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private void Render(object sender, EventArgs e)
       {
           if (timer_running)
           {
               if ((DateTime.Now - last_render).TotalMilliseconds < 35)
               {
                   // to kvuli tomu, aby se to vykreslovalo max jednou za 35ms
                   return;
               }
               last_render = DateTime.Now;

               // transformace fyzikalniho systemu
               PhysicsLogic.TransformSystem(tableDepositor);
                   // poslani dat na klienta (pokud je zapnuty vystup do serveru)
               if (CommonAttribService.OUTPUT_SERVER_ALLOWED)
               {
                   DateTime now = DateTime.Now;
                   // kontrola intervalu
                   if ((now - lastSendTime).TotalMilliseconds > CaptureSettings.Instance().SEND_INTERVAL)
                   {
                       
                       if (!CaptureSettings.Instance().SEND_IMAGES)
                       {
                           Console.WriteLine("POSILAM KAMENY");
                           serverSender.SendRocks(RockList.createRockList(TableDepositor.GetAllRocks(), CommonAttribService.ACTUAL_TABLE_WIDTH, CommonAttribService.ACTUAL_TABLE_HEIGHT));
                           lastSendTime = now;
                       }
                       else
                       {
                           try
                           {
                               if (!CaptureSettings.Instance().MOTION_DETECTION || 
                                   (!sentImage && CameraManager.isInPeace && CameraManager.peaceCounter > 35))
                               {
                                   Console.WriteLine("POSILAM OBRAZEK");
                                   sentImage = true;
                                   serverSender.SendRocks(RockList.createRockImage(CameraManager.GetImage(),
                                   CalibrationSettings.Instance().CALIBRATION_LETTER_A,
                                   CalibrationSettings.Instance().CALIBRATION_LETTER_C,
                                   CalibrationSettings.Instance().CALIBRATION_LETTER_ROTATION));
                                   lastSendTime = now;
                               }
                               else
                               {
                                   if (!CameraManager.isInPeace && CameraManager.unpeaceCounter > 3)
                                   {
                                       sentImage = false;
                                   }
                               }
                           }
                           catch
                           {
                           }
                       }
                   }
               }

               // prekresleni obrazku
               tablePanel.tableAreaPanel.Repaint(tableDepositor);
               // pripadne prekresleni 3D vystupu
               if (table3D != null && table3D.IsVisible)
               {
                   table3D.Repaint(tableDepositor);
               }
           }
       }

       

       /// <summary>
       /// Vlozi novy objekt do soustavy
       /// </summary>
       /// <param name="obj"></param>
       public void InsertObject(A_TableObject obj){
           if (obj is Particle) tableDepositor.particles.Add((Particle)obj); // castici sem touhle cestou davat nikdy nebudeme
           if (obj is Graviton) tableDepositor.gravitons.Add((Graviton)obj);
           if (obj is Generator) tableDepositor.generators.Add((Generator)obj);
           if (obj is Magneton) tableDepositor.magnetons.Add((Magneton)obj);
           if (obj is BlackHole) tableDepositor.blackHoles.Add((BlackHole)obj);
       }

       /// <summary>
       /// Odstrani stary objekt ze soustavy
       /// </summary>
       /// <param name="obj"></param>
       public void RemoveObject(A_TableObject obj)
       {
           if (obj is Particle && tableDepositor.particles.Contains(obj)) tableDepositor.particles.Remove((Particle)obj);
           if (obj is Graviton && tableDepositor.gravitons.Contains(obj)) tableDepositor.gravitons.Remove((Graviton)obj);
           if (obj is Generator && tableDepositor.generators.Contains(obj)) tableDepositor.generators.Remove((Generator)obj);
           if (obj is Magneton && tableDepositor.magnetons.Contains(obj)) tableDepositor.magnetons.Remove((Magneton)obj);
           if (obj is BlackHole && tableDepositor.blackHoles.Contains(obj)) tableDepositor.blackHoles.Remove((BlackHole)obj);
       }
    }

 
}
