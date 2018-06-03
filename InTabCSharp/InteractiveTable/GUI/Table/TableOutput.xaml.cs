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
using System.Windows.Navigation;
using System.Windows.Shapes;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Accessories;
using InteractiveTable.GUI.Other;
using System.Windows.Media.Effects;
using InteractiveTable.Managers;
using System.IO;

namespace InteractiveTable.GUI.Table
{
    /// <summary>
    /// View pro vystupni okno
    /// </summary>
    public partial class TableOutput : Window
    {
        private TableDrawingManager tableDrawingManager;
        private int counter = 0;
        private Boolean maximized = false; // pokud true, je okno ve fullscreenu
        private Size preMaximized_size = new Size(0, 0); // velikost pred maximalizaci

        public TableOutput()
        {
            InitializeComponent();
            this.MouseDoubleClick += new MouseButtonEventHandler(TableOutput_MouseDoubleClick);
            tableDrawingManager = new TableDrawingManager();
        }

        /// <summary>
        /// Dvojitym klikem se okno maximalizuje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableOutput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (maximized)
            {
                this.WindowStyle = WindowStyle.ThreeDBorderWindow;
                this.WindowState = WindowState.Normal;
                if (preMaximized_size.Height != 0 && preMaximized_size.Width != 0)
                {
                    this.Width = preMaximized_size.Width;
                    this.Height = preMaximized_size.Height;
                }
                else
                {
                    this.Width = 800;
                    this.Height = 600;
                }
            }
            else
            {
                preMaximized_size = new Size(Width, Height);
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            maximized = !maximized;
        }

        /// <summary>
        /// Ulozi obrazek do souboru
        /// pouze pro debugovaci ucely
        /// </summary>
        /// <param name="objImage"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Guid SavePhoto(BitmapSource objImage, string path)
        {
            Guid photoID = System.Guid.NewGuid();

            FileStream filestream = new FileStream(path + ".jpg", FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 100;
            encoder.Frames.Add(BitmapFrame.Create(objImage));
            encoder.Save(filestream);

            return photoID;
        }

        
        /// <summary>
        /// Vykresli soustavu na plochu
        /// </summary>
        /// <param name="objects"></param>
        public void Repaint(TableDepositor objects)
        {
            if (CommonAttribService.OUTPUT_DRAW_ALLOWED)
            {
                BitmapSource bmp = null;
                if (GraphicsSettings.Instance().OUTPUT_TABLE_SIZE_DEPENDENT)
                {
                    bmp = tableDrawingManager.CreateBitmap(objects, true);
                }
                else
                {
                    if (((int)tableDrawingManager.actual_size.X) != CommonAttribService.ACTUAL_OUTPUT_WIDTH)
                        tableDrawingManager.Resize(CommonAttribService.ACTUAL_OUTPUT_WIDTH,
                            (int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * (((double)CommonAttribService.ACTUAL_OUTPUT_WIDTH) / CommonAttribService.ACTUAL_TABLE_WIDTH)));
                    bmp = tableDrawingManager.CreateBitmap(objects, false);
                }
                if (bmp != null)
                {
                    tableImage.Source = bmp;
                    tableImage.Width = ActualWidth;
                    tableImage.Height = ActualHeight;
                }
            }
        }
    }
}
