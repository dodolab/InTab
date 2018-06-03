using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace InteractiveTable.Accessories
{
    /// <summary>
    /// Pomocna trida pro rozsirene operace s obdelniky a body
    /// </summary>
    public static class PointHelper
    {
        /// <summary>
        /// Transformuje bod z komponenty o rozemerech ref_width x ref_height do soustavy,
        /// jejiz osa je ve stredu teto komponenty
        /// </summary>
        /// <param name="point_to_transform">bod, ktery se bude transformovat</param>
        /// <param name="ref_width">sirka komponenty</param>
        /// <param name="ref_height">vyska komponenty</param>
        /// <returns></returns>
        public static FPoint TransformPointToEuc(FPoint point_to_transform, double ref_width, double ref_height)
        {
            return new FPoint(point_to_transform.X - ref_width / 2, ref_height / 2 - point_to_transform.Y);
        }

        /// <summary>
        /// Transformuje bod z FRAME geometrie do Euklidovske geometrie
        /// </summary>
        /// <param name="point_to_transform">bod, ktery se bude transformovat</param>
        /// <param name="ref_width">sirka prostoru</param>
        /// <param name="ref_height">vyska prostoru</param>
        /// <returns></returns>
        public static FPoint TransformPointToFrame(FPoint point_to_transform, double ref_width, double ref_height)
        {
            return new FPoint(point_to_transform.X + ref_width / 2, ref_height / 2 - point_to_transform.Y);
        }

        /// <summary>
        /// Vrati stred obdelniku
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Vrati obsah obdelniku
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }

        /// <summary>
        /// Vrati vzdalenost mezi dvema body
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int Distance(this Point point, Point p)
        {
            return Math.Abs(point.X - p.X) + Math.Abs(point.Y - p.Y);
        }

        /// <summary>
        /// Normalizuje pole bodu tak, aby se vesly do obdelnika rectangle
        /// </summary>
        /// <param name="points">pole bodu</param>
        /// <param name="rectangle">obdelnik</param>
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
        /// Normalizuje pole bodu jako relativni normalizaci mezi obdelniky rectangle a needRectangle
        /// </summary>
        /// <param name="points"></param>
        /// <param name="rectangle"></param>
        /// <param name="needRectangle"></param>
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
        /// Vrati bod posunuty o dx a dy
        /// </summary>
        /// <param name="p"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static PointF Offset(this PointF p, float dx, float dy)
        {
            return new PointF(p.X + dx, p.Y + dy);
        }

    }
}
