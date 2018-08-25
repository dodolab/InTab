using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using InteractiveTable.Settings;
using DirectShowLib;
using InteractiveTable.Managers;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// User settings window
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            okBut.Click += new RoutedEventHandler(okBut_Click);
            dependOutputSizeChck.Click += new RoutedEventHandler(dependOutputSizeChck_Click);

            // get all webcams available and fill the combobox
            DsDevice[] devs = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            for (int i = 0; i < devs.Length; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = devs[i].Name;
                item.IsSelected = true;
                camIndexCombo.Items.Add(item);
            }
        }

        /// <summary>
        /// Load all values from storage
        /// </summary>
        public void LoadValues()
        {
            contourPathTbx.Text = CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH;
            camIndexCombo.SelectedIndex = CaptureSettings.Instance().DEFAULT_CAMERA_INDEX;
            dependOutputSizeChck.IsChecked = GraphicsSettings.Instance().OUTPUT_TABLE_SIZE_DEPENDENT;
            contourPathTbx.IsEnabled = GraphicsSettings.Instance().OUTPUT_TABLE_SIZE_DEPENDENT;
            dependOutputSizeTbx.Text = CommonAttribService.ACTUAL_OUTPUT_WIDTH.ToString();

            partColorRedTbx.Text = GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_R.ToString();
            partColorGreenTbx.Text = GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_G.ToString();
            partColorBlueTbx.Text = GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_B.ToString();
            motionDetectionChck.IsChecked = CaptureSettings.Instance().MOTION_DETECTION;
            detectionThreshold.Text = CaptureSettings.Instance().MOTION_TOLERANCE.ToString();
        }

        /// <summary>
        /// Click on RESOLUTION DEPENDENCY button
        /// </summary>
        private void dependOutputSizeChck_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)dependOutputSizeChck.IsChecked) dependOutputSizeTbx.IsEnabled = false;
            else dependOutputSizeTbx.IsEnabled = true;
        }

        /// <summary>
        /// Click on Load contour button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contourPathBut_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Templates(*.bin)|*.bin";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = ofd.FileName;
                try
                {
                    contourPathTbx.Text = fileName;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Click on OK will save all changes
        /// </summary>
        private void okBut_Click(object sender, RoutedEventArgs e)
        {
            CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH = contourPathTbx.Text;
            CaptureSettings.Instance().DEFAULT_CAMERA_INDEX = camIndexCombo.SelectedIndex;
            GraphicsSettings.Instance().OUTPUT_TABLE_SIZE_DEPENDENT = (bool)dependOutputSizeChck.IsChecked;
            CaptureSettings.Instance().MOTION_DETECTION = (bool)motionDetectionChck.IsChecked;

            try
            {
                GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_R = Byte.Parse(partColorRedTbx.Text);
                GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_G = Byte.Parse(partColorGreenTbx.Text);
                GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_B = Byte.Parse(partColorBlueTbx.Text);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Wrong values of particle colors (need to be 0-255)");
                return; 
            }
            try
            {
                int otp_width = Int32.Parse(dependOutputSizeTbx.Text);
                if (otp_width > 100 && otp_width < 2000)
                {
                   CommonAttribService.ACTUAL_OUTPUT_WIDTH = otp_width;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Allowed sizes are 100-2000");
                    return;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("The size cannot be determined!");
                return;
            }

            try
            {
                int tolerance = Int32.Parse(detectionThreshold.Text);
                if (tolerance < 500 || tolerance > 10000) throw new Exception();
                CaptureSettings.Instance().MOTION_TOLERANCE = tolerance;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Allowed value for tolerance must be in 500-10000");
                return;
            }

            CaptureSettings.Instance().Save();
            GraphicsSettings.Instance().Save();

              // if the left shift is pressed, we will not close the window (WTF??)
              if(!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))  this.Close();
        }

        /// <summary>
        /// Reset of al settings
        /// </summary>
        private void resetBut_Click(object sender, RoutedEventArgs e)
        {
            CalibrationSettings.Instance().Restart();
            CaptureSettings.Instance().Restart();
            GraphicsSettings.Instance().Restart();
            PhysicSettings.Instance().Restart();
        }
    }
}
