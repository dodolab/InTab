using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.GUI.Other;

namespace InteractiveTable.Managers
{
    /// <summary>
    /// Shared constants
    /// </summary>
    public class CommonAttribService
    {

        //=======================================
        public static int ACTUAL_TABLE_WIDTH = DEFAULT_TABLE_WIDTH;

        public static int ACTUAL_TABLE_HEIGHT = DEFAULT_TABLE_HEIGHT;

        // multiplier of table size
        public static double ACTUAL_TABLE_SIZE_MULTIPLIER = 1;

        public static Boolean MODE_2D = true; // if true, the output will go into the simulation window
        
        public static Random apiRandom = new Random();

        public const int DEFAULT_TABLE_WIDTH = 500;
        public const int DEFAULT_TABLE_HEIGHT = 300;

        // max perspective distortion for calibration
        public const int DEFAULT_CALIBRATION_MINIMUM = 20;

        // default camera settings
        public const int DEFAULT_CAM_WIDTH = 640;
        public const int DEFAULT_CAM_HEIGHT = 480;

        // difference toleration for stone detection
        public const double DEFAULT_TOLERATION_ROCK_LENGTH = 3;
        // max number of memorized iterations
        public const int DEFAULT_ITERATION_MEMORY = 5;
        
        public static MainWindow mainWindow;

        public static Templates DEFAULT_TEMPLATES;

        // color gradients
        public static LinkedList<FadeColor> DEFAULT_FADE_COLORS;

        // output to the OpenGL window
        public static Boolean OUTPUT_DRAW_ALLOWED = false;
        // output to the simulator window
        public static Boolean SIMULATION_DRAW_ALLOWED = true;
       
        // last rendered image
        public static BitmapSource LAST_BITMAP = null;

        // size of output image
        public static int ACTUAL_OUTPUT_WIDTH = DEFAULT_TABLE_WIDTH;
    }
}
