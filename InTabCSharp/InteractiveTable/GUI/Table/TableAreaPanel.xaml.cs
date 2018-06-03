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
    /// Simulacni okno stolu
    /// </summary>
    public partial class TableAreaPanel : UserControl
    {
        // vykreslovaci manazer
        private TableDrawingManager tableDrawingManager;
        // controller pro komponentu
        private TableController tableController;
        // aktualne vykresleny obrazek
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
        /// Vykresli soustavu na plochu
        /// </summary>
        /// <param name="objects"></param>
        public void Repaint(TableDepositor objects)
        {
            if (CommonAttribService.SIMULATION_DRAW_ALLOWED)
            {
                BitmapSource bmp = null;
                // pokud jsou zaple oba vystupy, budeme si brat bitmapu z toho druheho, neni treba ji vytvaret znovu
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
