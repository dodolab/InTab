using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.GUI.CaptureSet;
using InteractiveTable.Managers;
using System.Windows.Forms;
using InteractiveTable.GUI.Table;
using InteractiveTable.Settings;
using InteractiveTable.GUI.Other;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using InteractiveTable.Core.Data.Deposit;

namespace InteractiveTable.Controls
{
    /// <summary>
    /// Controller pro hlavni menu 
    /// </summary>
    public class MainMenuController
    {
        #region vars, get-set, init

        // odkaz na hlavni okno
        private MainWindow mainWindow;
        // odkaz na manazera oken
        private WindowManager mainManager;
        // odkaz na manazera stolu
        private TableManager tableManager;

        public MainMenuController()
        {

        }

        /// <summary>
        /// Vrati nebo nastavi odkaz na hlavni okno
        /// </summary>
        public MainWindow MainWindow
        {
            get { return mainWindow; }
            set { this.mainWindow = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi odkaz na manazera stolu
        /// </summary>
        public TableManager TableManager
        {
            get { return tableManager; }
            set { this.tableManager = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi odkaz na manazera oken
        /// </summary>
        public WindowManager MainManager
        {
            get { return mainManager; }
            set { this.mainManager = value; }
        }

        /// <summary>
        /// Nastavi handlery pro hlavni menu
        /// </summary>
        public void SetHandlers()
        {
            mainWindow.mainMenu.contourSubItem.Click += new System.Windows.RoutedEventHandler(Menu_contourSubItem_Click);
            mainWindow.mainMenu.openGLItem.Click += new System.Windows.RoutedEventHandler(OpenGLItem_Click);
            mainWindow.mainMenu.userSettingsItem.Click += new System.Windows.RoutedEventHandler(UserSettingsItem_Click);
            mainWindow.mainMenu.tableCalibrationItem.Click += new System.Windows.RoutedEventHandler(tableCalibrationItem_Click);
            mainWindow.mainMenu.letterCalibrationItem.Click += new System.Windows.RoutedEventHandler(letterCalibrationItem_Click);
            mainWindow.mainMenu.systemLoad.Click += new System.Windows.RoutedEventHandler(systemLoad_Click);
            mainWindow.mainMenu.systemSave.Click += new System.Windows.RoutedEventHandler(systemSave_Click);
            mainWindow.mainMenu.help.Click += new System.Windows.RoutedEventHandler(help_Click);
            mainWindow.mainMenu.endApp.Click += new System.Windows.RoutedEventHandler(endApp_Click);
            mainWindow.Closed += new EventHandler(mainWindow_Closed);
        }


        #endregion

        #region handlers

        /// <summary>
        /// Ukonceni aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endApp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonAttribService.mainWindow.Close();
        }

        /// <summary>
        /// Kliknuti na napovedu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void help_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("Help.chm");
            }
            catch
            {
                MessageBox.Show("Nemohu nalézt soubor s nápovědou!");
            }
        }

        /// <summary>
        /// Kliknuti na tlacitko ABOUT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void about_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           // TODO !!!!
        }

        /// <summary>
        /// Ulozeni cele soustavy do souboru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Nacteni cele soustavy ze souboru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void systemLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Upozornění: Načtená soustava není přístupná přes simulátor!");
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
        /// Kliknuti na kalibraci stolu otevre okno pro kalibraci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableCalibrationItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
                TableCalibrationWindow tableCl = new TableCalibrationWindow();
               // tableCl.Owner = CommonAttribService.mainWindow;
                tableCl.Show();
                tableCl.LoadValues();
        }

        /// <summary>
        /// Kliknuti na kalibraci stolu pro dopis v Intab2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void letterCalibrationItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LetterCalibrationWindow tableCl = new LetterCalibrationWindow();
            tableCl.Show();
            tableCl.LoadValues();
        }

        /// <summary>
        /// Kliknuti na uzivatelske nastaveni otevre okno s nastavenim
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserSettingsItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SettingsWindow setWin = new SettingsWindow();
            setWin.Owner = CommonAttribService.mainWindow;
            setWin.LoadValues();
            setWin.ShowDialog(); 
        }

        /// <summary>
        /// Otevreni okna pro nastaveni kontur a zachytavani
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_contourSubItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            // nastaveni MVC pointeru + otevreni okna
            CaptureWindow capture = new CaptureWindow();
            CaptureSetController capture_ctrl = new CaptureSetController();
            CaptureSetManager capture_mng = new CaptureSetManager();
            capture_ctrl.CaptureWindow = capture;
            capture_ctrl.CaptureManager = capture_mng;
            capture_mng.CaptureSetWindow = capture;
            capture_mng.CaptureSetControl = capture_ctrl;
            capture_ctrl.SetDefaultValues();
            capture_ctrl.SetHandlers();
            capture_mng.Initialize(); // inicializace vlakna pro odchytavani kamery
            if (CommonAttribService.DEFAULT_TEMPLATES != null) capture.Processor.templates = CommonAttribService.DEFAULT_TEMPLATES;
            capture.Show();
        }

        /// <summary>
        /// Zobrazeni vystupniho okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGLItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonAttribService.MODE_2D = false;
            TableOutput table3D = new TableOutput();
            tableManager.Table3D = table3D;

            // pri zruseni table3D okna se opet prepneme do modu vykreslovani na klasickou plochu
            table3D.Closed += new EventHandler(delegate(object sender2, EventArgs e2)
            {
                CommonAttribService.MODE_2D = true;
            });
            table3D.Show();
        }



        /// <summary>
        /// Pri zruseni hlavniho okna dojde k ukonceni cele aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Application.Exit();
        }


        #endregion
    }
}
