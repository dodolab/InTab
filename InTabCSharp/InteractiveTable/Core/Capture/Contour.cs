using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using InteractiveTable.Accessories;

namespace InteractiveTable.Core.Data.Capture
{
        /// <summary>
        /// Trida uchovavajici konturu se vsemi informacemi
        /// </summary>
        [Serializable]
        public class Contour
        {
            #region vars, constructors, get-setters

            Complex[] array; // pole komplexnich cisel, predstavuji vektory uzavrene smycky
            public Rectangle SourceBoundingRect; // obdelnik konvexni obalky



            protected Contour()
            {
            }

            /// <summary>
            /// Vytvori novou konturu
            /// </summary>
            /// <param name="capacity">velikost pole vektoru pro danou konturu</param>
            public Contour(int capacity)
            {
                array = new Complex[capacity];
            }

            /// <summary>
            /// Vytvori novou konturu s danym seznamem bodu
            /// </summary>
            /// <param name="points"></param>
            public Contour(IList<Point> points)
                : this(points, 0, points.Count)
            {
            }

            /// <summary>
            /// Vytvori novou konturu s danym seznamem bodu a definovanym pocatkem
            /// </summary>
            /// <param name="points">pole bodu</param>
            /// <param name="startIndex">pocatecni index</param>
            /// <param name="count">pocet vrcholu</param>
            public Contour(IList<Point> points, int startIndex, int count)
                : this(count)
            {
                // minima, deklarovana pocatecnim indexem
                int minX = points[startIndex].X; // minimum v ose X
                int minY = points[startIndex].Y; // minimum v ose Y
                int maxX = minX; // maximum v ose X
                int maxY = minY; // maximum v ose Y
                int endIndex = startIndex + count; // konecny index

                /*
                 * projde vsechny vrcholy a nalezne minimum a maximum v obou osach
                 */
                for (int i = startIndex; i < endIndex; i++)
                {
                    var p1 = points[i];
                    var p2 = (i == endIndex - 1) ? points[startIndex] : points[i + 1];
                    array[i] = new Complex(p2.X - p1.X, -p2.Y + p1.Y);

                    if (p1.X > maxX) maxX = p1.X;
                    if (p1.X < minX) minX = p1.X;
                    if (p1.Y > maxY) maxY = p1.Y;
                    if (p1.Y < minY) minY = p1.Y;
                }

                // vytvoreni konvexni obalky kontury
                SourceBoundingRect = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            }

            /// <summary>
            /// Vrati pocet vektoru v konture
            /// </summary>
            public int Count
            {
                get
                {
                    return array.Length;
                }
            }

            /// <summary>
            /// Pretizeny deklarator [], vraci vektor na indexu i
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public Complex this[int i]
            {
                get { return array[i]; }
                set { array[i] = value; }
            }

            /// <summary>
            /// Vytvori kopii kontury
            /// </summary>
            /// <returns></returns>
            public Contour Clone()
            {
                Contour result = new Contour();
                result.array = (Complex[])array.Clone();
                return result;
            }

            /// <summary>
            /// Vrati polomerovy rozdil s konturou C
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public double DiffR2(Contour c)
            {
                double max1 = 0;
                double max2 = 0;
                double sum = 0;

                // pro vsechny vrcholy spocti absolutni hodnotu THIS kontury a kontury C a spocti rozdil
                for (int i = 0; i < Count; i++)
                {
                    double v1 = array[i].Norma;
                    double v2 = c.array[i].Norma;
                    if (v1 > max1) max1 = v1;
                    if (v2 > max2) max2 = v2;
                    double v = v1 - v2;
                    sum += v * v;
                }
                double max = Math.Max(max1, max2);
                return 1 - sum / Count / max / max;
            }


            /// <summary>
            /// Vrati absolutni hodnotu souctu ctvercu absolutnich hodnot vsech vrcholu
            /// </summary>
            public double Norma
            {
                get
                {
                    double result = 0;
                    foreach (var c in array)
                        result += c.NormaSquare;
                    return Math.Sqrt(result);
                }
            }

            /// <summary>
            /// Vrati vsechny vrcholy razeny od pocatku
            /// </summary>
            /// <param name="startPoint"></param>
            /// <returns></returns>
            public Point[] GetPoints(Point startPoint)
            {
                Point[] result = new Point[Count + 1];
                PointF sum = startPoint;
                result[0] = Point.Round(sum);
                for (int i = 0; i < Count; i++)
                {
                    sum = sum.Offset((float)array[i].a, -(float)array[i].b);
                    result[i + 1] = Point.Round(sum);
                }

                return result;
            }

            /// <summary>
            /// Vrati konvexni obalku
            /// </summary>
            /// <returns></returns>
            public RectangleF GetBoundsRect()
            {
                double minX = 0, maxX = 0, minY = 0, maxY = 0;
                double sumX = 0, sumY = 0;
                for (int i = 0; i < Count; i++)
                {
                    var v = array[i];
                    sumX += v.a;
                    sumY += v.b;
                    if (sumX > maxX) maxX = sumX;
                    if (sumX < minX) minX = sumX;
                    if (sumY > maxY) maxY = sumY;
                    if (sumY < minY) minY = sumY;
                }

                return new RectangleF((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
            }

            #endregion

            #region functions


            /// <summary>
            /// Vrati oblast obaloveho obdelniku
            /// </summary>
            /// <returns></returns>
            public int SourceBoundingRectArea()
            {
                return SourceBoundingRect.Area();
            }

            /// <summary>
            /// Skalarni nasobek kontury
            /// </summary>
            public unsafe Complex Dot(Contour c, int shift)
            {
                var count = Count;
                double sumA = 0;
                double sumB = 0;
                // fixovani ukazatelu -> .NET nebude moci zmenit objekt, na ktery ukazuje
                fixed (Complex* ptr1 = &array[0])
                fixed (Complex* ptr2 = &c.array[shift])
                fixed (Complex* ptr22 = &c.array[0])
                fixed (Complex* ptr3 = &c.array[c.Count - 1])
                {
                    Complex* p1 = ptr1;
                    Complex* p2 = ptr2;
                    // pro vsechny vrcholy vynasob prislusne vrcholu s posunem SHIFT
                    for (int i = 0; i < count; i++)
                    {
                        Complex x1 = *p1;
                        Complex x2 = *p2;
                        sumA += x1.a * x2.a + x1.b * x2.b;
                        sumB += x1.b * x2.a - x1.a * x2.b;

                        p1++;
                        if (p2 == ptr3)
                            p2 = ptr22;
                        else
                            p2++;
                    }
                }
                return new Complex(sumA, sumB);
            }

            /// <summary>
            /// Interkorelace ICF s konturou C, resp. podobnost s konturou C
            /// vystupem je kontura podobnosti
            /// </summary>
            public Contour InterCorrelation(Contour c)
            {
                int count = Count;
                Contour result = new Contour(count);
                for (int i = 0; i < count; i++)
                    result.array[i] = Dot(c, i);

                return result;
            }

            /// <summary>
            /// Interkorelace ICF s konturou C posunutou nejvyse o maxShift
            /// </summary>
            public Contour InterCorrelation(Contour c, int maxShift)
            {
                Contour result = new Contour(maxShift);
                int i = 0;
                int count = Count;
                while (i < maxShift / 2)
                {
                    result.array[i] = Dot(c, i);
                    result.array[maxShift - i - 1] = Dot(c, c.Count - i - 1);
                    i++;
                }
                return result;
            }


            /// <summary>
            /// Autokorelacni funkce ACF, vyjadruje podobnost kontury se sebou samou,
            /// vyjadruje simetrii kontury
            /// </summary>
            public unsafe Contour AutoCorrelation(bool normalize)
            {
                int count = Count / 2;
                Contour result = new Contour(count);
                fixed (Complex* ptr = &result.array[0])
                {
                    Complex* p = ptr;
                    double maxNormaSq = 0;
                    for (int i = 0; i < count; i++)
                    {
                        *p = Dot(this, i);
                        double normaSq = (*p).NormaSquare;
                        if (normaSq > maxNormaSq)
                            maxNormaSq = normaSq;
                        p++;
                    }
                    if (normalize)
                    {
                        maxNormaSq = Math.Sqrt(maxNormaSq);
                        p = ptr;
                        for (int i = 0; i < count; i++)
                        {
                            *p = new Complex((*p).a / maxNormaSq, (*p).b / maxNormaSq);
                            p++;
                        }
                    }
                }

                return result;
            }

            /// <summary>
            /// Normalizuje konturu
            /// </summary>
            public void Normalize()
            {
                // najdi nejvyssi absolutni hodnotu
                double max = FindMaxNorma().Norma;
                if (max > double.Epsilon)
                    Scale(1 / max);
            }

            /// <summary>
            /// Najde nejvyssi absolutni hodnotu
            /// </summary>
            public Complex FindMaxNorma()
            {
                double max = 0d;
                Complex res = default(Complex);
                foreach (var c in array)
                    if (c.Norma > max)
                    {
                        max = c.Norma;
                        res = c;
                    }
                return res;
            }


            /// <summary>
            /// Skalarni soucin s konturou C bez shiftu
            /// </summary>
            public Complex Dot(Contour c)
            {
                return Dot(c, 0);
            }

     
            /// <summary>
            /// Zvetsi konturu o parametr scale
            /// </summary>
            /// <param name="scale"></param>
            public void Scale(double scale)
            {
                for (int i = 0; i < Count; i++)
                    this[i] = scale * this[i];
            }

            /// <summary>
            /// Znasobi konturu komplexnim cislem
            /// </summary>
            /// <param name="c"></param>
            public void Mult(Complex c)
            {
                for (int i = 0; i < Count; i++)
                    this[i] = c * this[i];
            }

            /// <summary>
            /// Orotuje konturu o dany uhel
            /// </summary>
            /// <param name="angle"></param>
            public void Rotate(double angle)
            {
                var cosA = Math.Cos(angle);
                var sinA = Math.Sin(angle);
                for (int i = 0; i < Count; i++)
                    this[i] = this[i].Rotate(cosA, sinA);
            }

            /// <summary>
            /// Normalizovany skalarni soucin
            /// </summary>
            public Complex NormDot(Contour c)
            {
                var count = this.Count;
                double sumA = 0;
                double sumB = 0;
                double norm1 = 0;
                double norm2 = 0;
                for (int i = 0; i < count; i++)
                {
                    var x1 = this[i];
                    var x2 = c[i];
                    sumA += x1.a * x2.a + x1.b * x2.b;
                    sumB += x1.b * x2.a - x1.a * x2.b;
                    norm1 += x1.NormaSquare;
                    norm2 += x2.NormaSquare;
                }

                double k = 1d / Math.Sqrt(norm1 * norm2);
                return new Complex(sumA * k, sumB * k);
            }

            /// <summary>
            /// Diskretni furierova transformace
            /// </summary>
            public Contour Fourier()
            {
                int count = Count;
                Contour result = new Contour(count);
                for (int m = 0; m < count; m++)
                {
                    Complex sum = new Complex(0, 0);
                    double k = -2d * Math.PI * m / count;
                    for (int n = 0; n < count; n++)
                        sum += this[n].Rotate(k * n);

                    result.array[m] = sum;
                }

                return result;
            }

            /// <summary>
            /// Vrati vzdalenost od kontury C
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public double Distance(Contour c)
            {
                double n1 = this.Norma;
                double n2 = c.Norma;
                return n1 * n1 + n2 * n2 - 2 * (this.Dot(c).a);
            }

            /// <summary>
            /// Zmena rozliseni kontury (pole vektoru)
            /// </summary>
            public void Equalization(int newCount)
            {
                if (newCount > Count)
                    EqualizationUp(newCount);
                else
                    EqualizationDown(newCount);
            }

            /// <summary>
            /// Zvysi pole vektoru definujici konturu a provede interpolaci
            /// </summary>
            /// <param name="newCount"></param>
            private void EqualizationUp(int newCount)
            {
                Complex currPoint = this[0];
                Complex[] newPoint = new Complex[newCount];

                for (int i = 0; i < newCount; i++)
                {
                    double index = 1d * i * Count / newCount;
                    int j = (int)index;
                    double k = index - j;
                    if (j == Count - 1)
                        newPoint[i] = this[j];
                    else
                        newPoint[i] = this[j] * (1 - k) + this[j + 1] * k;
                }

                array = newPoint;
            }

            /// <summary>
            /// Zmensi rozliseni kontury
            /// </summary>
            /// <param name="newCount"></param>
            private void EqualizationDown(int newCount)
            {
                Complex currPoint = this[0];
                Complex[] newPoint = new Complex[newCount];

                for (int i = 0; i < Count; i++)
                    newPoint[i * newCount / Count] += this[i];

                array = newPoint;
            }

            

            #endregion
        }
}
