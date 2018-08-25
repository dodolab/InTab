using System;
using System.Collections.Generic;
using InteractiveTable.GUI.Table;
using InteractiveTable.Managers;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Accessories;
using InteractiveTable.GUI.Other;

namespace InteractiveTable.Controls
{
    /// <summary>
    /// Controller for the simulator window
    /// </summary>
    public class TableController
    {
        #region var, get, set, const

        private TablePanel tablePanel;
        private TableManager tableManager;
        private HashSet<Image> rockImages;

        public TableController()
        {
            rockImages = new HashSet<Image>();
        }

        public TablePanel TablePanel
        {
            get { return tablePanel; }
            set { this.tablePanel = value; }
        }
        
        public TableManager TableManager
        {
            get { return tableManager; }
            set { this.tableManager = value; }
        }


        #endregion

        #region functions

        /// <summary>
        /// Recalculates positions of stones based on the feedback from camera
        /// </summary>
        public void RecalculateStones()
        {
            HashSet<Image> image_to_delete = new HashSet<Image>();

            foreach (Image img in rockImages)
            {
                FPoint pos = ((A_Rock)img.DataContext).Position;
                FPoint newRock_pos = PointHelper.TransformPointToFrame(pos, CommonAttribService.ACTUAL_TABLE_WIDTH, CommonAttribService.ACTUAL_TABLE_HEIGHT);
                img.Margin = new Thickness(newRock_pos.X / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER - img.Width / 2, newRock_pos.Y / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER - img.Height / 2, 0, 0);

                // stone has moved to an unknown place -> delete it
                if (img.Margin.Left <= 0 || img.Margin.Left >= (CommonAttribService.ACTUAL_TABLE_WIDTH / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER)
                    || img.Margin.Top <= 0 || img.Margin.Top >= (CommonAttribService.ACTUAL_TABLE_HEIGHT / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER))
                {
                    image_to_delete.Add(img);
                }
            }

            // remove stones
            foreach (Image img in image_to_delete)
            {
                tableManager.RemoveObject(((A_TableObject)img.DataContext));
                tablePanel.mainGrid.Children.Remove(img);
                rockImages.Remove(img);
            }
        }

        /// <summary>
        /// Sets prototypes of all stones for the bottom bar
        /// </summary>
        private void SetToolContext()
        {
            Graviton rck = new Graviton();
            Generator gn = new Generator();
            Magneton mg = new Magneton();
            BlackHole blh = new BlackHole();
            
            tablePanel.tableToolPanel.generatorRockImage.DataContext = gn;
            tablePanel.tableToolPanel.gravityRockImage.DataContext = rck;
            tablePanel.tableToolPanel.magnetonRockImage.DataContext = mg;
            tablePanel.tableToolPanel.blackHoleRockImage.DataContext = blh;
        }

        /// <summary>
        /// Returns true, if the mouse pointer is within a game board
        /// </summary>
        /// <returns></returns>
        private Boolean MouseInTableArea(System.Windows.Input.MouseButtonEventArgs e, Image img)
        {
            Grid tableGrid = tablePanel.tableAreaPanel.tableGrid as Grid;

            Point tablePosMouse = e.GetPosition(tableGrid);
            Point imagePosMouse = e.GetPosition(img);
            Point tablePosition = new Point(tablePosMouse.X + img.Width - imagePosMouse.X, tablePosMouse.Y + img.Height - imagePosMouse.Y);

            return ((tablePosMouse.X - imagePosMouse.X) >= 0 &&
                    (tablePosMouse.Y - imagePosMouse.Y) >= 0 &&
                     tablePosition.X <= tableGrid.Width &&
                     tablePosition.Y <= tableGrid.Height);
        }


        /// <summary>
        /// Returns a direction to which the stone cannot move anymore
        /// 0 = anywhere
        /// 1 = blocked from left
        /// 2 = blocked from top
        /// 3 = blocked from right
        /// 4 = blocked from bottom
        /// </summary>
        /// <returns></returns>
        private int MouseInTableArea(System.Windows.Input.MouseEventArgs e, Image img)
        {
            Grid tableGrid = tablePanel.tableAreaPanel.tableGrid as Grid;

            Point tablePosMouse = e.GetPosition(tableGrid);
            Point imagePosMouse = e.GetPosition(img);
            Point tablePosition = new Point(tablePosMouse.X + img.Width - imagePosMouse.X, tablePosMouse.Y + img.Height - imagePosMouse.Y);

            if ((tablePosMouse.X - imagePosMouse.X) < 0) return 1;
            if ((tablePosMouse.Y - imagePosMouse.Y) < 0) return 2;
            if (tablePosition.X > tableGrid.Width) return 3;
            if (tablePosition.Y > tableGrid.Height) return 0;
            return 0;
        }

        /// <summary>
        /// Adds a stone to a gameboard
        /// </summary>
        private void InsertRockImage(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MouseInTableArea(e, dragImage_new))
            {
                int left = (int)e.GetPosition(tablePanel).X - dragImage_mousePos_X;
                int top = (int)e.GetPosition(tablePanel).Y - dragImage_mousePos_Y;
                
                // sets handler so that we can manipulate with the object
                dragImage_new.MouseDown += new System.Windows.Input.MouseButtonEventHandler(RockImageOld_MouseDown);
                dragImage_new.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rockImage_MouseUp);
                
                // check the type of the object to insert
                A_TableObject object_to_put = null;
                if ((A_TableObject)dragImage_new.DataContext is Graviton)
                {
                    object_to_put = new Graviton();
                    ((Graviton)object_to_put).Name = "Graviton";
                    ((Graviton)object_to_put).BaseSettings = tableManager.TableDepositor.table.Settings.gravitonSettings;
                }
                if ((A_TableObject)dragImage_new.DataContext is Generator)
                {
                    object_to_put = new Generator();
                    ((Generator)object_to_put).Name = "Generator";
                    ((Generator)object_to_put).BaseSettings = tableManager.TableDepositor.table.Settings.generatorSettings;
                }
                if ((A_TableObject)dragImage_new.DataContext is Magneton)
                {
                    object_to_put = new Magneton();
                    ((Magneton)object_to_put).Name = "Magneton";
                    ((Magneton)object_to_put).BaseSettings = tableManager.TableDepositor.table.Settings.magnetonSettings;
                }
                if ((A_TableObject)dragImage_new.DataContext is BlackHole)
                {
                    object_to_put = new BlackHole();
                    ((BlackHole)object_to_put).Name = "BlackHole";
                    ((BlackHole)object_to_put).BaseSettings = tableManager.TableDepositor.table.Settings.blackHoleSettings;
                }

                // sets position
                object_to_put.Position = PointHelper.TransformPointToEuc(new FPoint(left * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + dragImage_new.Width / 2,
                    top * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + dragImage_new.Height / 2), CommonAttribService.ACTUAL_TABLE_WIDTH,
                    CommonAttribService.ACTUAL_TABLE_HEIGHT);

                dragImage_new.DataContext = object_to_put;
                // add it into a system
                tableManager.InsertObject(object_to_put);
                rockImages.Add(dragImage_new);
            }
            else
            {
                // icon is beyond the border of the game table -> delete it
                if (tablePanel.mainGrid.Children.Contains(dragImage_new))
                {
                    tablePanel.mainGrid.Children.Remove(dragImage_new);
                }
            }
            dragImage_new = null;
        }


        #endregion

        #region handlers
        
        public void SetHandlers()
        {

            tablePanel.tableToolPanel.generatorRockImage.MouseDown += new System.Windows.Input.MouseButtonEventHandler(RockImageNew_MouseDown);
            tablePanel.tableToolPanel.gravityRockImage.MouseDown += new System.Windows.Input.MouseButtonEventHandler(RockImageNew_MouseDown);
            tablePanel.tableToolPanel.magnetonRockImage.MouseDown += new System.Windows.Input.MouseButtonEventHandler(RockImageNew_MouseDown);
            tablePanel.tableToolPanel.blackHoleRockImage.MouseDown += new System.Windows.Input.MouseButtonEventHandler(RockImageNew_MouseDown);

            tablePanel.mainGrid.MouseMove += new System.Windows.Input.MouseEventHandler(mainGrid_MouseMove);
            tablePanel.mainGrid.LostFocus += new System.Windows.RoutedEventHandler(mainGrid_LostFocus);
            tablePanel.mainGrid.MouseLeave += new System.Windows.Input.MouseEventHandler(mainGrid_MouseLeave);
            tablePanel.mainGrid.MouseUp += new System.Windows.Input.MouseButtonEventHandler(mainGrid_MouseUp);
            tablePanel.SizeChanged += new SizeChangedEventHandler(tablePanel_SizeChanged);
            
            // initialize tools (bottom bar with stone prototypes)
            SetToolContext();
        }

        /// <summary>
        /// Recalculate size of the game board based on the size of the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tablePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // handle view
            tablePanel.tableAreaPanel.Width = tablePanel.tableAreaPanel.Width + e.NewSize.Width - e.PreviousSize.Width;
            tablePanel.tableAreaPanel.Height = tablePanel.tableAreaPanel.Height + e.NewSize.Height - e.PreviousSize.Height;
            tablePanel.tableAreaPanel.tableGrid.Width = tablePanel.tableAreaPanel.tableGrid.Width + (e.NewSize.Width - e.PreviousSize.Width);
            tablePanel.tableAreaPanel.tableGrid.Height = tablePanel.tableAreaPanel.tableGrid.Height + (e.NewSize.Height - e.PreviousSize.Height);


            // handle model
            FRectangle tableRect = ((FRectangle)((Table)tableManager.TableDepositor.table).Shape);
            tableRect.Width = tablePanel.tableAreaPanel.tableGrid.Width;
            tableRect.Height = tablePanel.tableAreaPanel.tableGrid.Height;

            // set margins of all stones
            foreach (Image img in rockImages)
            {
                img.Margin =
                    new Thickness(img.Margin.Left * ((CommonAttribService.ACTUAL_TABLE_WIDTH / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER) / tableRect.Width),
                        img.Margin.Top / ((CommonAttribService.ACTUAL_TABLE_HEIGHT / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER) / tableRect.Height), 0, 0);

                ((A_Rock)img.DataContext).Position =
                    PointHelper.TransformPointToEuc(new FPoint(img.Margin.Left * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + img.Width / 2,
                        img.Margin.Top * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + img.Height / 2), tableRect.Width, tableRect.Height);
            }

            CommonAttribService.ACTUAL_TABLE_WIDTH = (int)(tablePanel.tableAreaPanel.tableGrid.Width * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER);
            CommonAttribService.ACTUAL_TABLE_HEIGHT = (int)(tablePanel.tableAreaPanel.tableGrid.Height * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER);
        }


        /// <summary>
        /// Mouse goes up over an existing ston
        /// </summary>
        private void rockImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                // display stone edit dialog
                RockEditorWindow rcked = new RockEditorWindow();
                rcked.InitData((A_Rock)((Image)sender).DataContext);
                rcked.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                rcked.Owner = CommonAttribService.mainWindow;
                rcked.ShowDialog();
            }
            else
            {
                // if the mouse is over a bin, delete the stone
                Point thrashPoint = e.GetPosition(tablePanel.tableToolPanel.thrashImage);
                if (thrashPoint.X >= 0 && thrashPoint.X <= tablePanel.tableToolPanel.thrashImage.Width &&
                    thrashPoint.Y >= 0 && thrashPoint.Y <= tablePanel.tableToolPanel.thrashImage.Height)
                {
                    // delete the stone
                    tablePanel.mainGrid.Children.Remove((Image)sender);
                    rockImages.Remove((Image)sender);
                    tableManager.RemoveObject((A_TableObject)((Image)sender).DataContext);
                }
            }
        }
        
        // currently dragged image
        private Image dragImage_old;

        /// <summary>
        /// Click on an existing stone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RockImageOld_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragImage_old = (Image)sender;
            dragImage_mousePos_X = (int)e.GetPosition((Image)sender).X; // update position of the image based on the mouse pos
            dragImage_mousePos_Y = (int)e.GetPosition((Image)sender).Y;
        }

        /// <summary>
        /// Mouse up over a main panel invokes adding a new stone into a gameboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dragImage_new != null)
            {
                // user has manipulated with a stone -> add it into a game board
                InsertRockImage(e);
            }

            if (dragImage_old != null)
            {
                dragImage_old = null;
            }
        }

        /// <summary>
        /// Mouse leaves the main panel -> delete dragged stone
        /// </summary>
        private void mainGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (tablePanel.mainGrid.Children.Contains(dragImage_new))
                tablePanel.mainGrid.Children.Remove(dragImage_new);
            dragImage_new = null;
            dragImage_old = null;
        }

        /// <summary>
        /// If the main panel loses its focus, the dragged stone will disappear
        /// </summary>
        private void mainGrid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tablePanel.mainGrid.Children.Contains(dragImage_new))
                tablePanel.mainGrid.Children.Remove(dragImage_new);
            dragImage_new = null;
            dragImage_old = null;

        }

        /// <summary>
        /// Move over a grid
        /// </summary>
        private void mainGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (dragImage_new != null || dragImage_old != null) mainGrid_dragImageMove(e);
            }
        }

        /// <summary>
        /// Moving the stone from the bottombar to a gameboard with the mouse pointer
        /// </summary>
        /// <param name="e"></param>
        private void mainGrid_dragImageMove(System.Windows.Input.MouseEventArgs e)
        {
            double left = (int)e.GetPosition(tablePanel).X - dragImage_mousePos_X;
            double top = (int)e.GetPosition(tablePanel).Y - dragImage_mousePos_Y;

            // dragging a new stone
            if (dragImage_new != null) dragImage_new.Margin = new System.Windows.Thickness(left, top, 0, 0);
            // dragging a stone that has been already added to a gameboard
            if (dragImage_old != null)
            {

                double left_old = (int)dragImage_old.Margin.Left; 
                double top_old = (int)dragImage_old.Margin.Top;

                // already added stone cannot go beyond the game board
                int dragDirection = MouseInTableArea(e, dragImage_old);

                if (dragDirection == 0 ||
                   (dragDirection == 1 && left >= left_old) ||
                   (dragDirection == 2 && top >= top_old) ||
                   (dragDirection == 3 && left <= left_old) ||
                   (dragDirection == 4 && top <= top_old))
                {
                    dragImage_old.Margin = new System.Windows.Thickness(left, top, 0, 0);
                    ((A_TableObject)dragImage_old.DataContext).Position = PointHelper.TransformPointToEuc(
                        new FPoint(left * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + dragImage_old.Width / 2,
                            top * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + dragImage_old.Height / 2), CommonAttribService.ACTUAL_TABLE_WIDTH,
                   CommonAttribService.ACTUAL_TABLE_HEIGHT);

                }
            }
        }

        // image of a stone that is to be dragged
        private Image dragImage_new;
        // mouse position relative to an image of a stone
        private int dragImage_mousePos_X, dragImage_mousePos_Y;

        /// <summary>
        /// Click on a stone in the bottombar
        /// </summary>
        private void RockImageNew_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragImage_new = new Image(); // create a new image that will be dragged over the game board
            dragImage_new.Source = ((Image)sender).Source.Clone();
            dragImage_new.DataContext = ((Image)sender).DataContext; 
            dragImage_new.Name = ((Image)sender).Name;
            dragImage_new.Width = ((Image)sender).Width; 
            dragImage_new.Height = ((Image)sender).Height;
            dragImage_new.Margin = new System.Windows.Thickness(-100);
            dragImage_new.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            dragImage_new.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            dragImage_mousePos_X = (int)e.GetPosition((Image)sender).X; 
            dragImage_mousePos_Y = (int)e.GetPosition((Image)sender).Y;
            tablePanel.mainGrid.Children.Add(dragImage_new); 
        }

        #endregion
    }
}
