using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using InteractiveTable.Settings;
using InteractiveTable.Managers;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// Kalibracni okno, nastavuje kalibracni obdelnik
    /// </summary>
    public partial class TableCalibrationWindow : Window
    {

        private System.Drawing.Image frame; // obrazek z kamery jako Drawing.Bitmap
        private Image UIFrame; // obrazek jako UIComponent

        private Point[] calibrationPoints; // kalibracni body, staci pouze dva
        private double perspective = 0; // perspektivni zkresleni, vyjadruje pomer dolni zakladny a horni zakladny pouze pro mod perspective
        private System.Windows.Shapes.Ellipse[] visualPoints; // viditelne body
        private Line[] visualLines; // viditelne usecky;
        private int pointCounter = 0; // citac bodu
        private int rotation = 0; // rotace zachyceneho obrazku
        private CalibrationMode mode; // kalibracni mod (obdelnik nebo perspektiva)

        private Point originalMousePosition; // ulozeni pozice mysi pri pohybu nad oknem

        System.Windows.Shapes.Ellipse draggingPoint = null; // bod, se kterym uzivatel hybe
        System.Windows.Shapes.Line draggingLine = null; // usecka, se kterou uzivatel hybe
        private Point draggingPoint_firstPos = new Point(); // prvotni pozice draggingPointu, nez se s nim zacne hybat

        public TableCalibrationWindow()
        {
            InitializeComponent();
        }

        #region functions

        /// <summary>
        /// Nacte drivejsi zkalibrovane parametry
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

                // nastaveni radio buttonu
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
        /// Prekresli vsechny hodnoty
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
        /// Ulozi kalibracni parametry (Staci souradnice dvou rohu, rotace a perspektivni mod)
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
        /// Vytvori ze 2 kalibracnich bodu 4 usecky a spoji je do obdelniku
        /// </summary>
        private void CreateLines()
        {
            if ((calibrationPoints[1].X <= calibrationPoints[0].X) || (calibrationPoints[1].Y >= calibrationPoints[0].Y))
            {
                // musi se kreslit zleva doprava, jinak je to spatne
                MessageBox.Show("Záchytné body musí být zleva doprava zdola nahoru!!");
                resetBut_Click(null, null);
                return;
            }

            Point p1 = calibrationPoints[0];
            Point p3 = calibrationPoints[1];

            // ODHAD OSTATNICH BODU PODLE ZADANYCH HODNOT ( a = (2*r1r2)/(r1+1))
            double r1 = perspective;
            double r2 = p3.X - p1.X;
            double bx = p1.X + (2 * (r1 * r2)) / (r1 + 1); // X-ova souradnice bodu B v lichobezniku
            double cx = p3.X; // X-ova souradnice bodu C v lichobezniku
            double dx = p1.X + bx - cx; // X-ova souradnice bodu D v lichobezniku

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
        /// Vykresli bod na souradnicich X a Y; index je index v poli, kam se bod ulozi
        /// </summary>
        /// <param name="x">souradnice X</param>
        /// <param name="y">souradnice Y</param>
        /// <param name="index">index v poli, kam se novy bod ulozi</param>
        private void DrawPoint(int x, int y, int index)
        {
            System.Windows.Shapes.Ellipse point = new System.Windows.Shapes.Ellipse();
            point.Fill = Brushes.Red;
            point.Width = 10;
            point.Height = 10;
            point.DataContext = new Point(x, y); // v data-contextu bude mit ulozenu svoji polohu!!
            point.Margin = GetMargin(x - point.Width / 2, y - point.Height / 2);
            point.VerticalAlignment = VerticalAlignment.Top;
            point.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetZIndex(point, 1000);
            imageBox.Children.Add(point);
            if (index >= 0) visualPoints[index] = point;
            point.MouseDown += new MouseButtonEventHandler(point_MouseDown);
        }

        /// <summary>
        /// Vrati pozici bodu vzhledem k obrazku a vzhledem k celemu imageboxu
        /// </summary>
        /// <param name="x">pozice bodu v ose X</param>
        /// <param name="y">pozice bodu v ose Y</param>
        /// <returns></returns>
        private Thickness GetMargin(double x, double y)
        {
            Point referred = UIFrame.TranslatePoint(new Point(0, 0), imageBox);
            return new Thickness(referred.X + x, referred.Y + y, 0, 0);
        }

        /// <summary>
        /// Zvetsi/zmensi obdelnik podle tazeneho bodu
        /// </summary>
        /// <param name="delta"></param>
        private void ResizeRectangle(Point delta)
        {
            // musime zjistit vsechny body obdelnika, ktere se budou posouvat (vsechny krome uhlopricky)
            Point dragging = (Point)draggingPoint.DataContext;
            if (calibrationPoints[0].X == dragging.X) calibrationPoints[0] = new Point(calibrationPoints[0].X + delta.X, calibrationPoints[0].Y);
            if (calibrationPoints[0].Y == dragging.Y) calibrationPoints[0] = new Point(calibrationPoints[0].X, calibrationPoints[0].Y + delta.Y);
            if (calibrationPoints[1].X == dragging.X) calibrationPoints[1] = new Point(calibrationPoints[1].X + delta.X, calibrationPoints[1].Y);
            if (calibrationPoints[1].Y == dragging.Y) calibrationPoints[1] = new Point(calibrationPoints[1].X, calibrationPoints[1].Y + delta.Y);

            RepaintValues();
        }

        /// <summary>
        /// Posune vsechny objekty na plose podle hodnot X a Y
        /// </summary>
        /// <param name="x">hodnota X-ove souradnice</param>
        /// <param name="y">hodnota Y-ove souradnice</param>
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
        /// Provede transformaci perspektivy podle promenne X
        /// </summary>
        /// <param name="x"></param>
        private void TransformPerspective(double x)
        {
            System.Windows.Shapes.Ellipse referred = null; // bod, ktery bude take ovlivnen

            // najdeme bod, ktery bude take ovlivnen
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

            // kontrola, abychom tahli pouze za horni body
            if (referredPoint.Y > calibrationPoints.Min(rp => rp.Y)) return;
            // kontrola, aby zkresleni nebylo prilis velke
            if (Math.Abs(referredPoint.X - draggingOwnPoint.X - 2 * x) <= CommonAttribService.DEFAULT_CALIBRATION_MINIMUM) return;
            // kontrola, aby se body nedostaly mimo obrazek
            if ((x > 0) &&
                (Math.Min(referredPoint.X, draggingOwnPoint.X)) + x < 0 ||
                (Math.Max(referredPoint.X, draggingOwnPoint.X)) - x >= UIFrame.ActualWidth) return;

            TransformPerspectiveLines(referredPoint, draggingOwnPoint, x);

            // uprava zachytnych bodu
            if (((int)calibrationPoints[0].X) == (int)referredPoint.X && ((int)calibrationPoints[0].Y) == (int)referredPoint.Y) calibrationPoints[0] = new Point(referredPoint.X - x, referredPoint.Y);
            if (((int)calibrationPoints[0].X) == (int)draggingOwnPoint.X && ((int)calibrationPoints[0].Y) == (int)draggingOwnPoint.Y) calibrationPoints[0] = new Point(draggingOwnPoint.X + x, draggingOwnPoint.Y);
            if (((int)calibrationPoints[1].X) == (int)referredPoint.X && ((int)calibrationPoints[1].Y) == (int)referredPoint.Y) calibrationPoints[1] = new Point(referredPoint.X - x, referredPoint.Y);
            if (((int)calibrationPoints[1].X) == (int)draggingOwnPoint.X && ((int)calibrationPoints[1].Y) == (int)draggingOwnPoint.Y) calibrationPoints[1] = new Point(draggingOwnPoint.X + x, draggingOwnPoint.Y);

            // provedeme transformaci, prekreslime usecky, nastavime hodnoty
            referredPoint = new Point(referredPoint.X - x, referredPoint.Y);
            referred.Margin = new Thickness(referred.Margin.Left - x, referred.Margin.Top, 0, 0);
            referred.DataContext = referredPoint;
            draggingOwnPoint = new Point(draggingOwnPoint.X + x, draggingOwnPoint.Y);
            draggingPoint.Margin = new Thickness(draggingPoint.Margin.Left + x, draggingPoint.Margin.Top, 0, 0);
            draggingPoint.DataContext = draggingOwnPoint;

            // upraveni perspektivniho zkresleni
            perspective = Math.Abs((((Point)(visualPoints[3].DataContext)).X - ((Point)(visualPoints[0].DataContext)).X) / (((Point)(visualPoints[2].DataContext)).X - ((Point)(visualPoints[1].DataContext)).X));
        }

        /// <summary>
        /// Provede perspektivni transformaci vsech usecek
        /// </summary>
        /// <param name="referredPoint"></param>
        /// <param name="x"></param>
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
        /// Vykresli usecku z bodu A do bodu B
        /// </summary>
        /// <param name="x1">X-ova souradnice prvniho bodu</param>
        /// <param name="y1">Y-ova souradnice prvniho bodu</param>
        /// <param name="x2">X-ova souradnice druheho bodu</param>
        /// <param name="y2">Y-ova souradnice druheho bodu</param>
        /// <param name="index"></param>
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
        /// Ziska obrazek z kamery a promitne ho do imageboxu
        /// </summary>
        private void CaptureImage()
        {
            // ziska obrazek
            Image<Bgr, byte> tempFrame = CameraManager.GetImage();

            // rotace
            if (rotation != 0)
            {
                tempFrame = tempFrame.Rotate(rotation, new Bgr(0, 0, 0), true);
            }
            // prevede ho do bitmapy
            frame = tempFrame.ToBitmap(tempFrame.Width, tempFrame.Height);
            // prevede System.Drawing.Image do System.Windows.Controls, aby se mohl zobrazit
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
        /// Po nacteni okna se ihned zachyti obrazek a nastavi se handlery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CaptureImage();
            // nastaveni handleru::
            imageBox.MouseUp += new MouseButtonEventHandler(imageBox_MouseUp);
            imageBox.MouseMove += new MouseEventHandler(imageBox_MouseMove);

            visualPoints = new System.Windows.Shapes.Ellipse[4];
            visualLines = new Line[4];
       }

        /// <summary>
        /// Uvolneni tlacitka nad imageBoxem vykresli a zapise novy bod
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (draggingPoint == null) // pokud se nejednalo o hybani s bodem
            {
                if (pointCounter < 2) // pokud je zde 0 nebo 1 bod, vykresli ho
                {
                    pointCounter++;
                    calibrationPoints[pointCounter - 1] = new Point((int)e.GetPosition(UIFrame).X, (int)e.GetPosition(UIFrame).Y);
                    // pozice na platne jsou jine nez pozice na obrazku
                    DrawPoint((int)calibrationPoints[pointCounter - 1].X, (int)calibrationPoints[pointCounter - 1].Y, pointCounter - 1);

                    if (pointCounter == 2)
                    {
                        // TODO!!! perspektiva se pocita nejak divne
                        CreateLines(); // pokud jsou zde jiz dva body, vytvor obdelnik
                    }
                }
            }
            else
            {
                if (mode == CalibrationMode.RECTANGLE)
                {
                    // prekresli obdelnik podle toho, kam se hnul bod
                    ResizeRectangle(new Point(draggingPoint.Margin.Left - draggingPoint_firstPos.X, draggingPoint.Margin.Top - draggingPoint_firstPos.Y));
                }
            }
        }


        /// <summary>
        /// Pohyb mysi po plose; pokud je stisknute tlacitko mysi, bude
        /// se pohybovat budto bod nebo cely objekt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (draggingLine != null) // pohybuje se s useckou
                {
                    // hybame s useckou
                    Point delta = new Point(e.GetPosition(imageBox).X-originalMousePosition.X,e.GetPosition(imageBox).Y-originalMousePosition.Y);
                    ChangeMargins(delta.X, delta.Y);
                }
                else if (draggingPoint != null && mode == CalibrationMode.PERSPECTIVE) // pohybuje se s bodem -> perspektivni zkresleni
                {
                    // hybame s perspektivnim bodem
                    Point delta = new Point(e.GetPosition(imageBox).X - originalMousePosition.X, e.GetPosition(imageBox).Y - originalMousePosition.Y);
                    TransformPerspective(delta.X);
                }
                else if (draggingPoint != null && mode == CalibrationMode.RECTANGLE)
                {
                    // hybame s obdelnikovym bodem
                    Point delta = new Point(e.GetPosition(imageBox).X - originalMousePosition.X, e.GetPosition(imageBox).Y - originalMousePosition.Y);
                    draggingPoint.Margin = new Thickness(draggingPoint.Margin.Left + delta.X, draggingPoint.Margin.Top + delta.Y, 0, 0);
                }
            }
            else
            {
                draggingLine = null;
                draggingPoint = null;
            }

           originalMousePosition = e.GetPosition(imageBox); // ulozeni pozice mysi
        }

        /// <summary>
        /// Stisknuti tlacitka mysi nastavi draggingPoint, aby se mohl posouvat s mysi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void point_MouseDown(object sender, MouseButtonEventArgs e)
        {
            draggingPoint = (System.Windows.Shapes.Ellipse)sender;
            draggingPoint_firstPos = new Point(draggingPoint.Margin.Left, draggingPoint.Margin.Top);
        }

        /// <summary>
        /// Manipulace s useckou
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            draggingLine = (System.Windows.Shapes.Line)sender;
        }

       
        /// <summary>
        /// Po zavreni okna uvolnime kameru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        /// <summary>
        /// Kliknuti na tlacitko RESET vyresetuje nastaveni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetBut_Click(object sender, RoutedEventArgs e)
        {
            pointCounter = 0;
            imageBox.Children.Clear();
            if (mode == CalibrationMode.RECTANGLE) perspective = 1;
            CaptureImage();
        }

        /// <summary>
        /// Kliknuti na tlacitko OK ulozi provedene zmeny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKBut_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Chcete uložit změny?", "Změna nastavení", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                SaveValues();
            }
            // pokud je stiskly SHIFT, pouze se ulozi zmeny
            if(!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))  this.Close();
        }

        /// <summary>
        /// Kliknuti na tlacitko ZACHYTIT zachyti novy obrazek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frameBut_Click(object sender, RoutedEventArgs e)
        {
            CaptureImage();
        }

        /// <summary>
        /// Kliknuti na tlacitko ROTOVAT orotuje obrazek doprava
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotateBut3_Click(object sender, RoutedEventArgs e)
        {
            rotation = (rotation + 3) % 360;
            CaptureImage();
        }

        /// <summary>
        /// Kliknuti na tlacitko ROTOVAT orotuje obrazek doleva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotateBut30_Click(object sender, RoutedEventArgs e)
        {
            rotation = (rotation + 30) % 360;
            CaptureImage();
        }

        /// <summary>
        /// Zmena perspektivniho modu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rectangleRadio_Checked(object sender, RoutedEventArgs e)
        {
            // pokud byl predchozi mod perspective ,musime zacit odznova
            if (mode == CalibrationMode.PERSPECTIVE) resetBut_Click(null, null);
            this.mode = CalibrationMode.RECTANGLE;
            perspective = 1;
        }

        /// <summary>
        /// Zmena perspektivniho modu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
