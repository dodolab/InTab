using System;
using System.Windows;
using Emgu.CV;
using Emgu.CV.Structure;
using InteractiveTable.Core.Graphics;
using InteractiveTable.Core.Data.Capture;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// Simulator window that displays the table and recognized stones
    /// </summary>
    public partial class InteractiveWindow : Window
    {
        private double width = 400; // size for dispatcher
        private double height = 300;

        public InteractiveWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Resizes captured image and renders it along with all contours
        /// </summary>
        /// <param name="tempFrame">captured image</param>
        /// <param name="processor">processor with detected contours</param>
        public void DrawImage(Image<Bgr,byte> tempFrame, ContourFilter processor) {
            // Save the variables asynchronously
            Dispatcher.BeginInvoke(new Action(() =>
            {
                width = Width;
                height = Height;
            }));

            tempFrame = tempFrame.Resize((int)width, (int)(height - 20), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            DrawConturs(processor,tempFrame.Bitmap); 
        }

        /// <summary>
        /// Renders a captured image along with recognized contours
        /// </summary>
        private void DrawConturs(ContourFilter processor, System.Drawing.Bitmap imageBuffer)
        {
            System.Drawing.Font font = new System.Drawing.Font("Arial", 24);
            // visual style of contours and labels
            System.Drawing.Brush bgBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
            System.Drawing.Brush foreBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255, 255));
            System.Drawing.Pen borderPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 0, 0, 255));
            borderPen.Width = 5;
           
            // we use double buffer -> the rendering will take its place once the image is ready
            System.Drawing.Graphics grBuffer = System.Drawing.Graphics.FromImage(imageBuffer);
            grBuffer.ScaleTransform((float)(width / 640), (float)((height - 20) / 480));
            foreach (FoundTemplateDesc found in processor.foundTemplates)
            {
                // draw detected contours along with their name and wrapping rectangle
                System.Drawing.Rectangle foundRect = found.sample.contour.SourceBoundingRect;
                System.Drawing.Point p1 = new System.Drawing.Point((foundRect.Left + foundRect.Right) / 2, foundRect.Top);
                string text = found.template.name;
                
                grBuffer.DrawRectangle(borderPen, foundRect);
                grBuffer.DrawString(text, font, bgBrush, new System.Drawing.PointF(p1.X + 1 - font.Height / 3, p1.Y + 1 - font.Height));
                grBuffer.DrawString(text, font, foreBrush, new System.Drawing.PointF(p1.X - font.Height / 3, p1.Y - font.Height));
            }
            grBuffer.Dispose();
            captureBox.CreateGraphics().DrawImage(imageBuffer, 0, 0);
        }

    }
}
