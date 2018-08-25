using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.GUI.Other;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Trida uchovavajici promenne, ktere vyuzivaji alespon tri komponenty
    /// </summary>
    public class CommonAttribService
    {

        //=======================================
        public static int ACTUAL_TABLE_WIDTH = DEFAULT_TABLE_WIDTH;

        public static int ACTUAL_TABLE_HEIGHT = DEFAULT_TABLE_HEIGHT;

        // aktualni multiplikator velikosti stolu
        public static double ACTUAL_TABLE_SIZE_MULTIPLIER = 1;

        public static Boolean MODE_2D = true; // pokud true, bude vystup v simulacnim okne
        // odkaz na randomizer
        public static Random apiRandom = new Random();

        // defaultni sirka stolu
        public const int DEFAULT_TABLE_WIDTH = 500;
        // defaultni vyska stolu
        public const int DEFAULT_TABLE_HEIGHT = 300;

        // maximalni zkresleni perspektivy pri kalibraci
        public const int DEFAULT_CALIBRATION_MINIMUM = 20;

        // nastaveni kamery (default)
        public const int DEFAULT_CAM_WIDTH = 640;
        public const int DEFAULT_CAM_HEIGHT = 480;

        // tolerance, s jakou musi mit nalezene kameny diferenci
        public const double DEFAULT_TOLERATION_ROCK_LENGTH = 3;
        // maximalni pocet pamatovatelnych iteraci v rozpoznavani
        public const int DEFAULT_ITERATION_MEMORY = 5;

        // odkaz na hlavni okno
        public static MainWindow mainWindow;

        public static Templates DEFAULT_TEMPLATES;

        // NASTAVENI PRO BAREVNE PRECHODY
        public static LinkedList<FadeColor> DEFAULT_FADE_COLORS;

        // VYSTUP DO PROMITACKY
        public static Boolean OUTPUT_DRAW_ALLOWED = false;
        // VYSTUP DO SIMULACNIHO OKNA
        public static Boolean SIMULATION_DRAW_ALLOWED = true;
       

        // posledni vyrenderovany obrazek - pouziva jej pouze OUTPUT WINDOW!!
        public static BitmapSource LAST_BITMAP = null;

        // velikost vystupniho obrazu
        public static int ACTUAL_OUTPUT_WIDTH = DEFAULT_TABLE_WIDTH;
    }
}
