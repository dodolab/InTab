using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// Komplexni cisla se vsemi operacemi
    /// </summary>
    [Serializable]
    public struct Complex
    {
        /// <summary>
        /// Realne slozka
        /// </summary>
        public double a;

        /// <summary>
        /// Komplexni slozka
        /// </summary>
        public double b;

        /// <summary>
        /// Vytvori nove komplexni cislo
        /// </summary>
        /// <param name="a">Realna slozka</param>
        /// <param name="b">Komplexni slozka</param>
        public Complex(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// Vytvori komplexni cislo z polomeru a uhlu
        /// </summary>
        /// <param name="r">dany polomer</param>
        /// <param name="angle">uhel</param>
        /// <returns></returns>
        public static Complex FromExp(double r, double angle)
        { // jdi do prdele
            return new Complex(r * Math.Cos(angle), r * Math.Sin(angle));
        }

        /// <summary>
        /// Vrati uhel fi
        /// </summary>
        public double Angle
        {
            get
            {
                return Math.Atan2(b, a);
            }
        }

        /// <summary>
        /// Vypise komplexni cislo
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return a + "+i" + b;
        }

        /// <summary>
        /// Vrati absolutni hodnotu komplexniho cisla
        /// </summary>
        public double Norma
        {
            get { return Math.Sqrt(a * a + b * b); }
        }
        /// <summary>
        /// Vrati neodmocnenou absolutni hodnotu
        /// </summary>
        public double NormaSquare
        {
            get { return a * a + b * b; }
        }

        /// <summary>
        /// Operator pro scitani komplexnich cisel
        /// </summary>
        /// <param name="x1">prvni cislo</param>
        /// <param name="x2">druhe cislo</param>
        /// <returns></returns>
        public static Complex operator +(Complex x1, Complex x2)
        {
            return new Complex(x1.a + x2.a, x1.b + x2.b);
        }

        /// <summary>
        /// Operator pro nasobeni komplexnich cisel skalarem
        /// </summary>
        /// <param name="k">skalar</param>
        /// <param name="x">komplexni cislo</param>
        /// <returns></returns>
        public static Complex operator *(double k, Complex x)
        {
            return new Complex(k * x.a, k * x.b);
        }

        /// <summary>
        /// Operator pro nasobeni komplexnich cisel skalarem
        /// </summary>
        /// <param name="x">komplexni cislo</param>
        /// <param name="k">skalar</param>
        /// <returns></returns>
        public static Complex operator *(Complex x, double k)
        {
            return new Complex(k * x.a, k * x.b);
        }

        /// <summary>
        /// Operator pro vektorovy soucin komplexnich cisel
        /// </summary>
        /// <param name="x1">prvni cislo</param>
        /// <param name="x2">druhe cislo</param>
        /// <returns></returns>
        public static Complex operator *(Complex x1, Complex x2)
        {
            return new Complex(x1.a * x2.a - x1.b * x2.b, x1.b * x2.a + x1.a * x2.b);
        }

        /// <summary>
        /// Vrati kosunis uhlu fi
        /// </summary>
        /// <returns></returns>
        public double CosAngle()
        {
            return a / Math.Sqrt(a * a + b * b);
        }

        /// <summary>
        /// Rotace komplexniho cisla podle kosinove a sinove hodnoty uhlu 
        /// </summary>
        /// <param name="CosAngle">cosinus uhlu</param>
        /// <param name="SinAngle">sinus uhlu</param>
        /// <returns></returns>
        public Complex Rotate(double CosAngle, double SinAngle)
        {
            return new Complex(CosAngle * a - SinAngle * b, SinAngle * a + CosAngle * b);
        }

        /// <summary>
        /// Rotace komplexniho cisla podle uhlu v radianech
        /// </summary>
        /// <param name="Angle">uhel v radianech</param>
        /// <returns></returns>
        public Complex Rotate(double Angle)
        {
            var CosAngle = Math.Cos(Angle);
            var SinAngle = Math.Sin(Angle);
            return new Complex(CosAngle * a - SinAngle * b, SinAngle * a + CosAngle * b);
        }
    }

}
