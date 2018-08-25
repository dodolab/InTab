using System;
using InteractiveTable.GUI.Table;
using InteractiveTable.Managers;
using System.Windows;
using InteractiveTable.Settings;
using InteractiveTable.GUI.Other;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using System.Windows.Media.Imaging;

namespace InteractiveTable.Controls
{
    /// <summary>
    /// Controller for a panel with overal settings for the game board
    /// </summary>
    public class TableSettingsController
    {
        #region prom, get, set, const

        private SettingsPanel tableSetPanel;
        private TableManager tableManager;

        public TableSettingsController()
        {

        }

        public SettingsPanel TableSetPanel
        {
            get { return tableSetPanel; }
            set { this.tableSetPanel = value; }
        }

        public TableManager TableManager
        {
            get { return tableManager; }
            set { this.tableManager = value; }
        }

        
        public void SetHandlers()
        {
            // default settings of all checkboxes and sliders
            tableSetPanel.surfaceInterChck.IsChecked = PhysicSettings.Instance().DEFAULT_INTERACTION_ALLOWED;
            tableSetPanel.gravityMovementChck.IsChecked = PhysicSettings.Instance().DEFAULT_GRAVITON_ENABLED;
            tableSetPanel.magnetonMovementChck.IsChecked = PhysicSettings.Instance().DEFAULT_MAGNETON_ENABLED;
            tableSetPanel.generationChck.IsChecked = PhysicSettings.Instance().DEFAULT_GENERATOR_ENABLED;
            tableSetPanel.absorbChck.IsChecked = PhysicSettings.Instance().DEFAULT_BLACKHOLE_ENABLED;
            tableSetPanel.tableGravityChck.IsChecked = PhysicSettings.Instance().DEFAULT_TABLE_GRAVITY;
            tableSetPanel.gravitonLocalPulseChck.IsChecked = PhysicSettings.Instance().DEFAULT_ENERGY_GRAVITON_PULSING;
            tableSetPanel.magnetonLocalPulseChck.IsChecked = PhysicSettings.Instance().DEFAULT_ENERGY_MAGNETON_PULSING;
            tableSetPanel.particleEnergyLoseChck.IsChecked = PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING;
            tableSetPanel.particleColorChangeChck.IsChecked = GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_ALLOWED;
            tableSetPanel.generatorRegularGenChck.IsChecked = PhysicSettings.Instance().DEFAULT_GENERATING_REGULAR;
            tableSetPanel.particleSizeChck.IsChecked = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZING_ENABLED;
            tableSetPanel.gridDispChck.IsChecked = GraphicsSettings.Instance().DEFAULT_GRID_ENABLED;
            tableSetPanel.particleColorChangeChck.IsChecked = GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_ALLOWED;
            tableSetPanel.rockDispChck.IsChecked = GraphicsSettings.Instance().DEFAULT_OUTPUT_ROCK_DISPLAY;

            tableSetPanel.particleEnergyLoseSizeSlider.Value = PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED;
            tableSetPanel.particleEnergyLoseSizeSlider.Maximum = PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX;
            tableSetPanel.particleEnergyLoseSizeTb.Text = tableSetPanel.particleEnergyLoseSizeSlider.Value.ToString();
            tableSetPanel.generatorGenFirstAngleSlider.Value = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_OFFSET;
            tableSetPanel.generatorGenFirstAngleTb.Text = tableSetPanel.generatorGenFirstAngleSlider.Value.ToString();
            tableSetPanel.generatorGenAngleSlider.Value = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_MAX;
            tableSetPanel.generatorGenAngleTb.Text = tableSetPanel.generatorGenAngleSlider.Value.ToString();
            tableSetPanel.generatorMinVelocSlider.Value = PhysicSettings.Instance().DEFAULT_MIN_GENERATING_VELOCITY;
            tableSetPanel.generatorMinVelocTb.Text = tableSetPanel.generatorMinVelocSlider.Value.ToString();
            tableSetPanel.generatorMaxVelocSlider.Value = PhysicSettings.Instance().DEFAULT_MAX_GENERATING_VELOCITY;
            tableSetPanel.generatorMaxVelocTb.Text = tableSetPanel.generatorMaxVelocSlider.Value.ToString();
            tableSetPanel.generatorSpeedSlider.Value = PhysicSettings.Instance().DEFAULT_GENERATING_SPEED;
            tableSetPanel.generatorSpeedTb.Text = tableSetPanel.generatorSpeedSlider.Value.ToString();
            tableSetPanel.generatorMinSizeSlider.Value = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
            tableSetPanel.generatorMinSizeTb.Text = tableSetPanel.generatorMinSizeSlider.Value.ToString();
            tableSetPanel.generatorMaxSizeSlider.Value = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
            tableSetPanel.generatorMaxSizeTb.Text = tableSetPanel.generatorMaxSizeSlider.Value.ToString();
            tableSetPanel.tableSizeSlider.Value = CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER;
            tableSetPanel.tableSizeTb.Text = CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER.ToString("#");
            tableSetPanel.tableGravityXTbx.Text = PhysicSettings.Instance().DEFAULT_TABLE_GRAVITY_VECTOR.X.ToString();
            tableSetPanel.tableGravityYTbx.Text = PhysicSettings.Instance().DEFAULT_TABLE_GRAVITY_VECTOR.Y.ToString();

            tableSetPanel.pauseImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(pauseImage_MouseUp);
            tableSetPanel.playImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(playImage_MouseUp);
            tableSetPanel.stopImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(stopImage_MouseUp);
            tableSetPanel.cameraImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(cameraImage_MouseUp);
            tableSetPanel.recordImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(recordImage_MouseUp);
            tableSetPanel.simulationImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(simulationImage_MouseUp);
            tableSetPanel.outputImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(outputImage_MouseUp);
            tableSetPanel.generatorGenAngleSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.generatorGenFirstAngleSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.generatorMaxVelocSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.generatorMinVelocSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.particleEnergyLoseSizeSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.generatorSpeedSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.generatorMinSizeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.generatorMaxSizeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.tableSizeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderValueChanged);
            tableSetPanel.generatorRegularGenChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.gravityMovementChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.magnetonMovementChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.particleColorChangeChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.particleEnergyLoseChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.particleSizeChck.Click += new RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.gravitonLocalPulseChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.magnetonLocalPulseChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.surfaceInterChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.tableGravityChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.generationChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.absorbChck.Click += new System.Windows.RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.gridDispChck.Click += new RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.blackHoleLocalPulseChck.Click += new RoutedEventHandler(CheckBoxCheckChanged);
            tableSetPanel.rockDispChck.Click += new RoutedEventHandler(CheckBoxCheckChanged);

            tableSetPanel.gravityComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(ComboBoxItem_changed);
            tableSetPanel.magnetonComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(ComboBoxItem_changed);
            tableSetPanel.particleColorChangeComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(ComboBoxItem_changed);
            tableSetPanel.generationComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(ComboBoxItem_changed);
            tableSetPanel.absorbComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(ComboBoxItem_changed);
            tableSetPanel.particleSizeChangeComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(ComboBoxItem_changed);
            tableSetPanel.tableGravityXTbx.LostFocus += new System.Windows.RoutedEventHandler(TextBox_FocusLost);
            tableSetPanel.tableGravityYTbx.LostFocus += new System.Windows.RoutedEventHandler(TextBox_FocusLost);
            tableSetPanel.settingVisibilityBut.MouseUp += new System.Windows.Input.MouseButtonEventHandler(settingVisibilityBut_Click);
            tableSetPanel.particleColorChangeBut.Click += new RoutedEventHandler(particleColorChangeBut_Click);
        }

        #endregion

        #region handlery tlacitek

        /// <summary>
        /// Click on STOP will reset the simulation
        /// </summary>
        private void stopImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            tableManager.StopThread();
        }

        /// <summary>
        /// Click on PAUSE will pause the simulation
        /// </summary>
        private void pauseImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            tableManager.PauseThread();
        }

        /// <summary>
        /// Click on PLAY will execute the simulation
        /// </summary>
        private void playImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            tableManager.RunThread();
        }

        /// <summary>
        /// Click on the camera will display a capture window
        /// </summary>
        private void cameraImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(tableManager.InputManager.InteractiveWindow != null && tableManager.InputManager.InteractiveWindow.IsVisible))
            {
                InteractiveWindow intWin = new InteractiveWindow();
                intWin.Owner = CommonAttribService.mainWindow;
                tableManager.InputManager.InteractiveWindow = intWin;
                intWin.Show();
            }
        }

        /// <summary>
        /// Click on the RECORD button will execute camera recording for AR
        /// </summary>
        private void recordImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (tableManager.InputManager.IsRunning())
            {
                tableSetPanel.recordImage.Source = new BitmapImage(new Uri("/InteractiveTable;component/Template/images/tableIcons/play_record_test.png", UriKind.Relative));
                tableManager.InputManager.StopThread();
            }
            else
            {
                if (tableManager.InputManager.RunThread())
                {
                    tableSetPanel.recordImage.Source = new BitmapImage(new Uri("/InteractiveTable;component/Template/images/tableIcons/play_record_running_test.png", UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// Click on the OUTPUT icon will enable rendering into the projector output window
        /// </summary>
        private void outputImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CommonAttribService.OUTPUT_DRAW_ALLOWED = !CommonAttribService.OUTPUT_DRAW_ALLOWED;

            if (CommonAttribService.OUTPUT_DRAW_ALLOWED)
            {
                tableSetPanel.outputImage.Source = new BitmapImage(new Uri("/InteractiveTable;component/Template/images/tableIcons/pl_output_active.png", UriKind.Relative));
            }
            else
            {
                tableSetPanel.outputImage.Source = new BitmapImage(new Uri("/InteractiveTable;component/Template/images/tableIcons/pl_output.png", UriKind.Relative));
            }

        }

        /// <summary>
        /// Click on the OUTPUT icon will enable rendering into the simulation window
        /// </summary>
        private void simulationImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CommonAttribService.SIMULATION_DRAW_ALLOWED = !CommonAttribService.SIMULATION_DRAW_ALLOWED;

            if (CommonAttribService.SIMULATION_DRAW_ALLOWED)
            {
                tableSetPanel.simulationImage.Source = new BitmapImage(new Uri("/InteractiveTable;component/Template/images/tableIcons/pl_simulation_active.png", UriKind.Relative));
            }
            else
            {
                tableSetPanel.simulationImage.Source = new BitmapImage(new Uri("/InteractiveTable;component/Template/images/tableIcons/pl_simulation.png", UriKind.Relative));
            }
        }

        /// <summary>
        /// Click on color settings 
        /// </summary>
        private void particleColorChangeBut_Click(object sender, RoutedEventArgs e)
        {
            ParticleColorSetWindow partCol = new ParticleColorSetWindow();
            partCol.Owner = CommonAttribService.mainWindow;
            partCol.Show();
        }

        #endregion

        #region settings handlers



        /// <summary>
        /// Click on HID will hide or show the settings panel
        /// </summary>
        private void settingVisibilityBut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CommonAttribService.mainWindow.WindowState == WindowState.Maximized)
            {
                MessageBox.Show("This function is not available in maximized mode!");
            }
            else
            {
                if (tableSetPanel.settingsScroll.Visibility == Visibility.Collapsed)
                {

                    tableManager.TablePanel.tableAreaPanel.tableGrid.Width = tableManager.TablePanel.tableAreaPanel.Width - tableSetPanel.settingsScroll.Width;
                    tableManager.TablePanel.tableAreaPanel.Width = tableManager.TablePanel.tableAreaPanel.Width - tableSetPanel.settingsScroll.Width;
                    CommonAttribService.mainWindow.Width += tableSetPanel.settingsScroll.Width;
                    tableSetPanel.settingsScroll.Visibility = Visibility.Visible;
                }
                else
                {

                    tableManager.TablePanel.tableAreaPanel.tableGrid.Width = tableManager.TablePanel.tableAreaPanel.Width + tableSetPanel.settingsScroll.Width;
                    tableManager.TablePanel.tableAreaPanel.Width = tableManager.TablePanel.tableAreaPanel.Width + tableSetPanel.settingsScroll.Width;
                    CommonAttribService.mainWindow.Width -= tableSetPanel.settingsScroll.Width;
                    tableSetPanel.settingsScroll.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Change of value of any textbox
        /// </summary>
        private void TextBox_FocusLost(object sender, System.Windows.RoutedEventArgs e)
        {

            if (sender == tableSetPanel.tableGravityXTbx) // X-axis gravity
            {
                double value = 0;
                try
                {
                    value = Double.Parse(tableSetPanel.tableGravityXTbx.Text);
                }
                catch
                {
                    value = 0;
                }

                tableManager.TableDepositor.table.Settings.gravity.X = value;
            }

            if (sender == tableSetPanel.tableGravityYTbx) // Y-axis gravity
            {
                double value = 0;
                try
                {
                    value = Double.Parse(tableSetPanel.tableGravityYTbx.Text);
                }
                catch
                {
                    value = 0;
                }

                tableManager.TableDepositor.table.Settings.gravity.Y = value;
            }
        }

        /// <summary>
        /// Change of all dropdowns
        /// </summary>
        private void ComboBoxItem_changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender == tableSetPanel.gravityComboBox) // gravity mode change
            {
                if (tableSetPanel.aditiveGravityCmbIt.IsSelected) PhysicsSettings.gravitationMode = GravitationMode.ADITIVE;
                if (tableSetPanel.averageGravityCmbIt.IsSelected) PhysicsSettings.gravitationMode = GravitationMode.AVERAGE;
                if (tableSetPanel.multiplyGravityCmbIt.IsSelected) PhysicsSettings.gravitationMode = GravitationMode.MULTIPLY;
            }

            if (sender == tableSetPanel.magnetonComboBox) // magnetism mode change
            {
                if (tableSetPanel.aditiveMagnetonCmbIt.IsSelected) PhysicsSettings.magnetismMode = MagnetismMode.ADITIVE;
                if (tableSetPanel.averageMagnetonCmbIt.IsSelected) PhysicsSettings.magnetismMode = MagnetismMode.AVERAGE;
                if (tableSetPanel.multiplyMagnetonCmbIt.IsSelected) PhysicsSettings.magnetismMode = MagnetismMode.MULTIPLY;
            }


            if (sender == tableSetPanel.particleColorChangeComboBox) // colouring mode change
            {
                if (tableSetPanel.colorChange_gravityCmbIt.IsSelected) GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE = ParticleColorMode.GRAVITY;
                if (tableSetPanel.colorChange_sizeCmbIt.IsSelected) GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE = ParticleColorMode.SIZE;
                if (tableSetPanel.colorChange_velocityCmbIt.IsSelected) GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE = ParticleColorMode.VELOCITY;
                if (tableSetPanel.colorChange_weighCmbIt.IsSelected) GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE = ParticleColorMode.WEIGH;
            }

            if (sender == tableSetPanel.generationComboBox) // particle generating mode change
            {
                if (tableSetPanel.standardGenerationCmbIt.IsSelected) tableManager.TableDepositor.table.Settings.generatorSettings.generationMode = GenerationMode.STANDARD;
                if (tableSetPanel.strangeGenerationCmbIt.IsSelected) tableManager.TableDepositor.table.Settings.generatorSettings.generationMode = GenerationMode.STRANGE;
            }

            if (sender == tableSetPanel.absorbComboBox) // absorption mode change
            {
                if (tableSetPanel.blackHoleAbsorbCmbIt.IsSelected) PhysicsSettings.absorptionMode = AbsorptionMode.BLACKHOLE;
                if (tableSetPanel.regenerationAbsorbCmbIt.IsSelected) PhysicsSettings.absorptionMode = AbsorptionMode.RECYCLE;
                if (tableSetPanel.selectingAbsorbCmbIt.IsSelected) PhysicsSettings.absorptionMode = AbsorptionMode.SELECT;
            }
            
            if (sender == tableSetPanel.particleSizeChangeComboBox) // dynamic particle size mode change
            {
                if (tableSetPanel.sizeChange_gravityCmbIt.IsSelected) PhysicsSettings.particle_sizeMode = ParticleSizeMode.GRAVITY;
                if (tableSetPanel.sizeChange_velocityCmbIt.IsSelected) PhysicsSettings.particle_sizeMode = ParticleSizeMode.VELOCITY;
                if (tableSetPanel.sizeChange_weighCmbIt.IsSelected) PhysicsSettings.particle_sizeMode = ParticleSizeMode.WEIGH;
                if (tableSetPanel.sizeChange_noneCmbIt.IsSelected) PhysicsSettings.particle_sizeMode = ParticleSizeMode.NONE;
            }
        }

        /// <summary>
        /// Change of all checkboxes
        /// </summary>
        private void CheckBoxCheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender == tableSetPanel.generatorRegularGenChck) // regular generating of particles
            {
                tableManager.TableDepositor.table.Settings.generatorSettings.Regular_generating =
                    (bool)tableSetPanel.generatorRegularGenChck.IsChecked;
            }

            if (sender == tableSetPanel.gravityMovementChck) // variable gravity field
            {
                tableManager.TableDepositor.table.Settings.gravitonSettings.enabled =
                    (bool)tableSetPanel.gravityMovementChck.IsChecked;
            }

            if (sender == tableSetPanel.magnetonMovementChck) // magnetism
            {
                tableManager.TableDepositor.table.Settings.magnetonSettings.enabled =
                    (bool)tableSetPanel.magnetonMovementChck.IsChecked;
            }

            if (sender == tableSetPanel.particleColorChangeChck) // particle color change
            {
                GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_ALLOWED =
                    (bool)tableSetPanel.particleColorChangeChck.IsChecked;
            }

            if (sender == tableSetPanel.particleEnergyLoseChck) // particle energy change
            {
                tableManager.TableDepositor.table.Settings.energy_loosing =
                    (bool)tableSetPanel.particleEnergyLoseChck.IsChecked;
            }

            if (sender == tableSetPanel.gravitonLocalPulseChck) // change of local pulse of gravity stones
            {
                tableManager.TableDepositor.table.Settings.gravitonSettings.Energy_pulsing =
                   (bool)tableSetPanel.gravitonLocalPulseChck.IsChecked;
            }

            if (sender == tableSetPanel.magnetonLocalPulseChck) // change of local pulse of magnet stones
            {
                tableManager.TableDepositor.table.Settings.magnetonSettings.Energy_pulsing =
                   (bool)tableSetPanel.magnetonLocalPulseChck.IsChecked;
            }

            if (sender == tableSetPanel.surfaceInterChck) // collision change with the borders of the table
            {
                tableManager.TableDepositor.table.Settings.interaction =
                   (bool)tableSetPanel.surfaceInterChck.IsChecked;
            }

            if (sender == tableSetPanel.tableGravityChck) // gravity generated by the table
            {
                tableManager.TableDepositor.table.Settings.gravity_allowed =
                   (bool)tableSetPanel.tableGravityChck.IsChecked;
            }

            if (sender == tableSetPanel.generationChck) // enabling particle generator
            {
                tableManager.TableDepositor.table.Settings.generatorSettings.enabled =
                  (bool)tableSetPanel.generationChck.IsChecked;
            }

            if (sender == tableSetPanel.absorbChck) // enabling particle absorber
            {
                tableManager.TableDepositor.table.Settings.blackHoleSettings.enabled =
                  (bool)tableSetPanel.absorbChck.IsChecked;
            }

            if (sender == tableSetPanel.particleSizeChck) // enable variable size of particles
            {
                PhysicsSettings.particle_sizeChanging_allowed = (bool)tableSetPanel.particleSizeChck.IsChecked;
            }

            if (sender == tableSetPanel.gridDispChck) // display grid
            {
                GraphicsSettings.Instance().DEFAULT_GRID_ENABLED = (bool)tableSetPanel.gridDispChck.IsChecked;
            }

            if (sender == tableSetPanel.blackHoleLocalPulseChck)
            {
                tableManager.TableDepositor.table.Settings.blackHoleSettings.Energy_pulsing =
                    (bool)tableSetPanel.blackHoleLocalPulseChck.IsChecked;
            }

            if (sender == tableSetPanel.rockDispChck) // draw stones in the opengl window
            {
                GraphicsSettings.Instance().DEFAULT_OUTPUT_ROCK_DISPLAY = (bool)tableSetPanel.rockDispChck.IsChecked;
            }
        }


        /// <summary>
        /// Change of any slider
        /// </summary>
        private void sliderValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender == tableSetPanel.generatorGenAngleSlider) // change of directions of generators
            {
                tableSetPanel.generatorGenAngleTb.Text = tableSetPanel.generatorGenAngleSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.generatorSettings.angle_maximum = tableSetPanel.generatorGenAngleSlider.Value;
            }

            if (sender == tableSetPanel.generatorGenFirstAngleSlider) // change of initial direction of generated particles
            {
                tableSetPanel.generatorGenFirstAngleTb.Text = tableSetPanel.generatorGenFirstAngleSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.generatorSettings.angle_offset = tableSetPanel.generatorGenFirstAngleSlider.Value;
            }


            if (sender == tableSetPanel.generatorMaxVelocSlider) // change of max velocity of generated particles
            {
                tableSetPanel.generatorMaxVelocTb.Text = tableSetPanel.generatorMaxVelocSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.generatorSettings.particle_maximum_speed = tableSetPanel.generatorMaxVelocSlider.Value;

                if (tableSetPanel.generatorMinVelocSlider.Value > tableSetPanel.generatorMaxVelocSlider.Value)
                {
                    MessageBox.Show("Maximum velocity cannot be lower than the minimal!");
                    tableSetPanel.generatorMaxVelocSlider.Value = tableSetPanel.generatorMinVelocSlider.Value;
                }
            }

            if (sender == tableSetPanel.generatorMinVelocSlider) // change of min velocity of generated particles
            {

                tableSetPanel.generatorMinVelocTb.Text = tableSetPanel.generatorMinVelocSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.generatorSettings.particle_maximum_speed = tableSetPanel.generatorMinVelocSlider.Value;

                if (tableSetPanel.generatorMinVelocSlider.Value > tableSetPanel.generatorMaxVelocSlider.Value)
                {
                    MessageBox.Show("Minimum velocity cannot be greater than maximal!");
                    tableSetPanel.generatorMinVelocSlider.Value = tableSetPanel.generatorMaxVelocSlider.Value;
                }
            }

            if (sender == tableSetPanel.particleEnergyLoseSizeSlider) // change of energy decceleration speed
            {
                tableSetPanel.particleEnergyLoseSizeTb.Text = tableSetPanel.particleEnergyLoseSizeSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.energy_loosing_speed = tableSetPanel.particleEnergyLoseSizeSlider.Value;
            }

            if (sender == tableSetPanel.generatorSpeedSlider) // change of generating speed
            {
                tableSetPanel.generatorSpeedTb.Text = tableSetPanel.generatorSpeedSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.generatorSettings.generatingSpeed = tableSetPanel.generatorSpeedSlider.Value;
            }

            if (sender == tableSetPanel.generatorMinSizeSlider) // change of min size of generated particles
            {
                tableSetPanel.generatorMinSizeTb.Text = tableSetPanel.generatorMinSizeSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.generatorSettings.particle_minimum_size = tableSetPanel.generatorMinSizeSlider.Value;

                if (tableSetPanel.generatorMinSizeSlider.Value > tableSetPanel.generatorMaxSizeSlider.Value)
                {
                    MessageBox.Show("Minimal size cannot be greater than maximal size!");
                    tableSetPanel.generatorMinSizeSlider.Value = tableSetPanel.generatorMaxSizeSlider.Value;
                }
            }

            if (sender == tableSetPanel.generatorMaxSizeSlider) // change of max size of generated particles
            {
                tableSetPanel.generatorMaxSizeTb.Text = tableSetPanel.generatorMaxSizeSlider.Value.ToString("#.##");
                tableManager.TableDepositor.table.Settings.generatorSettings.particle_maximum_size = tableSetPanel.generatorMaxSizeSlider.Value;

                if (tableSetPanel.generatorMinSizeSlider.Value > tableSetPanel.generatorMaxSizeSlider.Value)
                {
                    MessageBox.Show("Maximal siz cannot be lower than minimal size!");
                    tableSetPanel.generatorMaxSizeSlider.Value = tableSetPanel.generatorMinSizeSlider.Value;
                }
            }

            if (sender == tableSetPanel.tableSizeSlider) // change of the size of the table
            {
                tableSetPanel.tableSizeTb.Text = tableSetPanel.tableSizeSlider.Value.ToString("#");
                double old_multiplier = CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER;
                double new_multiplier = tableSetPanel.tableSizeSlider.Value;

                CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER = new_multiplier;
                CommonAttribService.ACTUAL_TABLE_WIDTH = (int)(CommonAttribService.ACTUAL_TABLE_WIDTH * (new_multiplier / old_multiplier));
                CommonAttribService.ACTUAL_TABLE_HEIGHT = (int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * (new_multiplier / old_multiplier));

                tableManager.TablePanel.tableAreaPanel.TableController.RecalculateStones();
            }
        }
       
        #endregion
    }
}
