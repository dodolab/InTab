using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// Trida pro pocitani s vektory
    /// </summary>
    [Serializable]
    public class FVector
    {
        public FVector()
        {
        }

        private double x, y;
        /// <summary>
        /// Indikator, zda doslo ke zmene
        /// </summary>
        private bool changed = false;

        /// <summary>
        /// Velikost vektoru, prepocitava se vzdy pri zmene
        /// </summary>
        private double size;

        /// <summary>
        /// Vytvori novy vektor
        /// </summary>
        /// <param name="x">hodnota X</param>
        /// <param name="y">hodnota Y</param>
        public FVector(double x, double y)
        {
            this.X = x;
            this.Y = y;
            size = Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Vrati nebo nastavi hodnotu X
        /// </summary>
        public double X
        {
            get { return x; }
            set
            {
                this.x = value;
                changed = true;
            }
        }

        /// <summary>
        /// Vrati nebo nastavi hodnotu Y
        /// </summary>
        public double Y
        {
            get { return y; }
            set
            {
                this.y = value;
                changed = true;
            }
        }

        /// <summary>
        /// Pricte k vektoru hodnotu X a Y
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void Add(double X, double Y)
        {
            this.X += X;
            this.Y += Y;
            changed = true;
        }

        /// <summary>
        /// Secte vektor s vektorem other
        /// </summary>
        /// <param name="other"></param>
        public void Add(FVector other)
        {
            this.X += other.X;
            this.Y += other.Y;
            changed = true;
        }

        /// <summary>
        /// Nastavi hodnoty vektoru X a Y
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void Set(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            changed = true;
        }

        /// <summary>
        /// Nastavi vektor podle vektoru other
        /// </summary>
        /// <param name="other"></param>
        public void Set(FVector other)
        {
            this.X = other.X;
            this.Y = other.Y;
            changed = true;
        }

        /// <summary>
        /// Vynasobi vektor hodnotami X a Y
        /// </summary>
        /// <param name="valueX"></param>
        /// <param name="valueY"></param>
        public void Mult(double valueX, double valueY)
        {
            this.X *= valueX;
            this.Y *= valueY;
            changed = true;
        }

        /// <summary>
        /// Prevrati hodnoty X a Y
        /// </summary>
        public void Invert()
        {
            if (X != 0) this.X = 1 / X;
            if (Y != 0) this.Y = 1 / Y;
            changed = true;
        }

        /// <summary>
        /// Vrati velikost vektoru
        /// </summary>
        /// <returns></returns>
        public double Size()
        {
            if (changed)
            {
                size = Math.Sqrt(x * x + y * y);
                changed = false;
            }
            return size;
        }
    }
}
