using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows;

namespace InteractiveTable.Settings
{
    public class CaptureSettings
    {
        private static CaptureSettings instance = new CaptureSettings();

        public static CaptureSettings Instance()
        {
            return instance;
        }

        // defaultni kamera
        public int DEFAULT_CAMERA_INDEX;

        //====================================
        // NASTAVENI PRO ZPRACOVANI KONTUR
        //====================================

        public string DEFAULT_TEMPLATE_PATH = "";
        public string DEFAULT_FADECOLOR_PATH = "";

        public double DEFAULT_ROTATE_ANGLE;
        public int DEFAULT_BLOCK_LEVEL;
        public int DEFAULT_CONTOUR_MIN_AREA;
        public int DEFAULT_CONTOUR_MIN_LENGTH;
        public double DEFAULT_DILATATION;
        public double DEFAULT_ERODE;
        public double DEFAULT_GAMMA;
        public int DEFAULT_MAX_ACF;
        public double DEFAULT_MIN_ACF;
        public double DEFAULT_MIN_ICF;
        public bool DEFAULT_NOISE_ENABLED;
        public int DEFAULT_NOISE_FILTER;
        public bool DEFAULT_BLUR_ENABLED;
        public double DEFAULT_ADAPTIVE_THRESHOLD_PARAMETER;
        public int DEFAULT_ADAPTIVE_THRESHOLD_BLOCKSIZE;
        public bool DEFAULT_INVERT_ENABLED;
        public bool DEFAULT_NORMALIZE_ENABLED;
        public String SERVER_IP_ADDRESS;
        public bool SEND_IMAGES;
        public int SEND_INTERVAL;
        public bool MOTION_DETECTION;
        public int MOTION_TOLERANCE;


        public void Load()
        {
            if (File.Exists("settings_capture.xml"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CaptureSettings), new Type[] { typeof(String) });
                    StreamReader reader = new StreamReader("settings_capture.xml");
                    CaptureSettings obj = (CaptureSettings)serializer.Deserialize(reader);
                    CaptureSettings.instance = obj;
                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Vznikla chyba při načítání capture nastavení");
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
                XmlSerializer serializer = new XmlSerializer(typeof(CaptureSettings), new Type[] { typeof(String) });
                StreamWriter writer = new StreamWriter("settings_capture.xml");
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch
            {
                MessageBox.Show("Vznikla chyba při ukládání capture nastavení");
            }
        }

        public void Restart()
        {
          DEFAULT_CAMERA_INDEX = 0;
          DEFAULT_TEMPLATE_PATH = "";
          DEFAULT_FADECOLOR_PATH = "";

          DEFAULT_ROTATE_ANGLE = 180;
          DEFAULT_BLOCK_LEVEL = 0;
          DEFAULT_CONTOUR_MIN_AREA = 300;
          DEFAULT_CONTOUR_MIN_LENGTH = 100;
          DEFAULT_DILATATION = 0;
          DEFAULT_ERODE = 0;
          DEFAULT_GAMMA = 1.76;
          DEFAULT_MAX_ACF = 50;
          DEFAULT_MIN_ACF = 0.84;
          DEFAULT_MIN_ICF = 0.82;
          DEFAULT_NOISE_ENABLED = true;
          DEFAULT_NOISE_FILTER = 100;
          DEFAULT_BLUR_ENABLED = true;
          DEFAULT_ADAPTIVE_THRESHOLD_PARAMETER = 1.5;
          DEFAULT_ADAPTIVE_THRESHOLD_BLOCKSIZE = 80;
          DEFAULT_INVERT_ENABLED = false;
          DEFAULT_NORMALIZE_ENABLED = false;
          SERVER_IP_ADDRESS = "127.0.0.1";
          SEND_IMAGES = false;
          SEND_INTERVAL = 1000;
          MOTION_DETECTION = false;
          MOTION_TOLERANCE = 1000;
        }
    }
}
