using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// Helper class with extension methods for vectors and points
    /// </summary>
    public static class PointHelper
    {
        /// <summary>
        /// Transforms a point from a component of a size refWidth x refHeight into a system whose
        /// axis is in the origin of this component
        /// </summary>
        /// <param name="pointToTransform">point that is to be transformed</param>
        /// <param name="refWidth">width of the component</param>
        /// <param name="refHeight">height of the component</param>
        /// <returns></returns>
        public static FPoint TransformPointToEuc(FPoint pointToTransform, double refWidth, double refHeight)
        {
            return new FPoint(pointToTransform.X - refWidth / 2, refHeight / 2 - pointToTransform.Y);
        }

        /// <summary>
        /// Transforms a point from FRAME geometry into Euclidean geometry
        /// </summary>
        /// <param name="pointToTransform">point that is to be transformed</param>
        /// <param name="refWidth">width of the component</param>
        /// <param name="refHeight">height of the component</param>
        /// <returns></returns>
        public static FPoint TransformPointToFrame(FPoint pointToTransform, double refWidth, double refHeight)
        {
            return new FPoint(pointToTransform.X + refWidth / 2, refHeight / 2 - pointToTransform.Y);
        }

        /// <summary>
        /// Returns a center of a rectangle
        /// </summary>
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Returns an area of a rectangle
        /// </summary>
        /// <returns></returns>
        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }

        /// <summary>
        /// Returns a distance between two points
        /// </summary>
        /// <returns></returns>
        public static int Distance(this Point point, Point p)
        {
            return Math.Abs(point.X - p.X) + Math.Abs(point.Y - p.Y);
        }

        /// <summary>
        /// Normalizes an array of points so that they will fit into a given rectangle
        /// </summary>
        /// <param name="points">array of points to transform</param>
        public static void NormalizePoints(Point[] points, Rectangle rectangle)
        {
            if (rectangle.Height == 0 || rectangle.Width == 0)
                return;

            Matrix m = new Matrix();
            m.Translate(rectangle.Center().X, rectangle.Center().Y);

            if (rectangle.Width > rectangle.Height)
                m.Scale(1, 1f * rectangle.Width / rectangle.Height);
            else
                m.Scale(1f * rectangle.Height / rectangle.Width, 1);

            m.Translate(-rectangle.Center().X, -rectangle.Center().Y);
            m.TransformPoints(points);
        }

        /// <summary>
        /// Normalizes an array of points as a relative normalization between two given rectangles
        /// </summary>
        /// <param name="points">array of points to transform</param>
        public static void NormalizePoints2(Point[] points, Rectangle rectangle, Rectangle needRectangle)
        {
            if (rectangle.Height == 0 || rectangle.Width == 0)
                return;

            float k1 = 1f * needRectangle.Width / rectangle.Width;
            float k2 = 1f * needRectangle.Height / rectangle.Height;
            float k = Math.Min(k1, k2);

            Matrix m = new Matrix();
            m.Scale(k, k);
            m.Translate(needRectangle.X / k - rectangle.X, needRectangle.Y / k - rectangle.Y);
            m.TransformPoints(points);
        }

        /// <summary>
        /// Returns a point shifted by dx and dy
        /// </summary>
        public static PointF Offset(this PointF p, float dx, float dy)
        {
            return new PointF(p.X + dx, p.Y + dy);
        }

    }
}
