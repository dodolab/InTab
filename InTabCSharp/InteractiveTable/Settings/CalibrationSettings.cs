using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Xml.Serialization;


namespace InteractiveTable.Settings
{
    public class CalibrationSettings
    {
        private static CalibrationSettings instance = new CalibrationSettings();

        public static CalibrationSettings Instance()
        {
            return instance;
        }

        // kalibracni body A a C
        public Point CALIBRATION_POINT_A;
        public Point CALIBRATION_POINT_C;

        // kalibracni body pro InTab2
        public Point CALIBRATION_LETTER_A;
        public Point CALIBRATION_LETTER_C;

        // perspektivni zkresleni
        public double CALIBRATION_PERSPECTIVE;

        //rotace dopisu pro intab2
        public int CALIBRATION_LETTER_ROTATION;

        // rotace obrazku
        public int CALIBRATION_ROTATION;


        public void Load()
        {
            if (File.Exists("settings_calib.xml"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CalibrationSettings), new Type[] { typeof(Point) });
                    StreamReader reader = new StreamReader("settings_calib.xml");
                    CalibrationSettings obj = (CalibrationSettings)serializer.Deserialize(reader);
                    CalibrationSettings.instance = obj;
                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Vznikla chyba při načítání kalibračního nastavení");
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
                XmlSerializer serializer = new XmlSerializer(typeof(CalibrationSettings), new Type[] { typeof(Point) });
                StreamWriter writer = new StreamWriter("settings_calib.xml");
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch
            {
                MessageBox.Show("Vznikla chyba při ukládání kalibračního nastavení");
            }
        }

        public void Restart()
        {
         CALIBRATION_POINT_A = new Point(0,0);
         CALIBRATION_POINT_C = new Point(200,200);
         CALIBRATION_LETTER_A = new Point(0, 0);
         CALIBRATION_LETTER_C = new Point(200, 200);
         CALIBRATION_PERSPECTIVE = 0;
         CALIBRATION_ROTATION = 0;
         CALIBRATION_LETTER_ROTATION = 0;
        }
    
    }
}
