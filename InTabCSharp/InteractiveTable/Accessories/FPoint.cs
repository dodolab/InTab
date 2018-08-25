using System;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// Floating point in 2D space
    /// </summary>
    [Serializable]
    public class FPoint
    {
        public double X, Y;

        public FPoint()
        {
        }

        public FPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Translates the point by X and Y
        /// </summary>
        public void Add(double X, double Y)
        {
            this.X += X;
            this.Y += Y;
        }

        /// <summary>
        /// Translates the point by coordinates represented in the parameter
        /// </summary>
        public void Add(FPoint other)
        {
            this.X += other.X;
            this.Y += other.Y;
        }

        public void Set(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public void Set(FPoint other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }
    }
}
