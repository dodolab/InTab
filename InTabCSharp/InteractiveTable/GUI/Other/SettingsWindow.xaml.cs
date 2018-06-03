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
using System.Windows.Forms;
using System.IO;
using InteractiveTable.Settings;
using DirectShowLib;
using InteractiveTable.Managers;
using System.Net;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// Okno pro uzivatelske nastaveni, zde bude mozno nastavit vse, co bude pri dalsim
    /// startu programu defaultne prednastaveno (napr. cesta k souboru kontur)
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            okBut.Click += new RoutedEventHandler(okBut_Click);
            dependOutputSizeChck.Click += new RoutedEventHandler(dependOutputSizeChck_Click);

            // ziskame vsechny webkamery
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
        /// Nacte vsechny hodnoty z nastaveni
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
            serverIPTbx.Text = CaptureSettings.Instance().SERVER_IP_ADDRESS;
            sendIntervalTbx.Text = CaptureSettings.Instance().SEND_INTERVAL.ToString();
            motionDetectionChck.IsChecked = CaptureSettings.Instance().MOTION_DETECTION;
            detectionThreshold.Text = CaptureSettings.Instance().MOTION_TOLERANCE.ToString();
            if (CaptureSettings.Instance().SEND_IMAGES) sendImageRadio.IsChecked = true;
            else sendRockRadio.IsChecked = true;
        }

        /// <summary>
        /// Kliknuti na tlacitko Zavislost rozliseni vystupu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dependOutputSizeChck_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)dependOutputSizeChck.IsChecked) dependOutputSizeTbx.IsEnabled = false;
            else dependOutputSizeTbx.IsEnabled = true;
        }

        /// <summary>
        /// Kliknuti na tlacitko NALEZT nalezne soubor se sablonami
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
        /// Kliknuti na tlacitko OK ulozi veskere zmeny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okBut_Click(object sender, RoutedEventArgs e)
        {

            CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH = contourPathTbx.Text;
            CaptureSettings.Instance().DEFAULT_CAMERA_INDEX = camIndexCombo.SelectedIndex;
            GraphicsSettings.Instance().OUTPUT_TABLE_SIZE_DEPENDENT = (bool)dependOutputSizeChck.IsChecked;
            CaptureSettings.Instance().SEND_IMAGES =(bool)sendImageRadio.IsChecked;
            CaptureSettings.Instance().MOTION_DETECTION = (bool)motionDetectionChck.IsChecked;

            String ipAddress = serverIPTbx.Text;

            IPAddress address;
            if (IPAddress.TryParse(ipAddress, out address))
            {
                CaptureSettings.Instance().SERVER_IP_ADDRESS = ipAddress;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Špatná IP adresa serveru!");
                return;
            }

            try
            {
                GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_R = Byte.Parse(partColorRedTbx.Text);
                GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_G = Byte.Parse(partColorGreenTbx.Text);
                GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_B = Byte.Parse(partColorBlueTbx.Text);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Špatné hodnoty barev částic (musí být v rozmezí 0-255)");
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
                    System.Windows.Forms.MessageBox.Show("Povolené hodnoty velikosti jsou 320-1920");
                    return;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Nastavena špatná hodnota velikosti tabulky!");
                return;
            }

            try
            {
                int interval = Int32.Parse(sendIntervalTbx.Text);
                if (interval < 100 || interval > 10000) throw new Exception();
                CaptureSettings.Instance().SEND_INTERVAL = interval;
            }
            catch
            { 
                System.Windows.Forms.MessageBox.Show("Nastavena špatná hodnota intervalu! Povolené hodnoty jsou 100-10000");
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
                System.Windows.Forms.MessageBox.Show("Nastavena špatná hodnota tolerance! Povolené hodnoty jsou 500-10000");
                return;
            }

            CaptureSettings.Instance().Save();
            GraphicsSettings.Instance().Save();

              // pokud je zmacknuty shift, okno se nezavre; jen z debugovacich duvodu
              if(!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))  this.Close();
        }

        /// <summary>
        /// Reset veskereho nastaveni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetBut_Click(object sender, RoutedEventArgs e)
        {
            CalibrationSettings.Instance().Restart();
            CaptureSettings.Instance().Restart();
            GraphicsSettings.Instance().Restart();
            PhysicSettings.Instance().Restart();
        }

    }
}
