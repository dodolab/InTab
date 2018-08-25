using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;
using InteractiveTable.Settings;
using InteractiveTable.Managers;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// Calibration window
    /// </summary>
    public partial class TableCalibrationWindow : Window
    {

        private System.Drawing.Image frame; // image from camera as System.Drawing.Image
        private Image UIFrame; // image from camera as UIComponent

        private Point[] calibrationPoints; // calibration points
        private double perspective = 0; // perspective deformation, expresses a ration of the bottom and the top base of a trapezoid
        private System.Windows.Shapes.Ellipse[] visualPoints; // visible points
        private Line[] visualLines; // visible lines
        private int pointCounter = 0; // point counter
        private int rotation = 0; // rotation of the captured image
        private CalibrationMode mode; // calibration mode

        private Point originalMousePosition; // mouse position before moving over the window

        System.Windows.Shapes.Ellipse draggingPoint = null; // current dragging point
        System.Windows.Shapes.Line draggingLine = null; // current dragging line 
        private Point draggingPoint_firstPos = new Point(); // original position of the dragging point

        public TableCalibrationWindow()
        {
            InitializeComponent();
        }

        #region functions

        /// <summary>
        /// Loads calibration settings
        /// </summary>
        public void LoadValues()
        {
            calibrationPoints = new Point[2];
            calibrationPoints[0] = CalibrationSettings.Instance().CALIBRATION_POINT_A;
            calibrationPoints[1] = CalibrationSettings.Instance().CALIBRATION_POINT_C;

            if (calibrationPoints[0].X != calibrationPoints[1].X && calibrationPoints[0].Y != calibrationPoints[1].Y)
            {
                pointCounter = 2;
                rotation = CalibrationSettings.Instance().CALIBRATION_ROTATION;
                perspective = CalibrationSettings.Instance().CALIBRATION_PERSPECTIVE;
                
                if (perspective != 1)
                {
                    perspectiveRadio.IsChecked = true;
                    mode = CalibrationMode.PERSPECTIVE;
                }
                else
                {
                    mode = CalibrationMode.RECTANGLE;
                    rectangleRadio.IsChecked = true;
                }

                DrawPoint((int)calibrationPoints[0].X, (int)calibrationPoints[0].Y, 0);
                DrawPoint((int)calibrationPoints[1].X, (int)calibrationPoints[1].Y, 1);
                CreateLines();
            }
            CaptureImage();
        }

        /// <summary>
        /// Refreshes all values
        /// </summary>
        private void RepaintValues()
        {
            foreach (Line ln in visualLines) imageBox.Children.Remove(ln);
            foreach (System.Windows.Shapes.Ellipse el in visualPoints) imageBox.Children.Remove(el);

            DrawPoint((int)calibrationPoints[0].X, (int)calibrationPoints[0].Y, 0);
            DrawPoint((int)calibrationPoints[1].X, (int)calibrationPoints[1].Y, 1);
            CreateLines();
            CaptureImage();
        }

        /// <summary>
        /// Saves calibration parameters (we need only coordinates of two edges, rotation and the mode)
        /// </summary>
        private void SaveValues()
        {
            CalibrationSettings.Instance().CALIBRATION_POINT_A.X = calibrationPoints[0].X;
            CalibrationSettings.Instance().CALIBRATION_POINT_A.Y = calibrationPoints[0].Y;
            CalibrationSettings.Instance().CALIBRATION_POINT_C.X = calibrationPoints[1].X;
            CalibrationSettings.Instance().CALIBRATION_POINT_C.Y = calibrationPoints[1].Y;
            CalibrationSettings.Instance().CALIBRATION_ROTATION = rotation;
            CalibrationSettings.Instance().CALIBRATION_PERSPECTIVE = perspective;
            
            CalibrationSettings.Instance().Save();
        }

        /// <summary>
        /// Creates 4 lines from 2 calibration points and joins them into a rectangle
        /// </summary>
        private void CreateLines()
        {
            if ((calibrationPoints[1].X <= calibrationPoints[0].X) || (calibrationPoints[1].Y >= calibrationPoints[0].Y))
            {
                // we must draw the points from left to right
                MessageBox.Show("Anchor points must be created from left to right and from bottom to top!");
                resetBut_Click(null, null);
                return;
            }

            Point p1 = calibrationPoints[0];
            Point p3 = calibrationPoints[1];

            // estimation of all other points ( a = (2*r1r2)/(r1+1))
            double r1 = perspective;
            double r2 = p3.X - p1.X;
            double bx = p1.X + (2 * (r1 * r2)) / (r1 + 1); // X-coord of the B-point of a trapezoid
            double cx = p3.X; // X-coord of the C-point of a trapezoid
            double dx = p1.X + bx - cx; // X-coord of the D-point of a trapezoid

            Point p2 = new Point(dx, p3.Y);
            Point p4 = new Point(bx, p1.Y);

            DrawPoint((int)p2.X, (int)p2.Y, 2);
            DrawPoint((int)p4.X, (int)p4.Y, 3);

            DrawLine((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, 0);
            DrawLine((int)p2.X, (int)p2.Y, (int)p3.X, (int)p3.Y, 1);
            DrawLine((int)p3.X, (int)p3.Y, (int)p4.X, (int)p4.Y, 2);
            DrawLine((int)p4.X, (int)p4.Y, (int)p1.X, (int)p1.Y, 3);
        }

        /// <summary>
        /// Draws a point on [X,Y] coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinte</param>
        /// <param name="index">index of an array to which the new point should be stored</param>
        private void DrawPoint(int x, int y, int index)
        {
            System.Windows.Shapes.Ellipse point = new System.Windows.Shapes.Ellipse();
            point.Fill = Brushes.Red;
            point.Width = 10;
            point.Height = 10;
            point.DataContext = new Point(x, y); // store its position into a dataContext
            point.Margin = GetMargin(x - point.Width / 2, y - point.Height / 2);
            point.VerticalAlignment = VerticalAlignment.Top;
            point.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetZIndex(point, 1000);
            imageBox.Children.Add(point);
            if (index >= 0) visualPoints[index] = point;
            point.MouseDown += new MouseButtonEventHandler(point_MouseDown);
        }

        /// <summary>
        /// Returns a position of a point relative to an image
        /// </summary>
        /// <returns></returns>
        private Thickness GetMargin(double x, double y)
        {
            Point referred = UIFrame.TranslatePoint(new Point(0, 0), imageBox);
            return new Thickness(referred.X + x, referred.Y + y, 0, 0);
        }

        /// <summary>
        /// Changes the size of the rectangle according to the dragged point
        /// </summary>
        private void ResizeRectangle(Point delta)
        {
            Point dragging = (Point)draggingPoint.DataContext;
            if (calibrationPoints[0].X == dragging.X) calibrationPoints[0] = new Point(calibrationPoints[0].X + delta.X, calibrationPoints[0].Y);
            if (calibrationPoints[0].Y == dragging.Y) calibrationPoints[0] = new Point(calibrationPoints[0].X, calibrationPoints[0].Y + delta.Y);
            if (calibrationPoints[1].X == dragging.X) calibrationPoints[1] = new Point(calibrationPoints[1].X + delta.X, calibrationPoints[1].Y);
            if (calibrationPoints[1].Y == dragging.Y) calibrationPoints[1] = new Point(calibrationPoints[1].X, calibrationPoints[1].Y + delta.Y);

            RepaintValues();
        }

        /// <summary>
        /// Shifts all objects by X and Y
        /// </summary>
        private void ChangeMargins(double x, double y)
        {
            calibrationPoints[0] = new Point(calibrationPoints[0].X + x, calibrationPoints[0].Y + y);
            calibrationPoints[1] = new Point(calibrationPoints[1].X + x, calibrationPoints[1].Y + y);

            visualPoints[0].Margin = new Thickness(visualPoints[0].Margin.Left + x, visualPoints[0].Margin.Top + y, 0, 0);
            visualPoints[1].Margin = new Thickness(visualPoints[1].Margin.Left + x, visualPoints[1].Margin.Top + y, 0, 0);
            visualPoints[2].Margin = new Thickness(visualPoints[2].Margin.Left + x, visualPoints[2].Margin.Top + y, 0, 0);
            visualPoints[3].Margin = new Thickness(visualPoints[3].Margin.Left + x, visualPoints[3].Margin.Top + y, 0, 0);

            visualPoints[0].DataContext = new Point(((Point)visualPoints[0].DataContext).X + x, ((Point)visualPoints[0].DataContext).Y + y);
            visualPoints[1].DataContext = new Point(((Point)visualPoints[1].DataContext).X + x, ((Point)visualPoints[1].DataContext).Y + y);
            visualPoints[2].DataContext = new Point(((Point)visualPoints[2].DataContext).X + x, ((Point)visualPoints[2].DataContext).Y + y);
            visualPoints[3].DataContext = new Point(((Point)visualPoints[3].DataContext).X + x, ((Point)visualPoints[3].DataContext).Y + y);

            visualLines[0].Margin = new Thickness(visualLines[0].Margin.Left + x, visualLines[0].Margin.Top + y, 0, 0);
            visualLines[1].Margin = new Thickness(visualLines[1].Margin.Left + x, visualLines[1].Margin.Top + y, 0, 0);
            visualLines[2].Margin = new Thickness(visualLines[2].Margin.Left + x, visualLines[2].Margin.Top + y, 0, 0);
            visualLines[3].Margin = new Thickness(visualLines[3].Margin.Left + x, visualLines[3].Margin.Top + y, 0, 0);

            visualLines[0].DataContext = new Point[]{new Point(((Point[])visualLines[0].DataContext)[0].X+x,((Point[])visualLines[0].DataContext)[0].Y+y),
                new Point(((Point[])visualLines[0].DataContext)[1].X+x,((Point[])visualLines[0].DataContext)[1].Y+y)};
            visualLines[1].DataContext = new Point[]{new Point(((Point[])visualLines[1].DataContext)[0].X+x,((Point[])visualLines[1].DataContext)[0].Y+y),
                new Point(((Point[])visualLines[1].DataContext)[1].X+x,((Point[])visualLines[1].DataContext)[1].Y+y)};
            visualLines[2].DataContext = new Point[]{new Point(((Point[])visualLines[2].DataContext)[0].X+x,((Point[])visualLines[2].DataContext)[0].Y+y),
                new Point(((Point[])visualLines[2].DataContext)[1].X+x,((Point[])visualLines[2].DataContext)[1].Y+y)};
            visualLines[3].DataContext = new Point[]{new Point(((Point[])visualLines[3].DataContext)[0].X+x,((Point[])visualLines[3].DataContext)[0].Y+y),
                new Point(((Point[])visualLines[3].DataContext)[1].X+x,((Point[])visualLines[3].DataContext)[1].Y+y)};
        }

        /// <summary>
        /// Transforms the perspective by X-parameter
        /// </summary>
        /// <param name="x"></param>
        private void TransformPerspective(double x)
        {
            System.Windows.Shapes.Ellipse referred = null; // a point that will also be affected

            foreach (System.Windows.Shapes.Ellipse el in visualPoints)
            {
                if (el != draggingPoint && el.Margin.Top == draggingPoint.Margin.Top)
                {
                    referred = el;
                    break;
                }
            }

            Point referredPoint = (Point)referred.DataContext;
            Point draggingOwnPoint = (Point)draggingPoint.DataContext;
            
            // we need to move only the top points
            if (referredPoint.Y > calibrationPoints.Min(rp => rp.Y)) return;
            // we shouldn't cross the min dragging point
            if (Math.Abs(referredPoint.X - draggingOwnPoint.X - 2 * x) <= CommonAttribService.DEFAULT_CALIBRATION_MINIMUM) return;
            // the points mustn't go beyond the image
            if ((x > 0) &&
                (Math.Min(referredPoint.X, draggingOwnPoint.X)) + x < 0 ||
                (Math.Max(referredPoint.X, draggingOwnPoint.X)) - x >= UIFrame.ActualWidth) return;

            TransformPerspectiveLines(referredPoint, draggingOwnPoint, x);

            // relax the anchor points
            if (((int)calibrationPoints[0].X) == (int)referredPoint.X && ((int)calibrationPoints[0].Y) == (int)referredPoint.Y) calibrationPoints[0] = new Point(referredPoint.X - x, referredPoint.Y);
            if (((int)calibrationPoints[0].X) == (int)draggingOwnPoint.X && ((int)calibrationPoints[0].Y) == (int)draggingOwnPoint.Y) calibrationPoints[0] = new Point(draggingOwnPoint.X + x, draggingOwnPoint.Y);
            if (((int)calibrationPoints[1].X) == (int)referredPoint.X && ((int)calibrationPoints[1].Y) == (int)referredPoint.Y) calibrationPoints[1] = new Point(referredPoint.X - x, referredPoint.Y);
            if (((int)calibrationPoints[1].X) == (int)draggingOwnPoint.X && ((int)calibrationPoints[1].Y) == (int)draggingOwnPoint.Y) calibrationPoints[1] = new Point(draggingOwnPoint.X + x, draggingOwnPoint.Y);

            // apply the transform and re-render all lines
            referredPoint = new Point(referredPoint.X - x, referredPoint.Y);
            referred.Margin = new Thickness(referred.Margin.Left - x, referred.Margin.Top, 0, 0);
            referred.DataContext = referredPoint;
            draggingOwnPoint = new Point(draggingOwnPoint.X + x, draggingOwnPoint.Y);
            draggingPoint.Margin = new Thickness(draggingPoint.Margin.Left + x, draggingPoint.Margin.Top, 0, 0);
            draggingPoint.DataContext = draggingOwnPoint;

            // edit the perspective transform
            perspective = Math.Abs((((Point)(visualPoints[3].DataContext)).X - ((Point)(visualPoints[0].DataContext)).X) / (((Point)(visualPoints[2].DataContext)).X - ((Point)(visualPoints[1].DataContext)).X));
        }

        /// <summary>
        /// Transforms all lines by a referred point
        /// </summary>
        private void TransformPerspectiveLines(Point referredPoint, Point draggingOwnPoint, double x)
        {

            foreach (System.Windows.Shapes.Line line in visualLines)
            {
                Point[] linePoints = (Point[])line.DataContext;
                if (linePoints[0].X == referredPoint.X && linePoints[0].Y == referredPoint.Y)
                {
                    linePoints[0] = new Point(linePoints[0].X - x, linePoints[0].Y);
                    line.X1 -= x;
                }
                if (linePoints[1].X == referredPoint.X && linePoints[1].Y == referredPoint.Y)
                {
                    linePoints[1] = new Point(linePoints[1].X - x, linePoints[1].Y);
                    line.X2 -= x;
                }
                if (linePoints[0].X == draggingOwnPoint.X && linePoints[0].Y == draggingOwnPoint.Y)
                {
                    linePoints[0] = new Point(linePoints[0].X + x, linePoints[0].Y);
                    line.X1 += x;
                }
                if (linePoints[1].X == draggingOwnPoint.X && linePoints[1].Y == draggingOwnPoint.Y)
                {
                    linePoints[1] = new Point(linePoints[1].X + x, linePoints[1].Y);
                    line.X2 += x;
                }
            }
        }

        /// <summary>
        /// Renders a line from A to B
        /// </summary>
        private void DrawLine(int x1, int y1, int x2, int y2, int index)
        {
            Line line = new Line();
            line.Stroke = Brushes.DarkRed;
            line.StrokeThickness = 5;
            line.X1 = GetMargin(x1, 0).Left;
            line.Y1 = GetMargin(0, y1).Top;
            line.X2 = GetMargin(x2, 0).Left;
            line.Y2 = GetMargin(0, y2).Top;
            line.DataContext = new Point[] { new Point(x1, y1), new Point(x2, y2) };
            line.VerticalAlignment = VerticalAlignment.Top;
            line.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetZIndex(line, 999);
            imageBox.Children.Add(line);
            visualLines[index] = line;
            line.MouseDown += new MouseButtonEventHandler(line_MouseDown);
        }

        /// <summary>
        /// Captures a image and renders it into the view
        /// </summary>
        private void CaptureImage()
        {
            // get image
            Image<Bgr, byte> tempFrame = CameraManager.GetImage();

            // rotate image
            if (rotation != 0)
            {
                tempFrame = tempFrame.Rotate(rotation, new Bgr(0, 0, 0), true);
            }
            // transform into bitmap
            frame = tempFrame.ToBitmap(tempFrame.Width, tempFrame.Height);
            // transform into UIComponent
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(frame);
            IntPtr hBitmap = bmp.GetHbitmap();
            System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            UIFrame = new Image();
            UIFrame.Source = WpfBitmap;
            if (imageBox.Children.Contains(UIFrame)) imageBox.Children.Remove(UIFrame);
            imageBox.Children.Add(UIFrame);
        }


        #endregion

        #region handlers

        /// <summary>
        /// Capture an image after load
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CaptureImage();
            imageBox.MouseUp += new MouseButtonEventHandler(imageBox_MouseUp);
            imageBox.MouseMove += new MouseEventHandler(imageBox_MouseMove);

            visualPoints = new System.Windows.Shapes.Ellipse[4];
            visualLines = new Line[4];
       }

        /// <summary>
        /// Releasing mouse button over the imageView will create a new point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (draggingPoint == null) 
            {
                if (pointCounter < 2) // there must be two points at most
                {
                    pointCounter++;
                    calibrationPoints[pointCounter - 1] = new Point((int)e.GetPosition(UIFrame).X, (int)e.GetPosition(UIFrame).Y);
                    DrawPoint((int)calibrationPoints[pointCounter - 1].X, (int)calibrationPoints[pointCounter - 1].Y, pointCounter - 1);

                    if (pointCounter == 2)
                    {
                        // if there are more than 2 points, create a rectangle
                    }
                }
            }
            else
            {
                if (mode == CalibrationMode.RECTANGLE)
                {
                    ResizeRectangle(new Point(draggingPoint.Margin.Left - draggingPoint_firstPos.X, draggingPoint.Margin.Top - draggingPoint_firstPos.Y));
                }
            }
        }


        /// <summary>
        /// Move with a line or a point
        /// </summary>
        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (draggingLine != null) 
                {
                    // moving a line
                    Point delta = new Point(e.GetPosition(imageBox).X-originalMousePosition.X,e.GetPosition(imageBox).Y-originalMousePosition.Y);
                    ChangeMargins(delta.X, delta.Y);
                }
                else if (draggingPoint != null && mode == CalibrationMode.PERSPECTIVE)
                {
                    // moving a point in perspective mode
                    Point delta = new Point(e.GetPosition(imageBox).X - originalMousePosition.X, e.GetPosition(imageBox).Y - originalMousePosition.Y);
                    TransformPerspective(delta.X);
                }
                else if (draggingPoint != null && mode == CalibrationMode.RECTANGLE)
                {
                    // moving a point in rectangle mode
                    Point delta = new Point(e.GetPosition(imageBox).X - originalMousePosition.X, e.GetPosition(imageBox).Y - originalMousePosition.Y);
                    draggingPoint.Margin = new Thickness(draggingPoint.Margin.Left + delta.X, draggingPoint.Margin.Top + delta.Y, 0, 0);
                }
            }
            else
            {
                draggingLine = null;
                draggingPoint = null;
            }

           originalMousePosition = e.GetPosition(imageBox); // save mouse position
        }

        /// <summary>
        /// Pressing a mouse button will set a dragging point
        /// </summary>
        private void point_MouseDown(object sender, MouseButtonEventArgs e)
        {
            draggingPoint = (System.Windows.Shapes.Ellipse)sender;
            draggingPoint_firstPos = new Point(draggingPoint.Margin.Left, draggingPoint.Margin.Top);
        }

        /// <summary>
        /// Moving a line
        /// </summary>
        private void line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            draggingLine = (System.Windows.Shapes.Line)sender;
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        /// <summary>
        /// Click on reset will reset all settings
        /// </summary>
        private void resetBut_Click(object sender, RoutedEventArgs e)
        {
            pointCounter = 0;
            imageBox.Children.Clear();
            if (mode == CalibrationMode.RECTANGLE) perspective = 1;
            CaptureImage();
        }

        /// <summary>
        /// Click on OK will save changes
        /// </summary>
        private void OKBut_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Do you want to save changes?", "Settings change", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                SaveValues();
            }
            // if the SHIFT is pressed, we will not close the window (WTF??)
            if(!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))  this.Close();
        }

        /// <summary>
        /// Click on CAPTURE will capture a new image
        /// </summary>
        private void frameBut_Click(object sender, RoutedEventArgs e)
        {
            CaptureImage();
        }

        /// <summary>
        /// Click on ROTATE will rotate the image to the right
        /// </summary>
        private void rotateBut3_Click(object sender, RoutedEventArgs e)
        {
            rotation = (rotation + 3) % 360;
            CaptureImage();
        }

        /// <summary>
        /// Click on ROTATE will rotate the image to the right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotateBut30_Click(object sender, RoutedEventArgs e)
        {
            rotation = (rotation + 30) % 360;
            CaptureImage();
        }

        /// <summary>
        /// Change of the mode
        /// </summary>
        private void rectangleRadio_Checked(object sender, RoutedEventArgs e)
        {
            // pokud byl predchozi mod perspective ,musime zacit odznova
            if (mode == CalibrationMode.PERSPECTIVE) resetBut_Click(null, null);
            this.mode = CalibrationMode.RECTANGLE;
            perspective = 1;
        }

        /// <summary>
        /// Change of the mode
        /// </summary>
        private void perspectiveRadio_Checked(object sender, RoutedEventArgs e)
        {
            this.mode = CalibrationMode.PERSPECTIVE;
        }

        #endregion
    }

    public enum CalibrationMode
    {
        RECTANGLE, PERSPECTIVE
    }
}
