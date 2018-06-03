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
    /// Nastaveni pro graficky vystup
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

        // pokud true, bude zobrazovat mrizku
        public bool DEFAULT_GRID_ENABLED = false;

        // pokud true, budou se ve vystupnim okne  zobrazovat kameny
        public bool DEFAULT_OUTPUT_ROCK_DISPLAY = false;


        // defaultni mod zmeny barvy castic
        public ParticleColorMode DEFAULT_PARTICLE_COLOR_MODE = ParticleColorMode.VELOCITY;

        // pokud true, budou se menit barvy casticim
        public bool DEFAULT_PARTICLE_COLOR_ALLOWED = true;
        
        // pokud true, bude velikost vystupu primo zavisla na velikosti stolu
        public bool OUTPUT_TABLE_SIZE_DEPENDENT = true;

        // defaultni barva castic (RGB)
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
                    MessageBox.Show("Vznikla chyba při načítání grafického nastavení");
                }
            }
            else // soubor neexistuje
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
                MessageBox.Show("Vznikla chyba při ukládání grafického nastavení");
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
