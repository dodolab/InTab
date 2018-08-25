using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using InteractiveTable.Settings;

namespace InteractiveTable.GUI.CaptureSet
{
    /// <summary>
    /// MVC for assigning stones to particular contours
    /// </summary>
    public partial class ContourSetWindow : Window
    {
        #region variables

        private Templates templates; // templates
        private HashSet<ContourRock> contourRocks; // stone + contour identifier
        private Boolean initialized = false; // init flag
        private ContourRock actual_contour; // a current contour that is to be configured

        #endregion

        #region cons, getters, setters
        
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
        
        public Templates Templates
        {
            get { return templates; }
            set { this.templates = value; }
        }

        public HashSet<ContourRock> ContourRocks
        {
            get { return contourRocks; }
        }

        #endregion

        #region view handlers

        /// <summary>
        /// Change of a slider will just change a text label below
        /// </summary>
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
        /// Change of a stone type will either hide or show relevant panels
        /// </summary>
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
        /// Resets all values
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

        #region combobox handlers

        /// <summary>
        /// Contour selection change will save current values and reload values
        /// </summary>
        private void contourNameCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialized)
            {
                SaveValues();
                RestartComponents();
                actual_contour = contourRocks.First(ctr => ctr.contour_name.Equals(((ComboBoxItem)contourNameCombo.SelectedItem).Content));
                LoadValues();
            }
        }


        #endregion

        #region other handlers

        /// <summary>
        /// Click on OK will save the settings and close the window
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Do you want to save your changes?", "Settings change", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == 
                System.Windows.Forms.DialogResult.Yes)
            {
                SaveValues();
                templates.rockSettings = contourRocks;
                
                // Try use a default path. If something goes wrong, display a file dialog
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
        /// Loads default settings
        /// </summary>
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

        #region logic

        /// <summary>
        /// Initializes all data
        /// </summary>
        public void InitData()
        {
            contourRocks = templates.rockSettings;

            if (contourRocks == null) contourRocks = new HashSet<ContourRock>();

            if (templates != null)
            {
                // put the name of each contour into a combobox and create a ContourRock, bound with its id
                foreach (Template tmp in templates)
                {
                    if (contourRocks.Count(cnt => cnt.contour_name.Equals(tmp.name)) == 0)
                    {
                        // create a referential stone
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

                LoadValues(); 
                initialized = true;
            }
        }

        /// <summary>
        /// Resets values of the current contour settings
        /// </summary>
        private void RestartValues()
        {
            // if we change a type of a stone, everything will be reseted 
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
        /// Loads values from a contour
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
        /// Saves values of a contour upon click on settings of another contour
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
