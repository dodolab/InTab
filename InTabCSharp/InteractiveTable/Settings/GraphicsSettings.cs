using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace InteractiveTable.Settings
{
    public enum ParticleColorMode { GRAVITY, VELOCITY, WEIGH, SIZE }

    /// <summary>
    /// Settings for graphical output
    /// </summary>
    public class GraphicsSettings
    {


        private GraphicsSettings()
        {
        }


        private static GraphicsSettings instance = new GraphicsSettings();

        public static GraphicsSettings Instance()
        {
            return instance;
        }

     
        public bool DEFAULT_GRID_ENABLED = false;

        // if true, stones will be rendered in the output window
        public bool DEFAULT_OUTPUT_ROCK_DISPLAY = false;

        public ParticleColorMode DEFAULT_PARTICLE_COLOR_MODE = ParticleColorMode.VELOCITY;

        public bool DEFAULT_PARTICLE_COLOR_ALLOWED = true;
        
        // if true, the size of the output window will depend on the size of the table
        public bool OUTPUT_TABLE_SIZE_DEPENDENT = true;

        // default particle color
        public  byte DEFAULT_PARTICLE_COLOR_R = 20;
        public  byte DEFAULT_PARTICLE_COLOR_G = 200;
        public  byte DEFAULT_PARTICLE_COLOR_B = 150;


        public void Load()
        {
            if (File.Exists("settings_graphics.xml"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GraphicsSettings), new Type[] { typeof(ParticleColorMode) });
                    StreamReader reader = new StreamReader("settings_graphics.xml");
                    GraphicsSettings obj = (GraphicsSettings)serializer.Deserialize(reader);
                    GraphicsSettings.instance = obj;
                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error ocurred during loading of graphical settings");
                }
            }
            else // file doesn't exist
            {
                Restart();
            }
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GraphicsSettings), new Type[] { typeof(ParticleColorMode) });
                StreamWriter writer = new StreamWriter("settings_graphics.xml");
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch
            {
                MessageBox.Show("An error ocurred during saving of graphical settings");
            }
        }

        public void Restart()
        {
           DEFAULT_GRID_ENABLED = false;
           DEFAULT_OUTPUT_ROCK_DISPLAY = false;
           DEFAULT_PARTICLE_COLOR_MODE = ParticleColorMode.VELOCITY;
           DEFAULT_PARTICLE_COLOR_ALLOWED = true;
           OUTPUT_TABLE_SIZE_DEPENDENT = true;
           DEFAULT_PARTICLE_COLOR_R = 20;
           DEFAULT_PARTICLE_COLOR_G = 200;
           DEFAULT_PARTICLE_COLOR_B = 150;
        }
    }
}
