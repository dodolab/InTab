using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using InteractiveTable.GUI.Other;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Accessories;
using System.IO;

namespace InteractiveTable.Managers
{

    /// <summary>
    /// Trida starajici se o vykreslovani celeho systemu
    /// </summary>
    public class TableDrawingManager
    {
        public FVector actual_size; // aktualni velikost obrazovky
        private UInt32[] pixelData; // pixelovy buffer
        private double ratioX; // pomer velikosti stolu : velikost obrazku
        private double ratioY; // pomer velikosti stolu : velikost obrazku
        private uint color_graviton = 0xFF00FF00; // defaultni barva gravitonoveho bodu (pokud ma byt vykreslen)
        private uint color_generator = 0xFF0000FF; // defaultni barva generatoroveho bodu
        private uint color_magneton = 0xFFFFFF00; // def. barva mag. bodu
        private uint color_blackHole = 0xFFFF0000; // def. barva blackhole bodu
        private uint color_particle = 0xFFDDFFFF; // meni se pri kazdem behu (reinicializovano)
        private Dictionary<double, uint> colorParamDict = new Dictionary<double, uint>(); // vzornik barev 
        public static bool color_changed = false; // okno s nastavenim barev pouziva tuto promennou pro reinicializaci vzorniku
        private int renderCounter; // pocitadlo renderu, pouziva se pro Garbage collector


        public TableDrawingManager()
        {
            pixelData = new UInt32[(int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * CommonAttribService.ACTUAL_TABLE_WIDTH)];
            actual_size = new FVector(CommonAttribService.ACTUAL_TABLE_WIDTH, CommonAttribService.ACTUAL_TABLE_HEIGHT);
        }

        /// <summary>
        /// Vykresli barvu na danou pozici v poli 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        private void FillArray(double x, double y, uint color)
        {
            try
            {
                    y = (int)actual_size.Y - y;
                if (x >= 0 && x < (int)actual_size.X &&
                    y >= 0 && y < ((int)actual_size.Y - 1)) pixelData[(int)(((int)x) + ((int)actual_size.X) * ((int)y))] = color;
            }
            catch { }
        }

        /// <summary>
        /// Vrati barvu na zaklade rychlosti castice
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private uint ColorVelocity(double param)
        {
            // zaokrouhlime parametr na 3 desetinna mista, muzeme tedy dostat
            // maximalne 999 barev a to nam staci!!
            param = Math.Round(param,3);
            if (param <= 0) param = 0.001; // osetreni mezi
            if (param >= 1) param = 0.999;

            if (color_changed)
            {
                color_changed = false;
                colorParamDict.Clear();
            }
            // memoizace:: pamatujeme si drive vypoctene hodnoty ze slovniku
            if (colorParamDict.ContainsKey(param))
            {
                return colorParamDict[param];
            }
            else
            {
                // vypocteme jednotlive barevne slozky

                LinkedListNode<FadeColor> first = CommonAttribService.DEFAULT_FADE_COLORS.First;
                while (first.Value.position < param) first = first.Next;
                if (first.Previous == null) colorParamDict[param] = 0x00000000 + (uint)((first.Value.r << 8) + (first.Value.g << 4) + first.Value.b << 2 + first.Value.a);
                else
                {
                    double diff = (param - first.Previous.Value.position) / (first.Value.position - first.Previous.Value.position);
                    double maxR = ((first.Value.r - first.Previous.Value.r));
                    double maxG = ((first.Value.g - first.Previous.Value.g));
                    double maxB = ((first.Value.b - first.Previous.Value.b));
                    double maxA = ((first.Value.a - first.Previous.Value.a));

                    uint r = (uint)(first.Previous.Value.r + maxR * diff);
                    uint g = (uint)(first.Previous.Value.g + maxG * diff);
                    uint b = (uint)(first.Previous.Value.b + maxB * diff);
                    uint a = (uint)(first.Previous.Value.a + maxA * diff);
                    colorParamDict[param] = 0x00000000 + (uint)(a << 24 | r << 16 | g << 8 | b);
 
                }

                return colorParamDict[param];
            }
        }

        /// <summary>
        /// Zmeni rozliseni barevneho pole (a stolu)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Resize(int x, int y)
        {
            pixelData = new UInt32[x*y];
            actual_size.X = x;
            actual_size.Y = y;
        }

        /// <summary>
        /// Vykresli castici, pouzivano hlavni metodou pro vykresleni
        /// </summary>
        /// <param name="part"></param>
        private void DrawParticle(Particle part)
        {
            FPoint position = new FPoint(part.Position.X, part.Position.Y);
            position.X += CommonAttribService.ACTUAL_TABLE_WIDTH / 2;
            position.Y += CommonAttribService.ACTUAL_TABLE_HEIGHT / 2;
            position.X *= ratioX;
            position.Y *= ratioY;


            if (CommonAttribService.DEFAULT_FADE_COLORS == null || CommonAttribService.DEFAULT_FADE_COLORS.Count == 0 || !GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_ALLOWED)
            {
                DrawCircle((int)position.X, (int)position.Y, (int)part.Settings.size, color_particle);
            }
            else
            {
                // rozliseni jednotlivych nastaveni 
                if (GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE == ParticleColorMode.GRAVITY)
                {
                    DrawCircle((int)position.X, (int)position.Y, (int)part.Settings.size, ColorVelocity(part.Vector_Acceleration.Size() / 5.0));
                }
                else if (GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE == ParticleColorMode.SIZE)
                {
                    DrawCircle((int)position.X, (int)position.Y, (int)part.Settings.size, ColorVelocity(part.Settings.size / 30.0));
                }
                else if (GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE == ParticleColorMode.VELOCITY)
                {
                    DrawCircle((int)position.X, (int)position.Y, (int)part.Settings.size, ColorVelocity(part.Vector_Velocity.Size() / 25.0));
                }
                else if (GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_MODE == ParticleColorMode.WEIGH)
                {
                    DrawCircle((int)position.X, (int)position.Y, (int)part.Settings.size, ColorVelocity(part.Settings.weigh / 50.0));
                }
            }
        }

    

        /// <summary>
        /// Vytvori obrazek soustavy
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="isTableSized"></param>
        /// <returns></returns>
        public BitmapSource CreateBitmap(TableDepositor objects, Boolean isTableSized)
        {
            try
            {
                renderCounter++; // pouzito prouze pro GC collect

                // pravidelne mazani pameti
                if (renderCounter > 10)
                {
                    renderCounter = 0;
                    GC.Collect();
                }

                ratioX = actual_size.X/CommonAttribService.ACTUAL_TABLE_WIDTH;
                ratioY = actual_size.Y/CommonAttribService.ACTUAL_TABLE_HEIGHT;

                color_particle = 0x00000000 + (uint)(255 << 24 | GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_R << 16 | GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_G << 8 | GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_B);

                // pokud se zmeni velikost stolu ,musi se obrazek zmensit na nejmensi moznou velikost
                if (isTableSized)
                {
                         if (pixelData.Length != (int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * CommonAttribService.ACTUAL_TABLE_WIDTH))
                         {
                             pixelData = new UInt32[(int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * CommonAttribService.ACTUAL_TABLE_WIDTH)];
                             actual_size.X = CommonAttribService.ACTUAL_TABLE_WIDTH;
                             actual_size.Y = CommonAttribService.ACTUAL_TABLE_HEIGHT;
                         }
                }

                // vymazani bufferu
                for (int i = 0; i < pixelData.Length; i++) pixelData[i] = 0xFF000000;
      
                if(GraphicsSettings.Instance().DEFAULT_GRID_ENABLED) DrawLines(objects); // vykresli mrizku

                // pro vsechny castice zjisti, jestli uz je na plose a prip. uprav jeji pozici,
                // jinak vytvor novy obrazek
                foreach (Particle part in objects.particles)
                {
                    DrawParticle(part);
                }

                if (GraphicsSettings.Instance().DEFAULT_OUTPUT_ROCK_DISPLAY)
                {

                    DrawRocks(objects);
                }

                // vytvorime bitmapu z pole pixelu a ulozime ji
                BitmapSource bmp = BitmapSource.Create((int)actual_size.X, (int)actual_size.Y, 96, 96, PixelFormats.Bgra32, null, pixelData, ((((int)actual_size.X * 32 + 31) & ~31) / 8));
                
                //ulozime posledni vygenerovanou bitmapu -> pouziva se, aby se negenerovalo 2x pro simulacni a vystupni okno
                CommonAttribService.LAST_BITMAP = bmp;
                return bmp;
                
            }
            catch {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!! VYJIMKA PRI VYKRESLOVANI");
            }

            return null;
        }

        /// <summary>
        /// Vykresli kameny jako male ctverecky (pro debugovaci ucely)
        /// </summary>
        /// <param name="objects"></param>
        private void DrawRocks(TableDepositor objects)
        {
            // pro vsechny gravitony:::
            foreach (Graviton grv in objects.gravitons)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 5, color_graviton);
            }

            // pro vsechny magnetony:::
            foreach (Magneton grv in objects.magnetons)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 5, color_magneton);
            }

            // pro vsechny cerme diry:::
            foreach (BlackHole grv in objects.blackHoles)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 8, color_blackHole);
            }

            // pro vsechny generatory:::
            foreach (Generator grv in objects.generators)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 5, color_generator);
            }
        }

        /// <summary>
        /// Nakresli ctverec o velikosti size a barve color
        /// </summary>
        /// <param name="x">souradnice X</param>
        /// <param name="y">souradnice Y</param>
        /// <param name="size">velikost ctverce</param>
        /// <param name="color">barva ctverce</param>
        private void DrawSquare(int x, int y, int size, uint color)
        {
            size = (int)(size * ratioX);
            if (size < 1) size = 1;
            if (size > 40) size = 40;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    FillArray(x + i - size / 2, y + j - size / 2, color);
                }
            }
        }

        /// <summary>
        /// Nakresli kruh o velikosti size a barve color
        /// </summary>
        /// <param name="x">Souradnice X</param>
        /// <param name="y">Souradnice Y</param>
        /// <param name="size">velikost kruhu</param>
        /// <param name="color">barva kruhu</param>
        private void DrawCircle(int x, int y, int size, uint color)
        {
            size = (int)(size * ratioX);
            if (size < 1) size = 1;
            if (size == 1)
            {
                FillArray(x, y, color);
                return;
            }

            if (size == 3)
            {
                // pro velikost 3 nebudeme nic pocitat, dame to takhle rovnou
                // kvuli slozitosti
                FillArray(x, y, color);
                FillArray(x + 1, y - 1, color & (0x00FFFFFF | (color/4)));
                FillArray(x + 1, y, color & (0x00FFFFFF | (color / 2)));
                FillArray(x + 1, y + 1, color & (0x00FFFFFF | (color / 4)));
                FillArray(x, y + 1, color & (0x00FFFFFF | (color / 2)));
                FillArray(x, y - 1, color & (0x00FFFFFF | (color / 2)));
                FillArray(x - 1, y - 1, color & (0x00FFFFFF | (color / 4)));
                FillArray(x - 1, y, color & (0x00FFFFFF | (color / 2)));
                FillArray(x - 1, y + 1, color & (0x00FFFFFF | (color / 4)));
                return;
            }
            if (size > 40) size = 40;

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        double length = Math.Sqrt((i + 0.5 - size / 2.0) * (i + 0.5 - size / 2.0) + (j + 0.5 - size / 2.0) * (j + 0.5 - size / 2.0));
                        if (length <= size / 2.55)
                        {

                            FillArray(x + i - size / 2.0, y + j - size / 2.0, color);
                        }
                    }
                }
            
        }

        /// <summary>
        /// Nakresli obdelnik o velikosti width x height a barve color
        /// </summary>
        /// <param name="x">souradnice v ose X</param>
        /// <param name="y">souradnice v ose Y</param>
        /// <param name="width">sirka obdelnika</param>
        /// <param name="height">vyska obdelnika</param>
        /// <param name="color">barva obdelnika</param>
        private void DrawRectangle(int x, int y, int width, int height, uint color)
        {
            width = (int)(width * ratioX);
            height = (int)(height * ratioY);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    FillArray(x + i - width / 2, y + j - height / 2, color);
                }
            }
        }

        /// <summary>
        /// Vykresli mrizku s gravitacnim pusobeni, experimentalni metoda!!
        /// </summary>
        /// <param name="objects"></param>
        private void DrawLines(TableDepositor objects)
        {
            DateTime pk = DateTime.Now;
            int row_pixel = (((int)(CommonAttribService.ACTUAL_TABLE_HEIGHT*ratioY)) / 20);
            int col_pixel = row_pixel;

            for (int i = 0; i < ((int)(ratioY*CommonAttribService.ACTUAL_TABLE_HEIGHT)); i += row_pixel)
            {

                for (int j = 0; j < ((int)(ratioX*CommonAttribService.ACTUAL_TABLE_WIDTH)); j += col_pixel)
                {

                    int orig_col = j;
                    int orig_row = i;

                    // originalni pozice bodu
                    int orig_position_x = orig_col - ((int)(ratioX * CommonAttribService.ACTUAL_TABLE_WIDTH)) / 2;
                    int orig_position_y = orig_row - ((int)(ratioY*CommonAttribService.ACTUAL_TABLE_HEIGHT)) / 2;


                    double temp_position_x = orig_position_x;
                    double temp_position_y = orig_position_y;

                    double shift_x = 0;
                    double shift_y = 0;

                    // pro kazdy graviton::
                    foreach (Graviton gr in objects.gravitons)
                    {
                        // vypocti delku mezi castici a gravitonem
                        double length = Math.Sqrt((temp_position_x - gr.Position.X) * (temp_position_x - gr.Position.X) +
                                        (temp_position_y - gr.Position.Y) * (temp_position_y - gr.Position.Y));

                        double length_x = (orig_position_x - gr.Position.X);
                        double length_y = (orig_position_y - gr.Position.Y);

                        // z delky mezi castici a gravitonem spocti potencial
                        double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * gr.Settings.weigh) / ((length * length / 100 + 10));

                        // posun castici
                        shift_x += acc * (gr.Position.X - temp_position_x) / (0.1 * gr.Settings.weigh);
                        shift_y += acc * (gr.Position.Y - temp_position_y) / (0.1 * gr.Settings.weigh);

                        temp_position_x = orig_position_x + shift_x;
                        temp_position_y = orig_position_y + shift_y;

                    }

                    // pro kazdy magneton::
                    foreach (Magneton mg in objects.magnetons)
                    {
                        // vypocti delku mezi castici a gravitonem
                        double length = Math.Sqrt((temp_position_x - mg.Position.X) * (temp_position_x - mg.Position.X) +
                                        (temp_position_y - mg.Position.Y) * (temp_position_y - mg.Position.Y));

                        double length_x = (orig_position_x - mg.Position.X);
                        double length_y = (orig_position_y - mg.Position.Y);

                        // z delky mezi castici a gravitonem spocti potencial
                        double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * mg.Settings.force) / ((length * length / 100 + 10));

                        // posun castici
                        shift_x -= acc * (mg.Position.X - temp_position_x);
                        shift_y -= acc * (mg.Position.Y - temp_position_y);

                        temp_position_x = orig_position_x + shift_x;
                        temp_position_y = orig_position_y + shift_y;

                    }


                    // pro kazdou cernou diru::
                    foreach (BlackHole mg in objects.blackHoles)
                    {
                        // vypocti delku mezi castici a gravitonem
                        double length = Math.Sqrt((temp_position_x - mg.Position.X) * (temp_position_x - mg.Position.X) +
                                        (temp_position_y - mg.Position.Y) * (temp_position_y - mg.Position.Y));

                        double length_x = (orig_position_x - mg.Position.X);
                        double length_y = (orig_position_y - mg.Position.Y);

                        // z delky mezi castici a gravitonem spocti potencial
                        double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * 80) / ((length * length / 100 + 10));

                        // posun castici
                        shift_x += acc * (mg.Position.X - temp_position_x);
                        shift_y += acc * (mg.Position.Y - temp_position_y);

                        temp_position_x = orig_position_x + shift_x;
                        temp_position_y = orig_position_y + shift_y;

                    }


                    // posunuta pozice bodu
                    int other_position_x = (int)(orig_col + shift_x);
                    int other_position_y = (int)(orig_row + shift_y);

                    int position = (int)((int)other_position_x + ((int)(ratioX * CommonAttribService.ACTUAL_TABLE_WIDTH)) * (((int)(ratioY * CommonAttribService.ACTUAL_TABLE_HEIGHT)) - (int)other_position_y));
                    int posX = (int)other_position_x;
                    int posY = (int)other_position_y;
                    DrawRectangle((int)posX, (int)posY, 1, 3, 0xFF333333);
                    DrawRectangle((int)posX, (int)posY, 3, 1, 0xFF333333);
                }
            }
        }

 
    }
}
