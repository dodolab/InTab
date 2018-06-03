using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.GUI.CaptureSet;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using InteractiveTable.Managers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.Settings;

namespace InteractiveTable.Controls
{
    /// <summary>
    /// Controller pro CaptureSet - okno, ktere prirazuje konturam vlastnosti podle kamenu
    /// </summary>
    public class CaptureSetController
    {
        #region getters, setters, handlers init

        private CaptureWindow captureWindow;
        private CaptureSetManager captureManager;

        public CaptureSetManager CaptureManager
        {
            get { return captureManager; }
            set { this.captureManager = value; }
        }

        public CaptureWindow CaptureWindow
        {
            get { return captureWindow; }
            set { captureWindow = value; }
        }

        /// <summary>
        /// Nastavi GUI eventy
        /// </summary>
        public void SetHandlers()
        { 
            captureWindow.newTemplateImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(newTemplateImage_MouseUp);
            captureWindow.openTemplateImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(openTemplateImage_MouseUp);
            captureWindow.saveTemplateImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(saveTemplateImage_MouseUp);
            captureWindow.addTemplateImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(addTemplateImage_MouseUp);
            captureWindow.editTemplateImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(editTemplateImage_MouseUp);
            captureWindow.loadImageImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(loadImageImage_MouseUp);
            captureWindow.setContourImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(setContourImage_MouseUp);
            
            captureWindow.noiseSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.angleSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.blockLevelSlider.ValueChanged+=new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.contourMinAreaSlider.ValueChanged+=new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.contourMinLengthSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.dilatationSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.erodeSlider.ValueChanged+=new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.gammaSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.maxACFAverSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.minACFSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
            captureWindow.minICFSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(slider_valueChanged);
           
            captureWindow.blurCheck.Checked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.invertCheck.Checked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.adaptiveNoiseCheck.Checked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.normalizeCheck.Checked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.showAngleCheck.Checked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.showBinaryCheck.Checked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.showContourCheck.Checked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);

            captureWindow.blurCheck.Unchecked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.invertCheck.Unchecked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.adaptiveNoiseCheck.Unchecked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.normalizeCheck.Unchecked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.showAngleCheck.Unchecked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.showBinaryCheck.Unchecked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.showContourCheck.Unchecked += new System.Windows.RoutedEventHandler(checkBox_checkChanged);
            captureWindow.resolutionComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(resolutionComboBox_SelectionChanged);

            captureWindow.resetButton.Click += new System.Windows.RoutedEventHandler(resetButton_Click);

            captureWindow.Closing += new System.ComponentModel.CancelEventHandler(captureWindow_Closing);
        }

        /// <summary>
        /// Kliknuti na tlacitko RESTART restartuje veskera nastaveni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            settingsLock = true;
            CaptureSettings.Instance().Restart();
            SetDefaultValues();
            settingsLock = false;
        }

        private Boolean settingsLock = false;
        /// <summary>
        /// Nastavi defaultni hodnoty
        /// </summary>
        public void SetDefaultValues()
        {
     
            // USER SETTINGS
            captureWindow.angleSlider.Value      = CaptureSettings.Instance().DEFAULT_ROTATE_ANGLE;
            captureWindow.blockLevelSlider.Value = CaptureSettings.Instance().DEFAULT_BLOCK_LEVEL;
            captureWindow.contourMinAreaSlider.Value = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_AREA;
            captureWindow.contourMinLengthSlider.Value = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_LENGTH;
            captureWindow.dilatationSlider.Value = CaptureSettings.Instance().DEFAULT_DILATATION;
            captureWindow.erodeSlider.Value = CaptureSettings.Instance().DEFAULT_ERODE;
            captureWindow.gammaSlider.Value = CaptureSettings.Instance().DEFAULT_GAMMA;
            captureWindow.maxACFAverSlider.Value = CaptureSettings.Instance().DEFAULT_MAX_ACF;
            captureWindow.minACFSlider.Value = CaptureSettings.Instance().DEFAULT_MIN_ACF;
            captureWindow.minICFSlider.Value = CaptureSettings.Instance().DEFAULT_MIN_ICF;
            captureWindow.adaptiveNoiseCheck.IsChecked = CaptureSettings.Instance().DEFAULT_NOISE_ENABLED;
            captureWindow.noiseSlider.Value = CaptureSettings.Instance().DEFAULT_NOISE_FILTER;
            captureWindow.blurCheck.IsChecked = CaptureSettings.Instance().DEFAULT_BLUR_ENABLED;
            captureWindow.adaptiveNoiseCheck.IsChecked = (CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_PARAMETER == 1.5) ? true : false;
            captureWindow.blockLevelSlider.Value = CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_BLOCKSIZE;
            captureWindow.invertCheck.IsChecked = CaptureSettings.Instance().DEFAULT_INVERT_ENABLED;
            captureWindow.normalizeCheck.IsChecked = CaptureSettings.Instance().DEFAULT_NORMALIZE_ENABLED;

            ApplySettings();
        }

        /// <summary>
        /// Ulozeni vsech hodnot a prekresleni
        /// </summary>
        public void ApplySettings()
        {
            try
            {
                captureWindow.angleLabel.Content = captureWindow.angleSlider.Value.ToString("n0");
                captureWindow.blockLevelLabel.Content = captureWindow.blockLevelSlider.Value.ToString("n0");
                captureWindow.contourMinAreaLabel.Content = captureWindow.contourMinAreaSlider.Value.ToString("n0");
                captureWindow.contourMinLengthLabel.Content = captureWindow.contourMinLengthSlider.Value.ToString("n0");
                captureWindow.dilatationLabel.Content = captureWindow.dilatationSlider.Value.ToString("n0");
                captureWindow.erodeLabel.Content = captureWindow.erodeSlider.Value.ToString("n0");
                captureWindow.gammaLabel.Content = captureWindow.gammaSlider.Value.ToString("n2");
                captureWindow.maxACFAverLabel.Content = captureWindow.maxACFAverSlider.Value.ToString("n0");
                captureWindow.minACFLabel.Content = captureWindow.minACFSlider.Value.ToString("n2");
                captureWindow.minICFLabel.Content = captureWindow.minICFSlider.Value.ToString("n2");
                captureWindow.noiseLabel.Content = captureWindow.noiseSlider.Value.ToString("n0");

                CaptureSettings.Instance().DEFAULT_DILATATION = captureWindow.dilatationSlider.Value;
                CaptureSettings.Instance().DEFAULT_ERODE = captureWindow.erodeSlider.Value;
                CaptureSettings.Instance().DEFAULT_GAMMA = captureWindow.gammaSlider.Value;
                CaptureSettings.Instance().DEFAULT_INVERT_ENABLED = (bool)captureWindow.invertCheck.IsChecked;
                CaptureSettings.Instance().DEFAULT_NORMALIZE_ENABLED = (bool)captureWindow.normalizeCheck.IsChecked;

                captureWindow.ShowAngle = (bool)captureWindow.showAngleCheck.IsChecked;
                CaptureSettings.Instance().DEFAULT_ROTATE_ANGLE = captureWindow.angleSlider.Value;
                captureWindow.Processor.finder.maxRotateAngle = CaptureSettings.Instance().DEFAULT_ROTATE_ANGLE * Math.PI / 180;
                captureWindow.Processor.minContourArea = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_AREA = (int)captureWindow.contourMinAreaSlider.Value;
                captureWindow.Processor.minContourLength = CaptureSettings.Instance().DEFAULT_CONTOUR_MIN_LENGTH = (int)captureWindow.contourMinLengthSlider.Value;
                captureWindow.Processor.finder.maxACFDescriptorDeviation = CaptureSettings.Instance().DEFAULT_MAX_ACF = (int)captureWindow.maxACFAverSlider.Value;
                captureWindow.Processor.finder.minACF = CaptureSettings.Instance().DEFAULT_MIN_ACF = (double)captureWindow.minACFSlider.Value;
                captureWindow.Processor.finder.minICF = CaptureSettings.Instance().DEFAULT_MIN_ICF = (double)captureWindow.minICFSlider.Value;
                captureWindow.Processor.noiseFilter = CaptureSettings.Instance().DEFAULT_NOISE_ENABLED = (bool)(captureWindow.noiseSlider.Value > 0);
                captureWindow.Processor.cannyThreshold = CaptureSettings.Instance().DEFAULT_NOISE_FILTER = (int)captureWindow.noiseSlider.Value;
                captureWindow.Processor.adaptiveThresholdBlockSize = CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_BLOCKSIZE = (int)captureWindow.blockLevelSlider.Value;
                captureWindow.Processor.adaptiveThresholdParameter = CaptureSettings.Instance().DEFAULT_ADAPTIVE_THRESHOLD_PARAMETER = (bool)captureWindow.adaptiveNoiseCheck.IsChecked ? 1.5 : 0.5;
                captureWindow.Processor.blur = CaptureSettings.Instance().DEFAULT_BLUR_ENABLED = (bool)(captureWindow.blurCheck.IsChecked);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Ulozeni veskerych hodnot do Properties
        /// </summary>
        public void SaveSettings()
        {
            CaptureSettings.Instance().Save();
        }

        /// <summary>
        /// Prekresli combobox, ktery zobrazuje mozna rozliseni
        /// </summary>
        public void ApplyCamSettings()
        {
            try
            {
                captureWindow.resolutionComboBox.Text = captureWindow.CamWidth + "x" + captureWindow.CamHeight;
            }
            catch (NullReferenceException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region handler logic

        /// <summary>
        /// Ukonceni okna, ulozi nastavene hodnoty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Chcete uložit změny?", "Změna nastavení", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                SaveSettings();
                try
                {
                    //reinicializace depositoru
                    CommonAttribService.mainWindow.MyManager.TableManager.InputManager.RealTableManager.Init(CommonAttribService.DEFAULT_TEMPLATES);
                }
                catch { }
            }
            captureManager.StopThread();
        }

        /// <summary>
        /// Zmena rozliseni kamery zpusobi zmenu v nekterych nastavenich
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resolutionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!settingsLock)
            {
                captureManager.CaptureFromCam = true;
                string[] parts = captureWindow.resolutionComboBox.SelectedValue.ToString().ToLower().Split('x');
                if (parts.Length == 2)
                {
                    int camWidth = int.Parse(parts[0]);
                    int camHeight = int.Parse(parts[1]);
                    if (captureWindow.CamHeight != camHeight || captureWindow.CamWidth != camWidth)
                    {
                        captureWindow.CamWidth = camWidth;
                        captureWindow.CamHeight = camHeight;
                        ApplyCamSettings();
                    }
                }
            }
        }

        /// <summary>
        /// Zmena zaskrtnuti libovolneho checkboxu ulozi provedene zmeny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_checkChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if(!settingsLock) ApplySettings();
        }

        /// <summary>
        /// Zmena hodnot libovolneho slideru ulozi veskere zmeny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_valueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (!settingsLock) ApplySettings();
        }

        /// <summary>
        /// Kliknuti na ikonu pro nacteni obrazku nacte obrazek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadImageImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image|*.bmp;*.png;*.jpg;*.jpeg";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                try
                {
                    Image<Bgr, byte> temp = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(ofd.FileName));
                    temp = temp.Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC,true);
                    captureWindow.ImageFrame = temp;
                    captureManager.CaptureFromCam = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        /// <summary>
        /// Kliknuti na ikonu pro editaci sablon zobrazi okno se seznamem sablon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editTemplateImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { 
            new TemplateEditor(captureWindow.Processor.templates).Show();
        }

        /// <summary>
        /// Kliknuti na ikonu pro zobrazeni okna s prohlizenim kontur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTemplateImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
              if (captureWindow.Frame != null)
                  new TemplateViewer(captureWindow.Processor.templates, captureWindow.Processor.samples, captureWindow.Frame).ShowDialog();
        }

        /// <summary>
        /// Kliknuti na ikonu pro ukladani sablon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveTemplateImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Templates(*.bin)|*.bin";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = sfd.FileName;
                try
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        new BinaryFormatter().Serialize(fs, captureWindow.Processor.templates);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Kliknuti na ikonu pro prirazovani kamenum konturam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setContourImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (captureWindow.Processor.templates.Count == 0)
            {
                MessageBox.Show("Nemáte nastavené šablony!");
                return; 
            }

            ContourSetWindow cts = new ContourSetWindow();
            cts.Templates = captureWindow.Processor.templates;
            cts.InitData();
            cts.ShowDialog();
        }

        /// <summary>
        /// Kliknuti na ikonu pro nacitani sablon ze souboru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openTemplateImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Templates(*.bin)|*.bin";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = ofd.FileName;
                CaptureSettings.Instance().DEFAULT_TEMPLATE_PATH = fileName;
                 
                try
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        // POZOR! zde se zmeni defaultni cesta i defaultni sablony, pokud dojde k otevreni nove sablony!
                        Templates tmp = (Templates)new BinaryFormatter().Deserialize(fs);
                        CommonAttribService.DEFAULT_TEMPLATES = tmp;
                        captureWindow.Processor.templates = tmp;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Kliknuti na ikonu pro zalozeni nove sablony
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newTemplateImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Chcete vytvořit novou databázi šablon?", "Vytvoření nové databáze", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                captureWindow.Processor.templates.Clear();
        }

        #endregion

    }
}
