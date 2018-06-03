using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InteractiveTable.Managers;
using Emgu.CV;
using Emgu.CV.Structure;
using InteractiveTable.Core.Graphics;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.Capture;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// Interaktivni okno, zobrazuje stul s rozpoznanymi kameny
    /// </summary>
    public partial class InteractiveWindow : Window
    {
        private double width = 400; // rozmery pro dispatcher
        private double height = 300;

        public InteractiveWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Zmensi zachyceny obrazek a vykresli ho spolu s konturami
        /// </summary>
        /// <param name="tempFrame">zachyceny obrazek</param>
        /// <param name="processor">processor s rozpoznanymi konturami</param>
        public void DrawImage(Image<Bgr,byte> tempFrame, ContourFilter processor){
            // ulozeni promennych asynchronne
            Dispatcher.BeginInvoke(new Action(() =>
            {
                width = Width;
                height = Height;
            }));

            tempFrame = tempFrame.Resize((int)width, (int)(height - 20), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);


            DrawConturs(processor,tempFrame.Bitmap); 
        }

        /// <summary>
        /// Vykresli zachyceny obrazek vcetne rozpoznanych kontur
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="imageBuffer"></param>
        private void DrawConturs(ContourFilter processor, System.Drawing.Bitmap imageBuffer)
        {
            System.Drawing.Font font = new System.Drawing.Font("Arial", 24); //16  
            // vizualni styl kontur, popisku atd.
            System.Drawing.Brush bgBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
            System.Drawing.Brush foreBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255, 255));
            System.Drawing.Pen borderPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 0, 0, 255));
            borderPen.Width = 5;
           
            // double buffer -> vykresleni probehne, az kdyz je vykresleno vse, co je potreba

           // System.Drawing.Bitmap imageBuffer = new System.Drawing.Bitmap(ApiSettings.DEFAULT_TABLE_WIDTH, ApiSettings.DEFAULT_TABLE_HEIGHT);
            System.Drawing.Graphics grBuffer = System.Drawing.Graphics.FromImage(imageBuffer);
            grBuffer.ScaleTransform((float)(width / 640), (float)((height - 20) / 480));
            foreach (FoundTemplateDesc found in processor.foundTemplates)
            {
                // vykresleni ROZPOZNANYCH kontur i s jejich nazvem, vykresli obalovy obdelnik
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
