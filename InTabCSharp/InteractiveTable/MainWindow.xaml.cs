using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InteractiveTable.GUI.CaptureSet;
using InteractiveTable.Controls;
using InteractiveTable.GUI.Other;
using InteractiveTable.Managers;
using InteractiveTable.Settings;

namespace InteractiveTable
{
    /// <summary>
    /// Main window logic
    /// </summary>
    public partial class MainWindow : Window
    {
        // nacitaci miniokenko
        private Loading loadingWindow;
        private System.Windows.Forms.Timer loadingTimer;
        private WindowManager myManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        public WindowManager MyManager
        {
            get { return myManager; }
        }


        /// <summary>
        /// Main window init - displays the loading window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            loadingWindow = new Loading();
            loadingWindow.Show();
            loadingWindow.Focus();
            loadingTimer = new System.Windows.Forms.Timer();
            loadingTimer.Tick += new EventHandler(loadingTimer_Tick);
            loadingTimer.Interval = 2000;
            loadingTimer.Start();
           
           Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
        }

        /// <summary>
        /// Unhandled exception handler for the dispatcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.Write(e.Exception.StackTrace);
        }

        /// <summary>
        /// Unhandled exception handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.Write(e.ExceptionObject.ToString());
        }

        /// <summary>
        /// Loading timer tick, disables loading window after ~3 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            if (this.IsLoaded)
            {
                loadingWindow.Close();
            }
            IsEnabled = true;
            loadingTimer.Stop();
        }

        /// <summary>
        /// Once the main window has been initialized, initialize the manager that will
        /// take care of the rest of the init process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // if there is no loading timer, cancel the loading window
            if (!loadingTimer.Enabled)
            {
                loadingWindow.Close();
            }

            // Load all settings from XML
            CalibrationSettings.Instance().Load();
            CaptureSettings.Instance().Load();
            GraphicsSettings.Instance().Load();
            PhysicSettings.Instance().Load();


            CommonAttribService.mainWindow = this;

            // connect the main manager with all other models
            myManager = new WindowManager();
            myManager.MyWindow = this;
            myManager.LoadDefaultValues();
            myManager.ManageMainWindow();

            CameraManager.Reinitialize();
        }

        /// <summary>
        /// Window size change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tablePanel.Width = tablePanel.Width + e.NewSize.Width - e.PreviousSize.Width;
            tablePanel.Height = tablePanel.Height + e.NewSize.Height - e.PreviousSize.Height;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CalibrationSettings.Instance().Save();
            CaptureSettings.Instance().Save();
            GraphicsSettings.Instance().Save();
            PhysicSettings.Instance().Save();
        }


    }
}
