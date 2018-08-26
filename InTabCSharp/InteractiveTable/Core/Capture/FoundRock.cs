using System;
using InteractiveTable.Accessories;

namespace InteractiveTable.Core.Data.Capture
{

    /// <summary>
    /// Detected stone and its position
    /// </summary>
    public class FoundRock
    {
        public FoundRockType type;
        public FPoint position;
        
        /// <summary>
        /// Radius of detected contour
        /// </summary>
        public double radius;

        /// <summary>
        /// Angle between the detected contour and its template
        /// </summary>
        public double angle;

        /// <summary>
        /// Scale between the detected contour and its template
        /// </summary>
        public double scale;

        /// <summary>
        /// Name of the contour
        /// </summary>
        public String contour_name;



        /// <summary>
        /// Pouziva se pro mergovani soustav;
        /// placedRock je pripadny umisteny kamen,
        /// ktery byl prohlasen za identicky
        /// s timto nalezenym kamenem
        /// </summary>
        public ContourRock placedRock;
        /// <summary>
        /// Minimalni vzdalenost mezi vsemi
        /// nalezenymi kameny v ramci jedne iterace
        /// </summary>
        public int minLength;

        public FoundRock(String contourName, FoundRockType type, FPoint pos, double radius, double scale, double angle)
        {
            this.type = type;
            this.contour_name = contourName;
            this.position = pos;
            this.radius = radius;
            this.scale = scale;
            this.angle = angle;
        }
    }

    /// <summary>
    /// Typ nalezeneho kamene
    /// </summary>
    public enum FoundRockType
    {
        GRAVITON, MAGNETON, BLACKHOLE, GENERATOR
    }
}
