using System;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// 2-Dimensional vector
    /// </summary>
    [Serializable]
    public class FVector
    {
        public FVector()
        {
        }

        private double x, y;
        
        /// <summary>
        /// Dirty flag
        /// </summary>
        private bool changed = false;

        /// <summary>
        /// Size of the vector, recalculated upon change
        /// </summary>
        private double size;

        public FVector(double x, double y)
        {
            this.X = x;
            this.Y = y;
            size = Math.Sqrt(x * x + y * y);
        }

        public double X
        {
            get { return x; }
            set
            {
                this.x = value;
                changed = true;
            }
        }
        
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
        /// Increases the value of the vector from given values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Add(double x, double y)
        {
            this.X += x;
            this.Y += y;
            changed = true;
        }

        /// <summary>
        /// Increases the value according to the given vector
        /// </summary>
        public void Add(FVector other)
        {
            this.X += other.X;
            this.Y += other.Y;
            changed = true;
        }

        public void Set(double x, double y)
        {
            this.X = x;
            this.Y = y;
            changed = true;
        }

        /// <summary>
        /// Assigns value by given vector
        /// </summary>
        public void Set(FVector other)
        {
            this.X = other.X;
            this.Y = other.Y;
            changed = true;
        }

        /// <summary>
        /// Multiplication
        /// </summary>
        public void Mult(double valueX, double valueY)
        {
            this.X *= valueX;
            this.Y *= valueY;
            changed = true;
        }

        /// <summary>
        /// Inverts X a Y
        /// </summary>
        public void Invert()
        {
            if (X != 0) this.X = 1 / X;
            if (Y != 0) this.Y = 1 / Y;
            changed = true;
        }

        /// <summary>
        /// Returns a size of this vector
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
