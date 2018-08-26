using System;
using System.Collections.Generic;
using System.Drawing;

namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Template consisting of contour and additional attributse
    /// </summary>
    [Serializable]
    public class Template
    {
        /// <summary>
        /// Name
        /// </summary>
        public string name; 
        /// <summary>
        /// Contour
        /// </summary>
        public Contour contour;
        /// <summary>
        /// Correlation contour
        /// </summary>
        public Contour autoCorr; 
        /// <summary>
        /// First point of the contour
        /// </summary>
        public Point startPoint; 
        /// <summary>
        /// If true, all angles will be taken into account
        /// </summary>
        public bool preferredAngleNoMore90 = false;

        /// <summary>
        /// First descriptor of autocorrelation
        /// </summary>
        public int autoCorrDescriptor1;
        /// <summary>
        /// Second descriptor of autocorrelation
        /// </summary>
        public int autoCorrDescriptor2;
        /// <summary>
        /// Third descriptor of autocorrelation
        /// </summary>
        public int autoCorrDescriptor3;
        /// <summary>
        /// Fourth descriptor of autocorrelation
        /// </summary>
        public int autoCorrDescriptor4;
        /// <summary>
        /// Contour norm
        /// </summary>
        public double contourNorma;
        /// <summary>
        /// Area of convex envelope
        /// </summary>
        public double sourceArea; 

        /// <summary>
        /// Data of assigned template
        /// </summary>
        [NonSerialized]
        public object tag;
        static int[] filter1 = { 1, 1, 1, 1 };
        static int[] filter2 = { -1, -1, 1, 1 };
        static int[] filter3 = { -1, 1, 1, -1 };
        static int[] filter4 = { -1, 1, -1, 1 };

        /// <summary>
        /// Creates a new template
        /// </summary>
        /// <param name="points">array of vertices</param>
        /// <param name="sourceArea">size of the envelope</param>
        /// <param name="templateSize">size of an array of the contour, might by different than points.Length</param>
        public Template(Point[] points, double sourceArea, int templateSize)
        {
            this.sourceArea = sourceArea;
            startPoint = points[0];
            contour = new Contour(points);
            contour.Equalization(templateSize);
            contourNorma = contour.Norma;
            autoCorr = contour.AutoCorrelation(true);

            CalcAutoCorrDescriptions();
        }


        /// <summary>
        /// Calculates auto-correlation
        /// </summary>
        public void CalcAutoCorrDescriptions()
        {
            int count = autoCorr.Count;
            double sum1 = 0;
            double sum2 = 0;
            double sum3 = 0;
            double sum4 = 0;
            for (int i = 0; i < count; i++)
            {
                double v = autoCorr[i].Norma;
                int j = 4 * i / count;

                sum1 += filter1[j] * v;
                sum2 += filter2[j] * v;
                sum3 += filter3[j] * v;
                sum4 += filter4[j] * v;
            }

            autoCorrDescriptor1 = (int)(100 * sum1 / count);
            autoCorrDescriptor2 = (int)(100 * sum2 / count);
            autoCorrDescriptor3 = (int)(100 * sum3 / count);
            autoCorrDescriptor4 = (int)(100 * sum4 / count);
        }

        /// <summary>
        /// Renders the contour into a graphic object
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="rect"></param>
        public void Draw(System.Drawing.Graphics gr, Rectangle rect)
        {
            gr.DrawRectangle(Pens.SteelBlue, rect);
            rect = new Rectangle(rect.Left, rect.Top, rect.Width - 24, rect.Height);

            var contour = this.contour.Clone();
            var autoCorr = this.autoCorr.Clone();
            autoCorr.Normalize();

            Rectangle r = new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height);
            r.Inflate(-20, -20);
            var points = contour.GetPoints(startPoint);
            Rectangle boundRect = Rectangle.Round(contour.GetBoundsRect());

            double w = boundRect.Width;
            double h = boundRect.Height;
            float k = (float)Math.Min(r.Width / w, r.Height / h);
            int dx = startPoint.X - contour.SourceBoundingRect.Left;
            int dy = startPoint.Y - contour.SourceBoundingRect.Top;
            int ddx = -(int)(boundRect.Left * k);
            int ddy = (int)(boundRect.Bottom * k);
            for (int i = 0; i < points.Length; i++)
                points[i] = new Point(r.Left + ddx + (int)((points[i].X - contour.SourceBoundingRect.Left - dx) * k), r.Top + ddy + 
                    (int)((points[i].Y - contour.SourceBoundingRect.Top - dy) * k));
            gr.DrawPolygon(Pens.Red, points);
        }
    }

    [Serializable]
    public class Templates : List<Template>
    {
        public int templateSize = 30;
        public HashSet<ContourRock> rockSettings = new HashSet<ContourRock>();
    }
}
