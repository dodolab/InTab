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
    /// Logika hlavniho okna
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
        /// Inicializace hlavniho okna - zobrazi pouze loading window
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
        /// Pokud dojde k nezname chybe, neprovede se nic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
          // sem se dostane vlakno pote, co bude vyhozena vyjimka do nejvyssiho predka
        }

        /// <summary>
        /// Zpracovani neodchytnutych vyjimek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // sem se dostane vyjimka, kterou nikdo neodchytnul
        }

        /// <summary>
        /// Loading timer tick, po 3 vterinach vypne loading okno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            // vypni loading window pouze pokud uz je hlavni okno nacteno,
            // v kazdem pripade vypni loading timer
            if(this.IsLoaded) loadingWindow.Close();
            IsEnabled = true;
            loadingTimer.Stop();
        }

        /// <summary>
        /// Po nacteni hlavniho okna se inicializuje hlavni manazer, ktery se postara
        /// o zbytek inicializace; take se inicializuje manazer kamery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // pokud loading timer uz nebezi, zrus loading okno
            if (!loadingTimer.Enabled)
            {
                loadingWindow.Close();
            }

            // nacteni veskereho nastaveni z XML souboru
            CalibrationSettings.Instance().Load();
            CaptureSettings.Instance().Load();
            GraphicsSettings.Instance().Load();
            PhysicSettings.Instance().Load();


            CommonAttribService.mainWindow = this;

            // vytvoreni manazeru, ktery propoji funkcionalitu VSEM hlavnim modelum
            myManager = new WindowManager();
            myManager.MyWindow = this;
            myManager.LoadDefaultValues();
            myManager.ManageMainWindow();

            CameraManager.Reinitialize();
        }

        /// <summary>
        /// Zmena velikosti okna
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
