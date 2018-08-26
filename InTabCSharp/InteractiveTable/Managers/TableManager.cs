using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTable.GUI.Table;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Core.Physics.System;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manager that delegates logic of rendering window
    /// </summary>
   public class TableManager
    {
       private TablePanel tablePanel;
       private TableOutput table3D;
       private System.Windows.Forms.Timer threadTimer; // timer for main window
       private TableDepositor tableDepositor;
       private InputManager inputManager;
       private Boolean timer_running = true; 
       private DateTime last_render = DateTime.Now; // time of the last render

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
       /// Executes the main thread that will check for captured image
       /// and refresh the table
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

        
       public void PauseThread()
       {
           timer_running = false;
       }
        
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
       /// Resets the whole system
       /// </summary>
       public void Restart()
       {
           StopThread();
           timer_running = true;
           RunThread();
       }
        
       public Boolean IsRunning
       {
           get { return timer_running; }
       }
        
       // a helper flag used for sending a picture only once if a change occured
       private bool sentImage = false;

       private DateTime lastSendTime = DateTime.Now;



       /// <summary>
       /// Refreshes the table
       /// </summary>
       private void Render(object sender, EventArgs e)
       {
           if (timer_running)
           {
               if ((DateTime.Now - last_render).TotalMilliseconds < 20)
               {
                   // only once per 20ms 
                   return;
               }
               last_render = DateTime.Now;
                
               // apply physics
               PhysicsLogic.TransformSystem(tableDepositor);
                
               // refresh the image
               tablePanel.tableAreaPanel.Repaint(tableDepositor);
               // refresh opengGL window
               if (table3D != null && table3D.IsVisible)
               {
                   table3D.Repaint(tableDepositor);
               }
           }
       }
        
       /// <summary>
       /// Inserts a new object into the system
       /// </summary>
       /// <param name="obj"></param>
       public void InsertObject(A_TableObject obj){
           if (obj is Particle) tableDepositor.particles.Add((Particle)obj); // this will likely not happen
           if (obj is Graviton) tableDepositor.gravitons.Add((Graviton)obj);
           if (obj is Generator) tableDepositor.generators.Add((Generator)obj);
           if (obj is Magneton) tableDepositor.magnetons.Add((Magneton)obj);
           if (obj is BlackHole) tableDepositor.blackHoles.Add((BlackHole)obj);
       }

       /// <summary>
       /// Removes an existing object from the system
       /// </summary>
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
