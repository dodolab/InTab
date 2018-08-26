using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTable.Controls;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.Capture;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using InteractiveTable.GUI.Other;
using System.Xml.Serialization;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manager of all windows
    /// </summary>
    public class WindowManager
    {
        private MainWindow myWindow;
        private TableManager tableManager;

        public WindowManager()
        {

        }

        public MainWindow MyWindow
        {
            get { return MyWindow; }
            set { this.myWindow = value; }
        }

        public TableManager TableManager
        {
            get { return tableManager; }
        }

        /// <summary>
        /// Initializes all MVCs
        /// </summary>
        public void ManageMainWindow()
        {

            // main window
            MainMenuController ctrl = new MainMenuController(); 
            ctrl.MainWindow = myWindow;
            ctrl.MainManager = this;
            ctrl.SetHandlers();

            // table window
            TableController ctrl2 = new TableController();
            TableManager tableMgr = new TableManager();
            this.tableManager = tableMgr;
            ctrl2.TablePanel = myWindow.tablePanel;
            myWindow.tablePanel.tableAreaPanel.TableController = ctrl2;
            ctrl2.TableManager = tableMgr;
            ctrl2.SetHandlers();

            // table settings
            TableSettingsController ctrl3 = new TableSettingsController();
            ctrl3.TableSetPanel = myWindow.tablePanel.tableSettingsPanel;
            ctrl3.TableManager = tableMgr;
            ctrl3.SetHandlers();

            ctrl.TableManager = tableMgr;
            tableMgr.TablePanel = myWindow.tablePanel;
        }

        /// <summary>
        /// Loads all default values
        /// </summary>
        public void LoadDefaultValues()
        {
            LoadTemplates();
            LoadFadeColors();
            LoadUserSettings(); 
        }

        /// <summary>
        /// Loads user settings from config file
        /// </summary>
        private void LoadUserSettings()
        {
            // this is here from historical reasons
            CommonAttribService.ACTUAL_OUTPUT_WIDTH = Properties.Settings.Default.DEFAULT_OUTPUT_DEPENDENT_WIDTH;
        }
        
        private void LoadTemplates()
        {
            if (CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH == "")
            {
                System.Windows.MessageBox.Show("No template path configured!");
            }
            else
            {
                string fileName = CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH;
                try
                {
                    if (!File.Exists(fileName))
                    {
                        System.Windows.MessageBox.Show("There is no file with templates for stones! Create one in contour settings!");
                    }
                    else using (FileStream fs = new FileStream(fileName, FileMode.Open))
                            CommonAttribService.DEFAULT_TEMPLATES = (Templates)new BinaryFormatter().Deserialize(fs);
                }
                catch
                {
                    System.Windows.MessageBox.Show("An error ocurred during template initialization");
                }

            }
        }

        /// <summary>
        /// Loads gradient colors
        /// </summary>
        private void LoadFadeColors()
        {
            // fade color path
            if (CaptureSettings.Instance().DEFAULT_FADECOLOR_PATH == "")
            {
                CaptureSettings.Instance().DEFAULT_FADECOLOR_PATH = "colors.xml";
            }

            string fadeName = CaptureSettings.Instance().DEFAULT_FADECOLOR_PATH;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashSet<FadeColor>), new Type[] { typeof(FadeColor) });
                StreamReader reader = new StreamReader(fadeName);

                // we need to order the colors as they are not serialized in order
                LinkedList<FadeColor> output = new LinkedList<FadeColor>();
                HashSet<FadeColor> temp = (HashSet<FadeColor>)serializer.Deserialize(reader);
                foreach (FadeColor fade in temp.OrderBy(fadec => fadec.position)) output.AddLast(fade);
                CommonAttribService.DEFAULT_FADE_COLORS = output;
                reader.Close();
            }
            catch
            {
                System.Windows.MessageBox.Show("An error ocurred during loading of color gradients");
            }
        }
    }
}
