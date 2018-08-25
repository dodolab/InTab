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
using System.Windows.Navigation;
using System.Windows.Shapes;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Physics.System;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Managers;
using InteractiveTable.Controls;

namespace InteractiveTable.GUI.Table
{
    /// <summary>
    /// Simulator window
    /// </summary>
    public partial class TableAreaPanel : UserControl
    {
        private TableDrawingManager tableDrawingManager;
        private TableController tableController;
        // current image being rendered
        private Image tableImage;

        public TableAreaPanel()
        {
            InitializeComponent();
            tableDrawingManager = new TableDrawingManager();
        }

        public TableController TableController
        {
            set { this.tableController = value; }
            get { return tableController; }
        }


        /// <summary>
        /// Renders the system into the window
        /// </summary>
        /// <param name="objects"></param>
        public void Repaint(TableDepositor objects)
        {
            if (CommonAttribService.SIMULATION_DRAW_ALLOWED)
            {
                BitmapSource bmp = null;
                // if both outputs are enabled, let's just take the picture from the OpenGL window
                if (CommonAttribService.OUTPUT_DRAW_ALLOWED && !CommonAttribService.MODE_2D)
                {
                    bmp = CommonAttribService.LAST_BITMAP;
                }
                else
                {
                    bmp = tableDrawingManager.CreateBitmap(objects, true);
                }
                if (bmp != null)
                {
                    if(tableImage == null )tableImage = new Image();
                    tableImage.Source = bmp;
                    tableImage.Width = ActualWidth;
                    tableImage.Height = ActualHeight;
                    tableImage.HorizontalAlignment = HorizontalAlignment.Left;
                    tableImage.VerticalAlignment = VerticalAlignment.Top;
                    if(!tableGrid.Children.Contains(tableImage)) tableGrid.Children.Add(tableImage);
                }
            }
        }
    }
}
