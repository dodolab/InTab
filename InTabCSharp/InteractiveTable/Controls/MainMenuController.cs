using System;
using InteractiveTable.GUI.CaptureSet;
using InteractiveTable.Managers;
using System.Windows.Forms;
using InteractiveTable.GUI.Table;
using InteractiveTable.GUI.Other;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using InteractiveTable.Core.Data.Deposit;

namespace InteractiveTable.Controls
{
    /// <summary>
    /// Controller for the main menu
    /// </summary>
    public class MainMenuController
    {
        #region vars, get-set, init

        // main window
        private MainWindow mainWindow;
        // window manager
        private WindowManager mainManager;
        // table manager
        private TableManager tableManager;

        public MainMenuController()
        {

        }
        
        public MainWindow MainWindow
        {
            get { return mainWindow; }
            set { this.mainWindow = value; }
        }

        public TableManager TableManager
        {
            get { return tableManager; }
            set { this.tableManager = value; }
        }

        public WindowManager MainManager
        {
            get { return mainManager; }
            set { this.mainManager = value; }
        }

        /// <summary>
        /// Sets handlers for the main menu
        /// </summary>
        public void SetHandlers()
        {
            mainWindow.mainMenu.contourSubItem.Click += new System.Windows.RoutedEventHandler(Menu_contourSubItem_Click);
            mainWindow.mainMenu.openGLItem.Click += new System.Windows.RoutedEventHandler(OpenGLItem_Click);
            mainWindow.mainMenu.userSettingsItem.Click += new System.Windows.RoutedEventHandler(UserSettingsItem_Click);
            mainWindow.mainMenu.tableCalibrationItem.Click += new System.Windows.RoutedEventHandler(tableCalibrationItem_Click);
            mainWindow.mainMenu.systemLoad.Click += new System.Windows.RoutedEventHandler(systemLoad_Click);
            mainWindow.mainMenu.systemSave.Click += new System.Windows.RoutedEventHandler(systemSave_Click);
            mainWindow.mainMenu.help.Click += new System.Windows.RoutedEventHandler(help_Click);
            mainWindow.mainMenu.endApp.Click += new System.Windows.RoutedEventHandler(endApp_Click);
            mainWindow.Closed += new EventHandler(mainWindow_Closed);
        }


        #endregion

        #region handlers

        /// <summary>
        /// Ends the application
        /// </summary>
        private void endApp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonAttribService.mainWindow.Close();
        }

        /// <summary>
        /// Displays a help file
        /// </summary>
        private void help_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("Help.chm");
            }
            catch
            {
                MessageBox.Show("I cannot find a help file!");
            }
        }

        /// <summary>
        /// Saves the whole application state into file
        /// </summary>
        private void systemSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Systems(*.sst)|*.sst";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = sfd.FileName;
                try
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        new BinaryFormatter().Serialize(fs, tableManager.TableDepositor);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Loads a whole application state from a file
        /// </summary>
        private void systemLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Warning: loaded system is not accessible via from the simulator!");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Systems(*.sst)|*.sst";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = ofd.FileName;
                try
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                        tableManager.TableDepositor = (TableDepositor)new BinaryFormatter().Deserialize(fs);

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

   
        /// <summary>
        /// Opens a window for calibration of the table
        /// </summary>
        private void tableCalibrationItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TableCalibrationWindow tableCl = new TableCalibrationWindow();
            tableCl.Show();
            tableCl.LoadValues();
        }

        /// <summary>
        /// Opens a window with user settings
        /// </summary>
        private void UserSettingsItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SettingsWindow setWin = new SettingsWindow();
            setWin.Owner = CommonAttribService.mainWindow;
            setWin.LoadValues();
            setWin.ShowDialog(); 
        }

        /// <summary>
        /// Opens a window for contour settings and capturing
        /// </summary>
        private void Menu_contourSubItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CaptureWindow capture = new CaptureWindow();
            CaptureSetController capture_ctrl = new CaptureSetController();
            CaptureSetManager capture_mng = new CaptureSetManager();
            capture_ctrl.CaptureWindow = capture;
            capture_ctrl.CaptureManager = capture_mng;
            capture_mng.CaptureSetWindow = capture;
            capture_mng.CaptureSetControl = capture_ctrl;
            capture_ctrl.SetDefaultValues();
            capture_ctrl.SetHandlers();
            capture_mng.Initialize(); // initialize a thread for camera capture
            if (CommonAttribService.DEFAULT_TEMPLATES != null) capture.Processor.templates = CommonAttribService.DEFAULT_TEMPLATES;
            capture.Show();
        }

        /// <summary>
        /// Opens an output window
        /// </summary>
        private void OpenGLItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonAttribService.MODE_2D = false;
            TableOutput table3D = new TableOutput();
            tableManager.Table3D = table3D;

            // change the mode back to 2D after closing this window
            table3D.Closed += new EventHandler(delegate(object sender2, EventArgs e2)
            {
                CommonAttribService.MODE_2D = true;
            });
            table3D.Show();
        }
        
        /// <summary>
        /// Close the application upon closing the main window
        /// </summary>
        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion
    }
}
