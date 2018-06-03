using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// Trida pro lepsi pocitani s body
    /// </summary>
    [Serializable]
    public class FPoint
    {
        public double X, Y;

        public FPoint()
        {
        }

        /// <summary>
        /// Vytvori novy bod
        /// </summary>
        /// <param name="x">x-ova hodnota</param>
        /// <param name="y">y-ova hodnota</param>
        public FPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Posune bod o hodnotu X a Y
        /// </summary>
        /// <param name="X">x-ovy posun</param>
        /// <param name="Y">y-ovy posun</param>
        public void Add(double X, double Y)
        {
            this.X += X;
            this.Y += Y;
        }

        /// <summary>
        /// Posune bod o souradnice bodu other
        /// </summary>
        /// <param name="other"></param>
        public void Add(FPoint other)
        {
            this.X += other.X;
            this.Y += other.Y;
        }

        /// <summary>
        /// Nastavi hodnoty X a Y
        /// </summary>
        /// <param name="X">nova hodnota X</param>
        /// <param name="Y">nova hodnota Y</param>
        public void Set(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Nastavi hodnoty podle bodu other
        /// </summary>
        /// <param name="other"></param>
        public void Set(FPoint other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }
    }
}
