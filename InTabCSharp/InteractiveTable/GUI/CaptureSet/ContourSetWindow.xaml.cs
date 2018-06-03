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
using System.Windows.Shapes;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using InteractiveTable.Settings;

namespace InteractiveTable.GUI.CaptureSet
{
    /// <summary>
    /// MVC pro prizazeni kamenu jednotlivym konturam vcetne nastaveni
    /// </summary>
    public partial class ContourSetWindow : Window
    {
        #region promenne

        private Templates templates; // sablony
        private HashSet<ContourRock> contourRocks; // kamen + identifikator kontury
        private Boolean initialized = false; // zda bylo okno inicializovano
        private ContourRock actual_contour; // aktualni kamen, ktery je nastavovan

        #endregion

        #region konstruktory, gettery a settery

        /// <summary>
        /// Vytvori nove okno a inicializuje handlery vsem objektum
        /// </summary>
        public ContourSetWindow()
        { 
            InitializeComponent();
            rockTypeCombo.SelectionChanged += new SelectionChangedEventHandler(rockTypeCombo_SelectionChanged);
            contourNameCombo.SelectionChanged += new SelectionChangedEventHandler(contourNameCombo_SelectionChanged);

            generatorGroup.Visibility = Visibility.Collapsed;
            magnetonGroup.Visibility = Visibility.Collapsed;
            blackHoleGroup.Visibility = Visibility.Collapsed;
            gravitonGroup.Visibility = Visibility.Collapsed;

            BHweighSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GangleMaxSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GangleOffSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GgenerSpeedSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GmaxPartVelocitySlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GminPartVelocitySlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GravWeighSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            MagForceSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GminPartSizeSlider.ValueChanged+=new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            GmaxPartSizeSlider.ValueChanged+=new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
        }

        /// <summary>
        /// Vrati nebo nastavi sablony
        /// </summary>
        public Templates Templates
        {
            get { return templates; }
            set { this.templates = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi propojeny seznam kontura-kamen
        /// </summary>
        public HashSet<ContourRock> ContourRocks
        {
            get { return contourRocks; }
        }

        #endregion

        #region view handlers

        /// <summary>
        /// Zmena hodnoty slideru pouze zmeni text pod nim
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender == BHweighSlider) BHweighTb.Text = BHweighSlider.Value.ToString("#.##");
            if (sender == GangleMaxSlider) GangleMaxTb.Text = GangleMaxSlider.Value.ToString("#.##");
            if (sender == GangleOffSlider) GangleOffTb.Text = GangleOffSlider.Value.ToString("#.##");
            if (sender == GgenerSpeedSlider) GgenerSpeedTb.Text = GgenerSpeedSlider.Value.ToString("#.##");
            if (sender == GravWeighSlider) GravWeighTb.Text = GravWeighSlider.Value.ToString("#.##");
            if (sender == MagForceSlider) MagForceTb.Text = MagForceSlider.Value.ToString("#.##");

            if (sender == GminPartSizeSlider)
            {
                GminPartSizeTb.Text = GminPartSizeSlider.Value.ToString("#.##");
                if (GminPartSizeSlider.Value > GmaxPartSizeSlider.Value) GminPartSizeSlider.Value = GmaxPartSizeSlider.Value;
            }

            if (sender == GmaxPartSizeSlider)
            {
                GmaxPartSizeTb.Text = GmaxPartSizeSlider.Value.ToString("#.##");
                if (GminPartSizeSlider.Value > GmaxPartSizeSlider.Value) GmaxPartSizeSlider.Value = GminPartSizeSlider.Value;
            }

            if (sender == GmaxPartVelocitySlider)
            {
                GmaxPartVelocityTb.Text = GmaxPartVelocitySlider.Value.ToString("#.##");
                if (GminPartVelocitySlider.Value > GmaxPartVelocitySlider.Value) GmaxPartVelocitySlider.Value = GminPartVelocitySlider.Value;
            }
            if (sender == GminPartVelocitySlider)
            {
                GminPartVelocityTb.Text = GminPartVelocitySlider.Value.ToString("#.##");
                if (GminPartVelocitySlider.Value > GmaxPartVelocitySlider.Value) GminPartVelocitySlider.Value = GmaxPartVelocitySlider.Value;
            }
        }

        /// <summary>
        /// Zmena hodnoty ve vyberu typu kamene pouze zviditelni prislusne groupboxy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rockTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialized)
            {
                generatorGroup.Visibility = Visibility.Collapsed;
                magnetonGroup.Visibility = Visibility.Collapsed;
                blackHoleGroup.Visibility = Visibility.Collapsed;
                gravitonGroup.Visibility = Visibility.Collapsed;

                if (blackHoleCmbIt.IsSelected)
                {
                    blackHoleGroup.Visibility = Visibility.Visible;
                }
                if (gravitonCmbIt.IsSelected)
                {
                    gravitonGroup.Visibility = Visibility.Visible;
                }
                if (generatorCmbIt.IsSelected)
                {
                    generatorGroup.Visibility = Visibility.Visible;
                }
                if (magnetonCmbIt.IsSelected)
                {
                    magnetonGroup.Visibility = Visibility.Visible;
                }
                RestartValues();
            }
        }


        /// <summary>
        /// Vynuluje veskere hodnoty
        /// </summary>
        public void RestartComponents()
        {
            GminPartSizeSlider.Value = 0;
            GmaxPartSizeSlider.Value = 0;
            BHweighSlider.Value = 0;
            GangleMaxSlider.Value = 0;
            GangleOffSlider.Value = 0;
            GgenerSpeedSlider.Value = 0;
            GmaxPartVelocitySlider.Value = 0;
            GminPartVelocitySlider.Value = 0;
            GravWeighSlider.Value = 0;
            MagForceSlider.Value = 0;
            pulsarChck.IsChecked = false;
            baseSetChck.IsChecked = false;
        }

        #endregion

        #region combobox handlery

        /// <summary>
        /// Zmena hodnoty ve vyberu kontury, ulozi stavajici hodnoty, vynuluje slidery a nacte soubory, pokud predtim byly nastaveny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contourNameCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialized)
            {
                SaveValues();
                RestartComponents();
                actual_contour = contourRocks.Where(ctr => ctr.contour_name.Equals(((ComboBoxItem)contourNameCombo.SelectedItem).Content)).First();
                LoadValues();
            }
        }


        #endregion

        #region ostatni handlery

        /// <summary>
        /// Kliknuti na tlacitko OK ulozi nastaveni a zavre okno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Chcete uložit změny?", "Změna nastavení", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                SaveValues();
                templates.rockSettings = contourRocks;

                // pokusime se ulozit soubor; pokud to nepujde, zobrazi se filedialog
                try
                {
                    string fileName = CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH;

                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        new BinaryFormatter().Serialize(fs, templates);
                }
                catch
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Templates(*.bin)|*.bin";
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string fileName = sfd.FileName;
                        try
                        {
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                                new BinaryFormatter().Serialize(fs, templates);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            this.Close();
        }

        /// <summary>
        /// Nacte defaultni nastaveni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadDefButton_Click(object sender, RoutedEventArgs e)
        {
            GminPartSizeSlider.Value = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
            GmaxPartSizeSlider.Value = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
            BHweighSlider.Value = PhysicSettings.Instance().DEFAULT_BLACKHOLE_WEIGH;
            GangleMaxSlider.Value = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_MAX;
            GangleOffSlider.Value = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_OFFSET;
            GgenerSpeedSlider.Value = PhysicSettings.Instance().DEFAULT_GENERATING_SPEED;
            GmaxPartVelocitySlider.Value = PhysicSettings.Instance().DEFAULT_MAX_GENERATING_VELOCITY;
            GminPartVelocitySlider.Value = PhysicSettings.Instance().DEFAULT_MIN_GENERATING_VELOCITY;
            GravWeighSlider.Value = PhysicSettings.Instance().DEFAULT_GRAVITON_WEIGH;
            MagForceSlider.Value = PhysicSettings.Instance().DEFAULT_MAGNETON_FORCE;
            pulsarChck.IsChecked = PhysicSettings.Instance().DEFAULT_ENERGY_GRAVITON_PULSING;
            baseSetChck.IsChecked = false;
        }

        #endregion

        #region logika

        /// <summary>
        /// Inicializuje vsechna data
        /// </summary>
        public void InitData()
        {
            contourRocks = templates.rockSettings;

            if (contourRocks == null) contourRocks = new HashSet<ContourRock>();

            if (templates != null)
            {
                // jmeno kazde kontury pridej do comboboxu a vytvor objekt ContourRock, svazany s jejim id
                foreach (Template tmp in templates)
                {
                    if (contourRocks.Count(cnt => cnt.contour_name.Equals(tmp.name)) == 0)
                    {
                        // vytvori prislusny kamen (pouze referencni)
                        A_Rock contextRock = null;
                        if (blackHoleCmbIt.IsSelected) contextRock = new BlackHole();
                        if (generatorCmbIt.IsSelected) contextRock = new Generator();
                        if (gravitonCmbIt.IsSelected) contextRock = new Graviton();
                        if (magnetonCmbIt.IsSelected) contextRock = new Magneton();

                        contourRocks.Add(new ContourRock(contextRock, tmp.name));
                    }

                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = tmp.name;
                    item.IsSelected = true;
                    contourNameCombo.Items.Add(item);
                }

                // aktualni kontura je ta, ktera je svazana se jmenem, ktere je prave oznaceno v comboboxu
                actual_contour = contourRocks.Where(cnt => cnt.contour_name.Equals(((ComboBoxItem)(contourNameCombo.SelectedItem)).Content)).First();
                if (actual_contour.rock is Graviton) gravitonCmbIt.IsSelected = true;
                else if (actual_contour.rock is Generator) generatorCmbIt.IsSelected = true;
                else if (actual_contour.rock is Magneton) magnetonCmbIt.IsSelected = true;
                else if (actual_contour.rock is BlackHole) blackHoleCmbIt.IsSelected = true;

                if (blackHoleCmbIt.IsSelected)
                {
                    blackHoleGroup.Visibility = Visibility.Visible;
                }
                if (gravitonCmbIt.IsSelected)
                {
                    gravitonGroup.Visibility = Visibility.Visible;
                }
                if (generatorCmbIt.IsSelected)
                {
                    generatorGroup.Visibility = Visibility.Visible;
                }
                if (magnetonCmbIt.IsSelected)
                {
                    magnetonGroup.Visibility = Visibility.Visible;
                }

                LoadValues(); // pokud nacitame existujici data, musi se nacist informace o prvni konture
                initialized = true;
            }
        }

        /// <summary>
        /// Vyresetuje hodnoty aktualniho nastaveni kontury - pristupuje primo k objektu
        /// </summary>
        private void RestartValues()
        {
            // pokud nastavujeme konturu a zmenime typ kamene, vse se vynuluje a zacne se znovu
            ContourRock contourRck = contourRocks.Where(ctr => ctr.contour_name.Equals(((ComboBoxItem)contourNameCombo.SelectedItem).Content)).First();
            contourRocks.Remove(contourRck);
            A_Rock contextRock = null;
            if (blackHoleCmbIt.IsSelected && !(contourRck.rock is BlackHole)) contextRock = new BlackHole();
            if (generatorCmbIt.IsSelected && !(contourRck.rock is Generator)) contextRock = new Generator();
            if (gravitonCmbIt.IsSelected && !(contourRck.rock is Graviton)) contextRock = new Graviton();
            if (magnetonCmbIt.IsSelected && !(contourRck.rock is Magneton)) contextRock = new Magneton();

            if(contextRock != null) contourRck.rock = contextRock;
            contourRocks.Add(contourRck);
        }

        /// <summary>
        /// Nacte jiz nastavene hodnoty kontury
        /// </summary>
        private void LoadValues()
        {
            ContourRock contourRck = actual_contour;

            if (contourRck.rock is Graviton)
            {
                rockTypeCombo.SelectedItem = gravitonCmbIt;
                GravWeighSlider.Value = ((Graviton)contourRck.rock).Settings.weigh;
                pulsarChck.IsChecked = ((Graviton)contourRck.rock).Settings.Energy_pulsing;
                baseSetChck.IsChecked = !((Graviton)contourRck.rock).Settings_Allowed;
            }
            if (contourRck.rock is Magneton)
            {
                rockTypeCombo.SelectedItem = magnetonCmbIt;
                MagForceSlider.Value = ((Magneton)contourRck.rock).Settings.force;
                pulsarChck.IsChecked = ((Magneton)contourRck.rock).Settings.Energy_pulsing;
                baseSetChck.IsChecked = !((Magneton)contourRck.rock).Settings_Allowed;
            }
            if (contourRck.rock is BlackHole)
            {
                 rockTypeCombo.SelectedItem = blackHoleCmbIt;
                 BHweighSlider.Value = ((BlackHole)contourRck.rock).Settings.weigh;
                 pulsarChck.IsChecked = ((BlackHole)contourRck.rock).Settings.Energy_pulsing;
                 baseSetChck.IsChecked = !((BlackHole)contourRck.rock).Settings_Allowed;
            }
            if (contourRck.rock is Generator)
            {
                rockTypeCombo.SelectedItem = generatorCmbIt;
                GangleMaxSlider.Value = ((Generator)contourRck.rock).Settings.angle_maximum;
                GangleOffSlider.Value = ((Generator)contourRck.rock).Settings.angle_offset;
                GgenerSpeedSlider.Value = ((Generator)contourRck.rock).Settings.generatingSpeed ;
                GmaxPartVelocitySlider.Value = ((Generator)contourRck.rock).Settings.particle_maximum_speed;
                GminPartVelocitySlider.Value = ((Generator)contourRck.rock).Settings.particle_minimum_speed;
                GmaxPartSizeSlider.Value = ((Generator)contourRck.rock).Settings.particle_minimum_size;
                GminPartSizeSlider.Value = ((Generator)contourRck.rock).Settings.particle_maximum_size;
                pulsarChck.IsChecked = ((Generator)contourRck.rock).Settings.Energy_pulsing;
                baseSetChck.IsChecked = !((Generator)contourRck.rock).Settings_Allowed;
            }
        }

        /// <summary>
        /// Ulozi hodnoty kontury pri kliknuti na nastaveni jine kontury
        /// </summary>
        private void SaveValues()
        {
            ContourRock contourRck = actual_contour;
         
            if (contourRck.rock is Graviton)
            {
                ((Graviton)contourRck.rock).Settings.weigh = GravWeighSlider.Value;
                ((Graviton)contourRck.rock).Settings.enabled = true;
                ((Graviton)contourRck.rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((Graviton)contourRck.rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
            if (contourRck.rock is Magneton)
            {
                ((Magneton)contourRck.rock).Settings.force = MagForceSlider.Value;
                ((Magneton)contourRck.rock).Settings.enabled = true;
                ((Magneton)contourRck.rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((Magneton)contourRck.rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
            if (contourRck.rock is BlackHole)
            {
                ((BlackHole)contourRck.rock).Settings.weigh = (int)BHweighSlider.Value;
                ((BlackHole)contourRck.rock).Settings.enabled = true;
                ((BlackHole)contourRck.rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((BlackHole)contourRck.rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
            if (contourRck.rock is Generator)
            {
                ((Generator)contourRck.rock).Settings.particle_minimum_size = GminPartSizeSlider.Value;
                ((Generator)contourRck.rock).Settings.particle_maximum_size = GmaxPartSizeSlider.Value;
                ((Generator)contourRck.rock).Settings.angle_maximum = GangleMaxSlider.Value;
                ((Generator)contourRck.rock).Settings.angle_offset = GangleOffSlider.Value;
                ((Generator)contourRck.rock).Settings.generatingSpeed = GgenerSpeedSlider.Value;
                ((Generator)contourRck.rock).Settings.particle_maximum_speed = GmaxPartVelocitySlider.Value;
                ((Generator)contourRck.rock).Settings.particle_minimum_speed = GminPartVelocitySlider.Value;
                ((Generator)contourRck.rock).Settings.enabled = true;
                ((Generator)contourRck.rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((Generator)contourRck.rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
        }

        #endregion

    }
}
