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
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Settings;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// Editor virtualnich kamenu v simulacnim okne
    /// </summary>
    public partial class RockEditorWindow : Window
    {
        // kamen, ktery se bude editovat
        private A_Rock rock;
 

        #region konstruktory, gettery a settery

        public RockEditorWindow()
        { 
            InitializeComponent();

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

   

        #endregion

        #region slider handlery

        /// <summary>
        /// Zmena hodnoty slideru pouze zmeni txt pod nim
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

        #endregion


        #region logika

        /// <summary>
        /// Inicializuje hodnoty komponent podle kamene
        /// </summary>
        /// <param name="rock"></param>
        public void InitData(A_Rock rock)
        {
            this.rock = rock;

                if (rock is BlackHole)
                {
                    blackHoleGroup.Visibility = Visibility.Visible;
                }
                if (rock is Graviton)
                {
                    gravitonGroup.Visibility = Visibility.Visible;
                }
                if (rock is Generator)
                {
                    generatorGroup.Visibility = Visibility.Visible;
                }
                if (rock is Magneton)
                {
                    magnetonGroup.Visibility = Visibility.Visible;
                }

                LoadValues(); // pokud nacitame existujici data, musi se nacist informace o prvni konture
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



       /// <summary>
       /// Nate hodnoty kontury, ktera jiz ma nastavene vlastnosti
       /// </summary>
        private void LoadValues()
        {

            if (rock is Graviton)
            {
                GravWeighSlider.Value = ((Graviton)rock).Settings.weigh;
                pulsarChck.IsChecked = ((Graviton)rock).Settings.Energy_pulsing;
                baseSetChck.IsChecked = !((Graviton)rock).Settings_Allowed;
            }
            if (rock is Magneton)
            {
                MagForceSlider.Value = ((Magneton)rock).Settings.force;
                pulsarChck.IsChecked = ((Magneton)rock).Settings.Energy_pulsing;
                baseSetChck.IsChecked = !((Magneton)rock).Settings_Allowed;
            }
            if (rock is BlackHole)
            {
                 BHweighSlider.Value = ((BlackHole)rock).Settings.weigh;
                 pulsarChck.IsChecked = ((BlackHole)rock).Settings.Energy_pulsing;
                 baseSetChck.IsChecked = !((BlackHole)rock).Settings_Allowed;
            }
            if (rock is Generator)
            {
                GangleMaxSlider.Value = ((Generator)rock).Settings.angle_maximum;
                GangleOffSlider.Value = ((Generator)rock).Settings.angle_offset;
                GgenerSpeedSlider.Value = ((Generator)rock).Settings.generatingSpeed ;
                GmaxPartVelocitySlider.Value = ((Generator)rock).Settings.particle_maximum_speed;
                GminPartVelocitySlider.Value = ((Generator)rock).Settings.particle_minimum_speed;
                GmaxPartSizeSlider.Value = ((Generator)rock).Settings.particle_minimum_size;
                GminPartSizeSlider.Value = ((Generator)rock).Settings.particle_maximum_size;
                pulsarChck.IsChecked = ((Generator)rock).Settings.Energy_pulsing;
                baseSetChck.IsChecked = !((Generator)rock).Settings_Allowed;
            }
        }

        /// <summary>
        /// Ulozi hodnoty kontury pri kliknuti na nastaveni jine kontury
        /// </summary>
        private void SaveValues()
        {
         
            if (rock is Graviton)
            {
                ((Graviton)rock).Settings.weigh = GravWeighSlider.Value;
                ((Graviton)rock).Settings.enabled = true;
                ((Graviton)rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((Graviton)rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
            if (rock is Magneton)
            {
                ((Magneton)rock).Settings.force = MagForceSlider.Value;
                ((Magneton)rock).Settings.enabled = true;
                ((Magneton)rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((Magneton)rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
            if (rock is BlackHole)
            {
                ((BlackHole)rock).Settings.weigh = (int)BHweighSlider.Value;
                ((BlackHole)rock).Settings.enabled = true;
                ((BlackHole)rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((BlackHole)rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
            if (rock is Generator)
            {
                ((Generator)rock).Settings.particle_minimum_size = GminPartSizeSlider.Value;
                ((Generator)rock).Settings.particle_maximum_size = GmaxPartSizeSlider.Value;
                ((Generator)rock).Settings.particle_maximum_speed = GmaxPartVelocitySlider.Value;
                ((Generator)rock).Settings.particle_minimum_speed = GminPartVelocitySlider.Value;

                ((Generator)rock).Settings.angle_maximum = GangleMaxSlider.Value;
                ((Generator)rock).Settings.angle_offset = GangleOffSlider.Value;
                ((Generator)rock).Settings.generatingSpeed = GgenerSpeedSlider.Value;
                ((Generator)rock).Settings.enabled = true;
                ((Generator)rock).Settings.Energy_pulsing = (bool)pulsarChck.IsChecked;
                ((Generator)rock).Settings_Allowed = !((bool)baseSetChck.IsChecked);
            }
        }

        #endregion

        /// <summary>
        /// Kliknuti na tlacitko OK ulozi nastaveni a zavre okno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
                SaveValues();
                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift)) this.Close();
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
    }
}

