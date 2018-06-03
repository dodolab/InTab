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
using Emgu.CV;
using Emgu.CV.Structure;
using InteractiveTable.Core.Graphics;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using InteractiveTable.Core.Data.Capture;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using InteractiveTable.Settings;
using InteractiveTable.Managers;


namespace InteractiveTable.GUI.CaptureSet
{
    /// <summary>
    /// Interaction logic for CaptureSet_MainWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window 
    {
        private int camWidth = CommonAttribService.DEFAULT_CAM_WIDTH; // sirka okna vstupu kamery
        private int camHeight = CommonAttribService.DEFAULT_CAM_HEIGHT; // vyska okna vstupu kamery
        private Image<Bgr, Byte> frame; // obrazek od kamery -> stale se meni
        private Image<Bgr, Byte> imageFrame; // obrazek nacteny -> nemeni se
        private int time = -1; // cas v milisekundach mezi jednotlivymi snimky

        // procesor, ktery slouzi k nalezeni a filtrovani kontur
        private ContourFilter processor;
        private Boolean showAngle; // pokud true, budou se u rozpoznavani vzoru ukazovat normaly vektoru


        public CaptureWindow()
        {
            InitializeComponent();

            this.resolutionComboBox.Items.Add("640x480");
            this.resolutionComboBox.Items.Add("320x240");
            this.resolutionComboBox.SelectedIndex = 1;
            Processor = new ContourFilter();
        }

        /// <summary>
        /// Vrati nebo nastavi casove zpozdeni
        /// </summary>
        public int TimeDelay
        {
            get { return time; }
            set { this.time = value; }
        }

        /// <summary>
        /// Pokud true, zobrazi na obrazovku uhly kontur
        /// </summary>
        public Boolean ShowAngle
        {
            get { return showAngle; }
            set { this.showAngle = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi processor filtrace kontur
        /// </summary>
        public ContourFilter Processor
        {
            get { return processor; }
            set { this.processor = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi obrazek z kamery
        /// </summary>
        public Image<Bgr, Byte> Frame
        {
            get { return frame; }
            set { this.frame = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi zpracovany obrazek
        /// </summary>
        public Image<Bgr, Byte> ImageFrame
        {
            get { return imageFrame; }
            set { this.imageFrame = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi sirku kamery
        /// </summary>
        public int CamWidth
        {
            get { return camWidth; }
            set { this.camWidth = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi vysku kamery
        /// </summary>
        public int CamHeight
        {
            get { return camHeight; }
            set { this.camHeight = value; }
        }

        /// <summary>
        /// Prekreslovani okna pro zobrazovani vstupu kamery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureBox_Paint(object sender, PaintEventArgs e)
        {

            if (frame == null) return;
          
            Font font = new Font("Arial",24);//16  
            // vizualni styl kontur, popisku atd.
            System.Drawing.Brush bgBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
            System.Drawing.Brush foreBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255, 255));
            System.Drawing.Pen borderPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(150, 0, 0, 255));

            // double buffer -> vykresleni probehne, az kdyz je vykresleno vse, co je potreba
            Bitmap imageBuffer = new Bitmap(camWidth, camHeight);
            Graphics grBuffer = Graphics.FromImage(imageBuffer);

            if ((bool)showContourCheck.IsChecked)
                foreach (var contour in processor.contours)
                    if (contour.Total > 1) // vykresleni detekovanych kontur
                        grBuffer.DrawLines(Pens.Blue, contour.ToArray());
            
            lock (processor.foundTemplates)
                foreach (FoundTemplateDesc found in processor.foundTemplates)
                {
                    // vykresleni ROZPOZNANYCH kontur i s jejich nazvem, vykresli obalovy obdelnik
                    System.Drawing.Rectangle foundRect = found.sample.contour.SourceBoundingRect;
                    System.Drawing.Point p1 = new System.Drawing.Point((foundRect.Left + foundRect.Right) / 2, foundRect.Top);
                    string text = found.template.name;
                    if (showAngle)
                        text += string.Format("\r\nangle={0:000}°\r\nscale={1:0.0}", 180 * found.angle / Math.PI, found.scale);
                    grBuffer.DrawRectangle(borderPen, foundRect);
                    grBuffer.DrawString(text, font, bgBrush, new PointF(p1.X + 1 - font.Height / 3, p1.Y + 1 - font.Height));
                    grBuffer.DrawString(text, font, foreBrush, new PointF(p1.X - font.Height / 3, p1.Y - font.Height));
                }
            string timeTxt = time.ToString() + " ms; " + ((int)1000.0 / time).ToString() + " FPS";
            grBuffer.DrawString(timeTxt, font, foreBrush, new PointF(5, 5));
            grBuffer.Dispose();
            e.Graphics.DrawImage(imageBuffer, 0, 0);
        }


    }
}
