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
using InteractiveTable.Settings;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using InteractiveTable.Managers;
using System.Xml.Serialization;

namespace InteractiveTable.GUI.Other
{
    /// <summary>
    /// MVC pro nastaveni barevnych prechodu castic
    /// </summary>
    public partial class ParticleColorSetWindow : Window
    {
        #region variables

        // znacky reprezentujici RGBA barvu s pozici
        private HashSet<Ellipse> fadeMarks = new HashSet<Ellipse>();
        // prave oznacena znacka
        private Ellipse selectedEllipse;
        // barva oznacene znacky
        private RadialGradientBrush selectedBrush;
        // barva neoznacene znacky
        private RadialGradientBrush unselectedBrush;
        // list barev
        private LinkedList<FadeColor> colors;

        #endregion

        #region constructors

        /// <summary>
        /// Otevre nove okno s nastavenim castic; nastavi veskere handlery a hodnoty
        /// </summary>
        public ParticleColorSetWindow()
        {
            InitializeComponent();
            // nastaveni barev u znacek, musi byt kruhovy prechod, aby
            // nebyl problem s ruznym pozadim
            unselectedBrush = new RadialGradientBrush();
            unselectedBrush.GradientOrigin = new Point(0.5, 0.5);
            unselectedBrush.GradientStops.Add(new GradientStop(Colors.Black, 0.0));
            unselectedBrush.GradientStops.Add(new GradientStop(Colors.Gray, 1.0));
            selectedBrush = new RadialGradientBrush();
            selectedBrush.GradientOrigin = new Point(0.5, 0.5);
            selectedBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            selectedBrush.GradientStops.Add(new GradientStop(Colors.Gray, 1.0));

            if (CommonAttribService.DEFAULT_FADE_COLORS != null) LoadColors(CommonAttribService.DEFAULT_FADE_COLORS);

            // nastaveni handleru
            this.MouseUp += new MouseButtonEventHandler(ParticleColorSetWindow_MouseUp);
            alphaSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(alphaSlider_ValueChanged);
            redSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(redSlider_ValueChanged);
            greenSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(greenSlider_ValueChanged);
            blueSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(blueSlider_ValueChanged);
            alphaTbx.TextChanged += new TextChangedEventHandler(alphaTbx_TextChanged);
            alphaTbx.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(alphaTbx_LostKeyboardFocus);
            redTbx.TextChanged += new TextChangedEventHandler(redTbx_TextChanged);
            redTbx.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(redTbx_LostKeyboardFocus);
            blueTbx.TextChanged += new TextChangedEventHandler(blueTbx_TextChanged);
            blueTbx.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(blueTbx_LostKeyboardFocus);
            greenTbx.TextChanged += new TextChangedEventHandler(greenTbx_TextChanged);
            greenTbx.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(greenTbx_LostKeyboardFocus);
        }

        #endregion

        #region logic

        /// <summary>
        /// Nacte barvy
        /// </summary>
        /// <param name="colors"></param>
        public void LoadColors(LinkedList<FadeColor> colors)
        {
            foreach(Ellipse el in fadeMarks) paintGrid.Children.Remove(el);
            fadeMarks.Clear();   // vycisti znacky

            this.colors = colors;

            //pokud zde NENI barva se znackou 0 a 1, musi se dorovnat
            if (colors.Count(col => col.position == 0) == 0) colors.Where(col => col.position == colors.Min(col2 => col2.position)).First().position = 0;
            if (colors.Count(col => col.position == 1) == 0) colors.Where(col => col.position == colors.Max(col2 => col2.position)).First().position = 1;

            // pro kazdou barvu vytvor znacku
            foreach (FadeColor color in colors)
            {
                Ellipse mark = new Ellipse();
                mark.Width = 8;
                mark.Height = 8;
                mark.Margin = new Thickness(color.position * fadeRect.Width, 0, 0, 0);
                mark.Margin = new Thickness(color.position * fadeRect.Width, 0, 0, 0);

                mark.HorizontalAlignment = HorizontalAlignment.Left;
                mark.VerticalAlignment = VerticalAlignment.Bottom;
                mark.DataContext = color;
                mark.Fill = unselectedBrush;
                mark.MouseDown += new MouseButtonEventHandler(mark_MouseDown);
                mark.MouseUp += new MouseButtonEventHandler(mark_MouseUp);
                mark.MouseMove += new MouseEventHandler(mark_MouseMove);
                Grid.SetRow(mark, 1);
                paintGrid.Children.Add(mark);
                fadeMarks.Add(mark);

            }
            RepaintFade(); // prekresli
        }

        /// <summary>
        /// Nacte informaci o barve do slideru a barevneho obdelniku
        /// </summary>
        /// <param name="color"></param>
        private void LoadColorInformation(FadeColor color)
        {
            colorRect.Fill = new SolidColorBrush(Color.FromRgb(color.r, color.g, color.b));
            redSlider.Value = color.r;
            redTbx.Text = color.r.ToString();
            greenSlider.Value = color.g;
            greenTbx.Text = color.g.ToString();
            blueSlider.Value = color.b;
            blueTbx.Text = color.b.ToString();
            alphaSlider.Value = color.a;
            alphaTbx.Text = color.a.ToString();
        }

        /// <summary>
        /// Oznaci znacku barevnym prechodem
        /// </summary>
        /// <param name="el"></param>
        private void SelectEllipse(Ellipse el)
        {
            el.Fill = selectedBrush;
            selectedEllipse = el;
            setGrid.IsEnabled = true;
        }

        /// <summary>
        /// Odznaci znacku barevnym prechodem
        /// </summary>
        private void UnSelectEllipse()
        {
            if (selectedEllipse != null) selectedEllipse.Fill = unselectedBrush;
            selectedEllipse = null;
            setGrid.IsEnabled = false;
        }

        /// <summary>
        /// Prekresli barevny prechod
        /// </summary>
        private void RepaintFade()
        {
            TableDrawingManager.color_changed = true;
            LinearGradientBrush colorBrush = new LinearGradientBrush();
            LinearGradientBrush alphaBrush = new LinearGradientBrush();
            colorBrush.StartPoint = new Point(0, 0);
            colorBrush.EndPoint = new Point(1, 0);
            alphaBrush.StartPoint = new Point(0, 0);
            alphaBrush.EndPoint = new Point(1, 0);

            foreach (Ellipse el in fadeMarks)
            {
                FadeColor color = (FadeColor)el.DataContext;
                colorBrush.GradientStops.Add(new GradientStop(Color.FromRgb(color.r, color.g, color.b), color.position));
                alphaBrush.GradientStops.Add(new GradientStop(Color.FromRgb(color.a, color.a, color.a), color.position));
            }
            fadeRect.Fill = colorBrush;
            alphaRect.Fill = alphaBrush;
        }
        #endregion

        #region textbox handlers
        //============== HANDLERY PRO TEXT BOXY ==============================================
        private void greenTbx_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (selectedEllipse != null)
            {
                greenTbx.IsHitTestVisible = false;
                greenTbx.Text = ((FadeColor)selectedEllipse.DataContext).g.ToString();
                greenTbx.IsHitTestVisible = true;
            }
        }

        private void blueTbx_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (selectedEllipse != null)
            {
                blueTbx.IsHitTestVisible = false;
                blueTbx.Text = ((FadeColor)selectedEllipse.DataContext).b.ToString();
                blueTbx.IsHitTestVisible = true;
            }
        }

        private void redTbx_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (selectedEllipse != null)
            {
                redTbx.IsHitTestVisible = false;
                redTbx.Text = ((FadeColor)selectedEllipse.DataContext).r.ToString();
                redTbx.IsHitTestVisible = true;
            }
        }

        private void alphaTbx_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (selectedEllipse != null)
            {
                alphaTbx.IsHitTestVisible = false;
                alphaTbx.Text = ((FadeColor)selectedEllipse.DataContext).a.ToString();
                alphaTbx.IsHitTestVisible = true;
            }
        }
        private void greenTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (greenTbx.IsKeyboardFocused)
            {
                try
                {
                    byte value = Byte.Parse(((TextBox)sender).Text);
                    greenSlider.Value = value;
                }
                catch { }
            }
        }

        private void blueTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (blueTbx.IsKeyboardFocused)
            {
                try
                {
                    byte value = Byte.Parse(((TextBox)sender).Text);
                    blueSlider.Value = value;
                }
                catch { }
            }
        }

        private void redTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (redTbx.IsKeyboardFocused)
            {
                try
                {
                    byte value = Byte.Parse(((TextBox)sender).Text);
                    redSlider.Value = value;
                }
                catch { }
            }
        }

        private void alphaTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (alphaTbx.IsKeyboardFocused)
            {
                try
                {
                    byte value = Byte.Parse(((TextBox)sender).Text);
                    alphaSlider.Value = value;
                }
                catch { }
            }
        }
        #endregion

        #region slider handlers

        // ========================= HANDLERY PRO SLIDERY ========================================

        private void blueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blueTb.Text = blueSlider.Value.ToString("#");
            if (selectedEllipse != null)
            {
                ((FadeColor)selectedEllipse.DataContext).b = (byte)blueSlider.Value;
                blueTbx.Text = blueSlider.Value.ToString("#");
                colorRect.Fill = new SolidColorBrush(Color.FromRgb(((FadeColor)selectedEllipse.DataContext).r,
                     ((FadeColor)selectedEllipse.DataContext).g, ((FadeColor)selectedEllipse.DataContext).b)); 
                RepaintFade();
            }
        }

        private void greenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectedEllipse != null)
            {
                greenTb.Text = greenSlider.Value.ToString("#");
                greenTbx.Text = greenSlider.Value.ToString("#");
                ((FadeColor)selectedEllipse.DataContext).g = (byte)greenSlider.Value;
                colorRect.Fill = new SolidColorBrush(Color.FromRgb(((FadeColor)selectedEllipse.DataContext).r,
                     ((FadeColor)selectedEllipse.DataContext).g, ((FadeColor)selectedEllipse.DataContext).b)); 
                RepaintFade();
            }
        }

        private void redSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectedEllipse != null)
            {
                redTb.Text = redSlider.Value.ToString("#");
                redTbx.Text = redSlider.Value.ToString("#");
                ((FadeColor)selectedEllipse.DataContext).r = (byte)redSlider.Value;
                colorRect.Fill = new SolidColorBrush(Color.FromRgb(((FadeColor)selectedEllipse.DataContext).r,
                     ((FadeColor)selectedEllipse.DataContext).g, ((FadeColor)selectedEllipse.DataContext).b)); 
                RepaintFade();
            }
        }

        private void alphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectedEllipse != null)
            {
                alphaTb.Text = alphaSlider.Value.ToString("#");
                alphaTbx.Text = alphaSlider.Value.ToString("#");
                ((FadeColor)selectedEllipse.DataContext).a = (byte)alphaSlider.Value;
                colorRect.Fill = new SolidColorBrush(Color.FromRgb(((FadeColor)selectedEllipse.DataContext).r,
                     ((FadeColor)selectedEllipse.DataContext).g, ((FadeColor)selectedEllipse.DataContext).b)); 
                RepaintFade();
            }
        }

        #endregion

        #region mouse handlers

        //==================== HANDLERY PRO MYS ================================

        /// <summary>
        /// Mouse up nad znackou zpusobi nacteni informaci o teto barve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mark_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((Ellipse)sender).ReleaseMouseCapture();
            UnSelectEllipse();
            SelectEllipse((Ellipse)sender);
           LoadColorInformation((FadeColor)selectedEllipse.DataContext);
        }

        /// <summary>
        /// Znacka, se kterou je manipulovano
        /// </summary>
        private Ellipse draggedEllipse = null;

        /// <summary>
        /// Stisknuti tlacitka mysi nad znackou
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mark_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Ellipse)sender).CaptureMouse();
            draggedEllipse = (Ellipse)sender;
        }

        /// <summary>
        /// Pohyb mysi nad znackou
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mark_MouseMove(object sender, MouseEventArgs e)
        {
            // muze se hybat jen se znackami uprostred
            if (draggedEllipse != null && e.LeftButton == MouseButtonState.Pressed && !(((FadeColor)draggedEllipse.DataContext).position <= 0 || ((FadeColor)draggedEllipse.DataContext).position >= 1))
            {
                double posX = e.GetPosition(paintGrid).X;
                if (posX >= draggedEllipse.Width && posX <= (fadeRect.Width - draggedEllipse.Width))
                {
                    draggedEllipse.Margin = new Thickness(posX, 0, 0, 0);
                    ((FadeColor)draggedEllipse.DataContext).position = posX / fadeRect.Width;
                    RepaintFade();
                }
            }
        }

        
        private void ParticleColorSetWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                UnSelectEllipse();
                draggedEllipse = null;
            }
        }

        #endregion

        #region button handlers

        //===================HANDLERY PRO TLACITKA=========================

        // prida novou znacku
        private void addBut_Click(object sender, RoutedEventArgs e)
        {
            FadeColor defaultColor = new FadeColor(0, 0, 0, 255, 0.5);
            Ellipse mark = new Ellipse();
            mark.Width = 8;
            mark.Height = 8;
            mark.Margin = new Thickness(defaultColor.position * fadeRect.Width, 0, 0, 0);
            mark.Margin = new Thickness(defaultColor.position * fadeRect.Width, 0, 0, 0);

            mark.HorizontalAlignment = HorizontalAlignment.Left;
            mark.VerticalAlignment = VerticalAlignment.Bottom;
            mark.DataContext = defaultColor;
            mark.Fill = unselectedBrush;
            mark.MouseDown += new MouseButtonEventHandler(mark_MouseDown);
            mark.MouseUp += new MouseButtonEventHandler(mark_MouseUp);
            mark.MouseMove += new MouseEventHandler(mark_MouseMove);
            Grid.SetRow(mark, 1);
            paintGrid.Children.Add(mark);
            fadeMarks.Add(mark);
            UnSelectEllipse();
            SelectEllipse(mark);
            RepaintFade();
        }

        // odstrani znacku
        private void removeBut_Click(object sender, RoutedEventArgs e)
        {
            // musi zde byt nejmene dve znacky
            if (fadeMarks.Count > 2 && selectedEllipse != null)
            {
                fadeMarks.Remove(selectedEllipse);
                paintGrid.Children.Remove(selectedEllipse);
                UnSelectEllipse();
                RepaintFade();
            }
        }

        // ulozi informace
        private void okBut_Click(object sender, RoutedEventArgs e)
        {
            // musime mit alespon dve znacky
            if (fadeMarks != null && fadeMarks.Count > 2)
            {
                // nejprve se to musi setridit
                LinkedList<FadeColor> output = new LinkedList<FadeColor>();
                HashSet<FadeColor> temp = new HashSet<FadeColor>();

                foreach (Ellipse el in fadeMarks) temp.Add((FadeColor)el.DataContext);
                foreach (FadeColor fade in temp.OrderBy(fadec => fadec.position)) output.AddLast(fade);
                CommonAttribService.DEFAULT_FADE_COLORS = output;

                // ulozeni do souboru
                // ulozime neserazeny hashset, protoze linkedlist se serializovat neda!!!
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HashSet<FadeColor>), new Type[] { typeof(FadeColor) });
                    StreamWriter writer = new StreamWriter(CaptureSettings.Instance().DEFAULT_FADECOLOR_PATH);
                    serializer.Serialize(writer, temp);
                    writer.Close();
                }
                catch (Exception es)
                {
                    System.Windows.MessageBox.Show("Nastal problém při ukládání barev!");
                }
            }

            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift)) this.Close();
        }

        // kliknuti na swap prohodi vsechny barvy
        private void swapBut_Click(object sender, RoutedEventArgs e)
        {
            foreach (FadeColor clr in colors)
            {
                clr.position = 1.0 - clr.position;
            }
            LoadColors(colors);
        }
        #endregion


    }

    /// <summary>
    /// Trida reprezentujici barvu prechodu
    /// </summary>
    [Serializable]
    public class FadeColor
    {
       public byte r, g, b, a;
       public double position;

       public FadeColor()
       {
       }

       public FadeColor(byte r, byte g, byte b, byte a, double position)
       {
           this.r = r;
           this.g = g;
           this.b = b;
           this.a = a;
           this.position = position;
       }
    }
}
