using System;
using System.Collections.Generic;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using InteractiveTable.GUI.Other;
using InteractiveTable.Accessories;

namespace InteractiveTable.Managers
{

    /// <summary>
    /// Manager that renders the whole system
    /// </summary>
    public class TableDrawingManager
    {
        public FVector actual_size; // current window size
        private UInt32[] pixelData; // pixel buffer
        private double ratioX; // ration between the width of the table and the width of the picture
        private double ratioY; // ration between the height of the table and the height of the picture
        private uint color_graviton = 0xFF00FF00; // default color for gravitons
        private uint color_generator = 0xFF0000FF; // default color for generators
        private uint color_magneton = 0xFFFFFF00; // default color for magnetons
        private uint color_blackHole = 0xFFFF0000; // default color for black holes
        private uint color_particle = 0xFFDDFFFF; // default color for particles
        private Dictionary<double, uint> colorParamDict = new Dictionary<double, uint>(); // lookup table for colors
        public static bool color_changed = false; // flag for refreshing lookup table for colors
        private int renderCounter; // number of rendered images


        public TableDrawingManager()
        {
            pixelData = new UInt32[(int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * CommonAttribService.ACTUAL_TABLE_WIDTH)];
            actual_size = new FVector(CommonAttribService.ACTUAL_TABLE_WIDTH, CommonAttribService.ACTUAL_TABLE_HEIGHT);
        }

        /// <summary>
        /// Renders a color in a 2D pixel array
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
        /// Returns a color based on a velocity of a particle
        /// </summary>
        /// <returns></returns>
        private uint ColorVelocity(double param)
        {
            // round to 3 -> 999 colors should be enough
            param = Math.Round(param,3);
            if (param <= 0) param = 0.001; 
            if (param >= 1) param = 0.999;

            if (color_changed)
            {
                color_changed = false;
                colorParamDict.Clear();
            }
            
            // remember previously calculated values
            if (colorParamDict.ContainsKey(param))
            {
                return colorParamDict[param];
            }
            else
            {
                // calculate particular colors
                LinkedListNode<FadeColor> first = CommonAttribService.DEFAULT_FADE_COLORS.First;
                while (first.Value.position < param && first.Next != null) first = first.Next;
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
        /// Resizes the table
        /// </summary>
        public void Resize(int x, int y)
        {
            pixelData = new UInt32[x*y];
            actual_size.X = x;
            actual_size.Y = y;
        }

        /// <summary>
        /// Renders a particle
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
                // distinguish among various settings 
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
        /// Creates an image of a table system
        /// </summary>
        /// <returns></returns>
        public BitmapSource CreateBitmap(TableDepositor objects, Boolean isTableSized)
        {
            try
            {
                renderCounter++; // used for garbage collector

                // invoke GC
                if (renderCounter > 30)
                {
                    renderCounter = 0;
                    GC.Collect();
                }

                ratioX = actual_size.X/CommonAttribService.ACTUAL_TABLE_WIDTH;
                ratioY = actual_size.Y/CommonAttribService.ACTUAL_TABLE_HEIGHT;

                color_particle = 0x00000000 + (uint)(255 << 24 | GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_R << 16 
                    | GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_G << 8 | GraphicsSettings.Instance().DEFAULT_PARTICLE_COLOR_B);

                // check resizing
                if (isTableSized)
                {
                         if (pixelData.Length != (int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * CommonAttribService.ACTUAL_TABLE_WIDTH))
                         {
                             pixelData = new UInt32[(int)(CommonAttribService.ACTUAL_TABLE_HEIGHT * CommonAttribService.ACTUAL_TABLE_WIDTH)];
                             actual_size.X = CommonAttribService.ACTUAL_TABLE_WIDTH;
                             actual_size.Y = CommonAttribService.ACTUAL_TABLE_HEIGHT;
                         }
                }

                // remove pixel buffer
                for (int i = 0; i < pixelData.Length; i++) pixelData[i] = 0xFF000000;
      
                if(GraphicsSettings.Instance().DEFAULT_GRID_ENABLED) DrawLines(objects); // draw grid

                // draw particles
                foreach (Particle part in objects.particles)
                {
                    DrawParticle(part);
                }

                // draw stones
                if (GraphicsSettings.Instance().DEFAULT_OUTPUT_ROCK_DISPLAY)
                {

                    DrawRocks(objects);
                }

                // create a bitmap for pixel array
                BitmapSource bmp = BitmapSource.Create((int)actual_size.X, (int)actual_size.Y, 96, 96, PixelFormats.Bgra32, null, pixelData, ((((int)actual_size.X * 32 + 31) & ~31) / 8));
                CommonAttribService.LAST_BITMAP = bmp;
                return bmp;
                
            }
            catch {
                // no-op here
            }

            return null;
        }

        /// <summary>
        /// Renders stones as tiny squares (mainly for debugging and polishing)
        /// </summary>
        /// <param name="objects"></param>
        private void DrawRocks(TableDepositor objects)
        {
            foreach (Graviton grv in objects.gravitons)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 5, color_graviton);
            }

            foreach (Magneton grv in objects.magnetons)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 5, color_magneton);
            }

            foreach (BlackHole grv in objects.blackHoles)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 8, color_blackHole);
            }

            foreach (Generator grv in objects.generators)
            {
                FPoint pos = new FPoint(grv.Position.X + CommonAttribService.ACTUAL_TABLE_WIDTH / 2, grv.Position.Y + CommonAttribService.ACTUAL_TABLE_HEIGHT / 2);
                pos.X *= ratioX;
                pos.Y *= ratioY;
                DrawSquare((int)pos.X, (int)pos.Y, 5, color_generator);
            }
        }

        /// <summary>
        /// Draws a square of given size
        /// </summary>
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
        /// Draw a circle of given size and color
        /// </summary>
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
                // if the size is 3, skip the calculation and draw it directly in order to increase performance
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

            // max size is 40
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
        /// Draws a rectangle of given size and color
        /// </summary>
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
        /// Draws a grid that determines gravity force
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

                    // original position of the point
                    int orig_position_x = orig_col - ((int)(ratioX * CommonAttribService.ACTUAL_TABLE_WIDTH)) / 2;
                    int orig_position_y = orig_row - ((int)(ratioY*CommonAttribService.ACTUAL_TABLE_HEIGHT)) / 2;


                    double temp_position_x = orig_position_x;
                    double temp_position_y = orig_position_y;

                    double shift_x = 0;
                    double shift_y = 0;

                    foreach (Graviton gr in objects.gravitons)
                    {
                        // calc distance between the original point and the graviton
                        double length = Math.Sqrt((temp_position_x - gr.Position.X) * (temp_position_x - gr.Position.X) +
                                        (temp_position_y - gr.Position.Y) * (temp_position_y - gr.Position.Y));

                        
                        // calculate potential
                        double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * gr.Settings.weigh) / ((length * length / 100 + 10));

                        // shift the grid point
                        shift_x += acc * (gr.Position.X - temp_position_x) / (0.1 * gr.Settings.weigh);
                        shift_y += acc * (gr.Position.Y - temp_position_y) / (0.1 * gr.Settings.weigh);

                        temp_position_x = orig_position_x + shift_x;
                        temp_position_y = orig_position_y + shift_y;

                    }
                    
                    foreach (Magneton mg in objects.magnetons)
                    {
                        double length = Math.Sqrt((temp_position_x - mg.Position.X) * (temp_position_x - mg.Position.X) +
                                        (temp_position_y - mg.Position.Y) * (temp_position_y - mg.Position.Y));

                        double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * mg.Settings.force) / ((length * length / 100 + 10));

                        shift_x -= acc * (mg.Position.X - temp_position_x);
                        shift_y -= acc * (mg.Position.Y - temp_position_y);

                        temp_position_x = orig_position_x + shift_x;
                        temp_position_y = orig_position_y + shift_y;
                    }

                    
                    foreach (BlackHole mg in objects.blackHoles)
                    {
                        double length = Math.Sqrt((temp_position_x - mg.Position.X) * (temp_position_x - mg.Position.X) +
                                        (temp_position_y - mg.Position.Y) * (temp_position_y - mg.Position.Y));

                        double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * 80) / ((length * length / 100 + 10));

                        shift_x += acc * (mg.Position.X - temp_position_x);
                        shift_y += acc * (mg.Position.Y - temp_position_y);

                        temp_position_x = orig_position_x + shift_x;
                        temp_position_y = orig_position_y + shift_y;
                    }


                    // shift the position of the grid point
                    int other_position_x = (int)(orig_col + shift_x);
                    int other_position_y = (int)(orig_row + shift_y);

                    int posX = (int)other_position_x;
                    int posY = (int)other_position_y;
                    DrawRectangle((int)posX, (int)posY, 1, 3, 0xFF333333);
                    DrawRectangle((int)posX, (int)posY, 3, 1, 0xFF333333);
                }
            }
        }
    }
}
