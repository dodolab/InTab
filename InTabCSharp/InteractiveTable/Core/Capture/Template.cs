using System;
using System.Collections.Generic;
using System.Drawing;
using InteractiveTable.Core.Data.Deposit;

namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Trida reprezentujici sablony skladajici se z kontury a dalsich vlastnosti
    /// </summary>
    [Serializable]
    public class Template
    {
        /// <summary>
        /// Jmeno sablony
        /// </summary>
        public string name; 
        /// <summary>
        /// Kontura sablony
        /// </summary>
        public Contour contour;
        /// <summary>
        /// Korelacni kontura sablony
        /// </summary>
        public Contour autoCorr; 
        /// <summary>
        /// Prvni vektor kontury
        /// </summary>
        public Point startPoint; 
        /// <summary>
        /// Pokud true, budou se uvazovat vsechny uhly
        /// </summary>
        public bool preferredAngleNoMore90 = false;

        /// <summary>
        /// Prvni deskriptor auto-korelace
        /// </summary>
        public int autoCorrDescriptor1;
        /// <summary>
        /// Druhy deskriptor auto-korelace
        /// </summary>
        public int autoCorrDescriptor2;
        /// <summary>
        /// Treti deskriptor auto-korelace
        /// </summary>
        public int autoCorrDescriptor3;
        /// <summary>
        /// Ctvrty deskriptor auto-korelace
        /// </summary>
        public int autoCorrDescriptor4;
        /// <summary>
        /// Normala kontury
        /// </summary>
        public double contourNorma;
        /// <summary>
        /// Obsah konvexni obalky
        /// </summary>
        public double sourceArea; 
        /// <summary>
        /// Pridruzeny objekt sablony, neni serializovan
        /// </summary>
        [NonSerialized]
        public object tag;
        static int[] filter1 = { 1, 1, 1, 1 };
        static int[] filter2 = { -1, -1, 1, 1 };
        static int[] filter3 = { -1, 1, 1, -1 };
        static int[] filter4 = { -1, 1, -1, 1 };

        /// <summary>
        /// Vytvori novou sablonu
        /// </summary>
        /// <param name="points">pole vektoru</param>
        /// <param name="sourceArea">velikost</param>
        /// <param name="templateSize">velikost pole kontury, muze byt jine nez velikost points</param>
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
        /// Spocita auto-korelaci
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
        /// Vykresli konturu do objektu gr
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

            //vykresli konturu
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
                points[i] = new Point(r.Left + ddx + (int)((points[i].X - contour.SourceBoundingRect.Left - dx) * k), r.Top + ddy + (int)((points[i].Y - contour.SourceBoundingRect.Top - dy) * k));
            gr.DrawPolygon(Pens.Red, points);
        }
    }

    /// <summary>
    /// Seznam sablon; kazda sablona obsahuje nastaveni daneho kamene
    /// </summary>
    [Serializable]
    public class Templates : List<Template>
    {
        public int templateSize = 30;
        public HashSet<ContourRock> rockSettings = new HashSet<ContourRock>();
    }
}
