using System;
using System.Windows;
using Emgu.CV;
using Emgu.CV.Structure;
using InteractiveTable.Core.Graphics;
using System.Drawing;
using InteractiveTable.Core.Data.Capture;
using System.Windows.Forms;
using InteractiveTable.Managers;


namespace InteractiveTable.GUI.CaptureSet
{
    /// <summary>
    /// Interaction logic for CaptureSet_MainWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window 
    {
        private int camWidth = CommonAttribService.DEFAULT_CAM_WIDTH; // width of camera window
        private int camHeight = CommonAttribService.DEFAULT_CAM_HEIGHT; // height of camera window
        private Image<Bgr, Byte> frame; // camera frame, refreshed constantly
        private Image<Bgr, Byte> imageFrame; // static frame
        private int time = -1; // ms time between particular images

        // contour detector
        private ContourFilter processor;
        private Boolean showAngle; // if true, normals of vertices will be shown


        public CaptureWindow()
        {
            InitializeComponent();

            this.resolutionComboBox.Items.Add("640x480");
            this.resolutionComboBox.Items.Add("320x240");
            this.resolutionComboBox.SelectedIndex = 1;
            Processor = new ContourFilter();
        }

        /// <summary>
        /// Gets or sets time delay
        /// </summary>
        public int TimeDelay
        {
            get { return time; }
            set { this.time = value; }
        }

        /// <summary>
        /// Gets or sets indicator if angles of contours should be display
        /// </summary>
        public Boolean ShowAngle
        {
            get { return showAngle; }
            set { this.showAngle = value; }
        }
        
        public ContourFilter Processor
        {
            get { return processor; }
            set { this.processor = value; }
        }

        /// <summary>
        /// Gets or sets captured frame
        /// </summary>
        public Image<Bgr, Byte> Frame
        {
            get { return frame; }
            set { this.frame = value; }
        }

        /// <summary>
        /// Gets or sets processed image
        /// </summary>
        public Image<Bgr, Byte> ImageFrame
        {
            get { return imageFrame; }
            set { this.imageFrame = value; }
        }
        
        public int CamWidth
        {
            get { return camWidth; }
            set { this.camWidth = value; }
        }

        public int CamHeight
        {
            get { return camHeight; }
            set { this.camHeight = value; }
        }

        /// <summary>
        /// Repaints the window along with templates
        /// </summary>
        private void captureBox_Paint(object sender, PaintEventArgs e)
        {

            if (frame == null) return;
          
            Font font = new Font("Arial",24);
            System.Drawing.Brush bgBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
            System.Drawing.Brush foreBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255, 255));
            System.Drawing.Pen borderPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(150, 0, 0, 255));

            // double buffer
            Bitmap imageBuffer = new Bitmap(camWidth, camHeight);
            Graphics grBuffer = Graphics.FromImage(imageBuffer);

            if ((bool)showContourCheck.IsChecked)
                foreach (var contour in processor.contours)
                    if (contour.Total > 1) 
                        grBuffer.DrawLines(Pens.Blue, contour.ToArray());
            
            lock (processor.foundTemplates)
                foreach (FoundTemplateDesc found in processor.foundTemplates)
                {
                    // draw detected contours together with their names and bounding rectangles
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
