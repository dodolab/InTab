using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Controls;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.Capture;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Windows;
using InteractiveTable.GUI.Other;
using System.Xml.Serialization;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Manazer vsech oken
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
        /// Inicializuje VSECHNY model-view-managery a controllery
        /// </summary>
        public void ManageMainWindow()
        {

            // nastaveni kontroleru hlavniho okna
            MainMenuController ctrl = new MainMenuController(); 
            ctrl.MainWindow = myWindow;
            ctrl.MainManager = this;
            ctrl.SetHandlers();

            // nastaveni kontroleru a manazeru tabulky
            TableController ctrl2 = new TableController();
            TableManager tableMgr = new TableManager();
            this.tableManager = tableMgr;
            ctrl2.TablePanel = myWindow.tablePanel;
            myWindow.tablePanel.tableAreaPanel.TableController = ctrl2;
            ctrl2.TableManager = tableMgr;
            ctrl2.SetHandlers();

            // nastaveni kontroleru nastaveni tabulky
            TableSettingsController ctrl3 = new TableSettingsController();
            ctrl3.TableSetPanel = myWindow.tablePanel.tableSettingsPanel;
            ctrl3.TableManager = tableMgr;
            ctrl3.SetHandlers();

            ctrl.TableManager = tableMgr;
            tableMgr.TablePanel = myWindow.tablePanel;
        }

        /// <summary>
        /// Nacte veskere defaultni hodnoty ulozene v konfiguracnim souboru vcetne serializovanych objektu
        /// </summary>
        public void LoadDefaultValues()
        {
            LoadTemplates();
            LoadFadeColors();
            LoadUserSettings(); 
        }

        /// <summary>
        /// Nacteni uzivatelskeho nastaveni z konfiguracniho souboru
        /// </summary>
        private void LoadUserSettings()
        {
            // tohle je tady jen s historickych duvodu
            CommonAttribService.ACTUAL_OUTPUT_WIDTH = Properties.Settings.Default.DEFAULT_OUTPUT_DEPENDENT_WIDTH;
        }

        /// <summary>
        /// Nacteni sablon
        /// </summary>
        private void LoadTemplates()
        {
            if (CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH == "")
            {
                System.Windows.MessageBox.Show("Není nastavena cesta k šablonám. Než budete sestavovat stůl, je třeba vytvořit kontury viz nápověda");
            }
            else
            {
                string fileName = CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH;
                try
                {
                    if (!File.Exists(fileName))
                    {
                        System.Windows.MessageBox.Show("Není vytvořen soubor se šablonami kamenů!");
                    }
                    else using (FileStream fs = new FileStream(fileName, FileMode.Open))
                            CommonAttribService.DEFAULT_TEMPLATES = (Templates)new BinaryFormatter().Deserialize(fs);
                }
                catch
                {
                    System.Windows.MessageBox.Show("Došlo k chybě při pokusu o načtení templates");
                }

            }
        }

        /// <summary>
        /// Nacteni prechodovych barev pro vykreslovani barev u castic
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

                // teď to musime seradit, protoze v XML mame HashSet (linkedlist se totiz neda serializovat)
                LinkedList<FadeColor> output = new LinkedList<FadeColor>();
                HashSet<FadeColor> temp = (HashSet<FadeColor>)serializer.Deserialize(reader);
                foreach (FadeColor fade in temp.OrderBy(fadec => fadec.position)) output.AddLast(fade);
                CommonAttribService.DEFAULT_FADE_COLORS = output;
                reader.Close();
            }
            catch
            {
                System.Windows.MessageBox.Show("Došlo k chybě při pokusu o načtení barevného nastavení");
            }
        }

    }
}
