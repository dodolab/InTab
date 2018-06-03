using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.GUI.Table;
using InteractiveTable.Managers;
using System.Windows.Controls;
using System.Windows;
using InteractiveTable.Settings;
using System.Windows.Input;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Accessories;
using InteractiveTable.GUI.Other;

namespace InteractiveTable.Controls
{
    /// <summary>
    /// Controller pro simulacni okno
    /// </summary>
    public class TableController
    {
        #region promenne, gettery, settery, konstruktory

        private TablePanel tablePanel;
        private TableManager tableManager;
        private HashSet<Image> rockImages;

        public TableController()
        {
            rockImages = new HashSet<Image>();
        }

        /// <summary>
        /// Vrati nebo nastavi odkaz na panel s kameny
        /// </summary>
        public TablePanel TablePanel
        {
            get { return tablePanel; }
            set { this.tablePanel = value; }
        }

        /// <summary>
        /// Vrati nebo nastavi odkaz na manazera stolu
        /// </summary>
        public TableManager TableManager
        {
            get { return tableManager; }
            set { this.tableManager = value; }
        }


        #endregion

        #region funkce

        /// <summary>
        /// Prepocita pozici kamenu-obrazku
        /// </summary>
        public void RecalculateStones()
        {
            HashSet<Image> image_to_delete = new HashSet<Image>();

            foreach (Image img in rockImages)
            {
                FPoint pos = ((A_Rock)img.DataContext).Position;
                FPoint newRock_pos = PointHelper.TransformPointToFrame(pos, CommonAttribService.ACTUAL_TABLE_WIDTH, CommonAttribService.ACTUAL_TABLE_HEIGHT);
                img.Margin = new Thickness(newRock_pos.X / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER - img.Width / 2, newRock_pos.Y / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER - img.Height / 2, 0, 0);

                // pokud se kamen posunul tam, kam nema, smazeme ho
                if (img.Margin.Left <= 0 || img.Margin.Left >= (CommonAttribService.ACTUAL_TABLE_WIDTH / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER)
                    || img.Margin.Top <= 0 || img.Margin.Top >= (CommonAttribService.ACTUAL_TABLE_HEIGHT / CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER))
                {
                    image_to_delete.Add(img);
                }
            }

            // musime to smazat zde, aby to nevyhodilo vyjimku pri modifikaci ve foreach
            foreach (Image img in image_to_delete)
            {
                tableManager.RemoveObject(((A_TableObject)img.DataContext));
                tablePanel.mainGrid.Children.Remove(img);
                rockImages.Remove(img);
            }
        }

        /// <summary>
        /// Prida obrazkum kamenu na liste do dataContext objekty, se kterymi se poji
        /// </summary>
        private void SetToolContext()
        {
            Graviton rck = new Graviton();
            Generator gn = new Generator();
            Magneton mg = new Magneton();
            BlackHole blh = new BlackHole();

            //=====================
            //tady se budou nastavovat
            // jeste nejake doplnujic
            // vlastnosti tech kamenu

            tablePanel.tableToolPanel.generatorRockImage.DataContext = gn;
            tablePanel.tableToolPanel.gravityRockImage.DataContext = rck;
            tablePanel.tableToolPanel.magnetonRockImage.DataContext = mg;
            tablePanel.tableToolPanel.blackHoleRockImage.DataContext = blh;
        }

        /// <summary>
        /// Vrati true, pokud je ukazatel mysi na herni plose
        /// </summary>
        /// <param name="e"></param>
        /// <param name="img"></param>
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
        /// Vrati zakazany smer, kam uz se kamen nesmi posunout
        /// 0 = muze kamkoliv
        /// 1 = nesmi DOLEVA
        /// 2 = nesmi NAHORU
        /// 3 = nesmi DOPRAVA
        /// 4 = nesmi DOLU
        /// </summary>
        /// <param name="e"></param>
        /// <param name="img"></param>
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
        /// Pridani kamene na plochu
        /// </summary>
        /// <param name="e"></param>
        private void InsertRockImage(System.Windows.Input.MouseButtonEventArgs e)
        {
            // zkontrolujeme, zda se nachazime na herni plose a pridame kamen
            if (MouseInTableArea(e, dragImage_new))
            {
                int left = (int)e.GetPosition(tablePanel).X - dragImage_mousePos_X;
                int top = (int)e.GetPosition(tablePanel).Y - dragImage_mousePos_Y;

                // nastaveni handleru, abychom s timto objektem mohli manipulovat
                dragImage_new.MouseDown += new System.Windows.Input.MouseButtonEventHandler(RockImageOld_MouseDown);
                dragImage_new.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rockImage_MouseUp);

                //kamen byl pridan, jeste o tom ale dame vedet tableManageru, ktery o tom informuje fyz. engine

                // zjistime, co je to za objekt a vytvorime ho znovu
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

                // nastavime mu pozici
                object_to_put.Position = PointHelper.TransformPointToEuc(new FPoint(left * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + dragImage_new.Width / 2,
                    top * CommonAttribService.ACTUAL_TABLE_SIZE_MULTIPLIER + dragImage_new.Height / 2), CommonAttribService.ACTUAL_TABLE_WIDTH,
                    CommonAttribService.ACTUAL_TABLE_HEIGHT);

                // priradime ho zpet do obrazku kamene
                dragImage_new.DataContext = object_to_put;
                // vlozime ho do soustavy
                tableManager.InsertObject(object_to_put);
                rockImages.Add(dragImage_new);
            }
            else
            {
                // smazeme obrazek z mainGridu, protoze je ve spatne pozici
                if (tablePanel.mainGrid.Children.Contains(dragImage_new))
                {
                    tablePanel.mainGrid.Children.Remove(dragImage_new);
                }
            }
            dragImage_new = null;
        }


        #endregion

        #region handlery

        /// <summary>
        /// Nastavi handlery pro stul
        /// </summary>
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

            // zavola se inicializace prvotnich kamenu
            SetToolContext();
        }

        /// <summary>
        /// Zvetseni tableAreaPanelu zpusobi, ze se zvetsi pouze kreslici plocha, to se tady musi osetrit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tablePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // zvetseni stolu (view)
            tablePanel.tableAreaPanel.Width = tablePanel.tableAreaPanel.Width + e.NewSize.Width - e.PreviousSize.Width;
            tablePanel.tableAreaPanel.Height = tablePanel.tableAreaPanel.Height + e.NewSize.Height - e.PreviousSize.Height;
            tablePanel.tableAreaPanel.tableGrid.Width = tablePanel.tableAreaPanel.tableGrid.Width + (e.NewSize.Width - e.PreviousSize.Width);
            tablePanel.tableAreaPanel.tableGrid.Height = tablePanel.tableAreaPanel.tableGrid.Height + (e.NewSize.Height - e.PreviousSize.Height);


            // zvetseni stolu (model)
            FRectangle tableRect = ((FRectangle)((Table)tableManager.TableDepositor.table).Shape);
            tableRect.Width = tablePanel.tableAreaPanel.tableGrid.Width;
            tableRect.Height = tablePanel.tableAreaPanel.tableGrid.Height;

            // upraveni polohy kamenu (obrazku)
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
        /// Uvolneni tlacitka mysi nad jiz pridanym kamenem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rockImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // prave tlacitko mysi zpristupni okno s nastavenim
            if (e.ChangedButton == MouseButton.Right)
            {
                RockEditorWindow rcked = new RockEditorWindow();
                rcked.InitData((A_Rock)((Image)sender).DataContext);
                rcked.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                rcked.Owner = CommonAttribService.mainWindow;
                rcked.ShowDialog();
            }
            else
            {

                // pokud je mys nad kosem, smaz kamen
                Point thrashPoint = e.GetPosition(tablePanel.tableToolPanel.thrashImage);
                if (thrashPoint.X >= 0 && thrashPoint.X <= tablePanel.tableToolPanel.thrashImage.Width &&
                    thrashPoint.Y >= 0 && thrashPoint.Y <= tablePanel.tableToolPanel.thrashImage.Height)
                {
                    // kamen vymazeme
                    tablePanel.mainGrid.Children.Remove((Image)sender);
                    rockImages.Remove((Image)sender);
                    tableManager.RemoveObject((A_TableObject)((Image)sender).DataContext);
                }
            }
        }

        // kamen na plose, se kterym je aktualne manipulovano
        private Image dragImage_old;

        /// <summary>
        /// Kliknuti mysi na kamen, ktery jiz je na plose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RockImageOld_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragImage_old = (Image)sender; // nastavime odkaz na kamen, se kterym je provedena interakce
            dragImage_mousePos_X = (int)e.GetPosition((Image)sender).X; // nastaveni rozmeru vuci mysi
            dragImage_mousePos_Y = (int)e.GetPosition((Image)sender).Y;
        }

        /// <summary>
        /// Uvolneni tlacitka mysi nad hlavnim panelem zpusobi, ze se prida novy kamen (pokud tam nejaky je)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // pokud je odkaz na dragImage, uzivatel manipuloval s kamenem
            if (dragImage_new != null)
            {
                InsertRockImage(e);
            }

            if (dragImage_old != null)
            {
                dragImage_old = null;
            }
        }

        /// <summary>
        /// Mys opusti hlavnihlavni panel v TablePanel - pokud tahne nejaky kamen, smaze se
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //pokud uzivatel presouva kamen z listy a pote se dostane mimo panel, kamen zmizi
            if (tablePanel.mainGrid.Children.Contains(dragImage_new))
                tablePanel.mainGrid.Children.Remove(dragImage_new);
            dragImage_new = null;
            dragImage_old = null;
        }

        /// <summary>
        /// Nastane, pokud hlavni panel v TablePanel ztrati fokus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainGrid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //pokud uzivatel presouva kamen z listy a panel ztrati focus, tak musi zmizet
            if (tablePanel.mainGrid.Children.Contains(dragImage_new))
                tablePanel.mainGrid.Children.Remove(dragImage_new);
            dragImage_new = null;
            dragImage_old = null;

        }

        /// <summary>
        /// Pohyb mysi po plose, rozdeleni na nekolik moznych akci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (dragImage_new != null || dragImage_old != null) mainGrid_dragImageMove(e);
            }
        }

        /// <summary>
        /// Premistovani kamenu z listy po plose
        /// </summary>
        /// <param name="e"></param>
        private void mainGrid_dragImageMove(System.Windows.Input.MouseEventArgs e)
        {
            double left = (int)e.GetPosition(tablePanel).X - dragImage_mousePos_X;
            double top = (int)e.GetPosition(tablePanel).Y - dragImage_mousePos_Y;

            // hybani s novym kamenem (jeste nebyl pridan)
            if (dragImage_new != null) dragImage_new.Margin = new System.Windows.Thickness(left, top, 0, 0);
            // hybani s jiz pridanym kamenem, krome polohy obrazku se meni i jeho fyzicka poloha
            if (dragImage_old != null)
            {

                double left_old = (int)dragImage_old.Margin.Left; // stary margin, slouzi pro porovnani
                double top_old = (int)dragImage_old.Margin.Top;

                // obrazek jiz pridany muze mit pozici jen v ramci plochy
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

        // obrazek kamenu, ktery se bude presouvat
        private Image dragImage_new;
        // pozice mysi vzhledem k obrazku kamenu -> aby se vykresloval ve stejne vzd. od mysi
        private int dragImage_mousePos_X, dragImage_mousePos_Y;

        /// <summary>
        /// Kliknuti na gravitacni kamen na liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RockImageNew_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragImage_new = new Image(); // vytvoreni noveho obrazku, ktery se bude posouvat po liste
            dragImage_new.Source = ((Image)sender).Source.Clone();
            dragImage_new.DataContext = ((Image)sender).DataContext; // kopirovani kontextu
            dragImage_new.Name = ((Image)sender).Name;
            dragImage_new.Width = ((Image)sender).Width; // nastaveni parametru (rozmery, pocatecni umisteni)
            dragImage_new.Height = ((Image)sender).Height;
            dragImage_new.Margin = new System.Windows.Thickness(-100);
            dragImage_new.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            dragImage_new.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            dragImage_mousePos_X = (int)e.GetPosition((Image)sender).X; // nastaveni rozmeru vuci mysi
            dragImage_mousePos_Y = (int)e.GetPosition((Image)sender).Y;
            tablePanel.mainGrid.Children.Add(dragImage_new); // pridani do panelu v TablePanel
        }

        #endregion
    }
}
