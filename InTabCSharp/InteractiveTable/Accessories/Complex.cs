using System;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// Complex numbers with all operations
    /// </summary>
    [Serializable]
    public struct Complex
    {
        /// <summary>
        /// Real part
        /// </summary>
        public double a;

        /// <summary>
        /// Complex part
        /// </summary>
        public double b;

        /// <summary>
        /// Creates a new complex number
        /// </summary>
        /// <param name="a">Real part</param>
        /// <param name="b">Complex part</param>
        public Complex(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// Creates a new complex number from a radius and an angle
        /// </summary>
        /// <param name="r">radius</param>
        /// <param name="angle">angle</param>
        /// <returns></returns>
        public static Complex FromExp(double r, double angle)
        { 
            return new Complex(r * Math.Cos(angle), r * Math.Sin(angle));
        }

        /// <summary>
        /// Returns an angle
        /// </summary>
        public double Angle
        {
            get
            {
                return Math.Atan2(b, a);
            }
        }

        /// <summary>
        /// Renders a complex number
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return a + "+i" + b;
        }

        /// <summary>
        /// Returns an absolute value of the complex number
        /// </summary>
        public double Norma
        {
            get { return Math.Sqrt(a * a + b * b); }
        }
        /// <summary>
        /// Returns an absolute value of the complex number, not squared
        /// </summary>
        public double NormaSquare
        {
            get { return a * a + b * b; }
        }
        
        public static Complex operator +(Complex x1, Complex x2)
        {
            return new Complex(x1.a + x2.a, x1.b + x2.b);
        }

        public static Complex operator *(double k, Complex x)
        {
            return new Complex(k * x.a, k * x.b);
        }

        public static Complex operator *(Complex x, double k)
        {
            return new Complex(k * x.a, k * x.b);
        }

        public static Complex operator *(Complex x1, Complex x2)
        {
            return new Complex(x1.a * x2.a - x1.b * x2.b, x1.b * x2.a + x1.a * x2.b);
        }

        /// <summary>
        /// Returns a cosine value of the angle
        /// </summary>
        /// <returns></returns>
        public double CosAngle()
        {
            return a / Math.Sqrt(a * a + b * b);
        }

        /// <summary>
        /// Returns a rotation of the complex number around the cosine and sine part of the angle
        /// </summary>
        /// <param name="cosAngle">cosine part</param>
        /// <param name="sinAngle">sine part</param>
        /// <returns></returns>
        public Complex Rotate(double cosAngle, double sinAngle)
        {
            return new Complex(cosAngle * a - sinAngle * b, sinAngle * a + cosAngle * b);
        }

        /// <summary>
        /// Returns a rotation of the complex number around a given radian
        /// </summary>
        /// <param name="angle">angle in radians</param>
        /// <returns></returns>
        public Complex Rotate(double angle)
        {
            var CosAngle = Math.Cos(angle);
            var SinAngle = Math.Sin(angle);
            return new Complex(CosAngle * a - SinAngle * b, SinAngle * a + CosAngle * b);
        }
    }

}
