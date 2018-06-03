using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Accessories;

namespace InteractiveTable.Core.Data.Capture
{

    /// <summary>
    /// Trida uchovavajici nalezeny kamen (jeho typ a pozici)
    /// </summary>
    public class FoundRock
    {
        /// <summary>
        /// Typ nalezeneho kamene
        /// </summary>
        public FoundRockType type;
        /// <summary>
        /// Pozice nalezeneho kamene
        /// </summary>
        public FPoint position;
        /// <summary>
        /// Polomer nalezene kontury
        /// </summary>
        public double radius;
        /// <summary>
        /// Uhel mezi originalem a nalezenou sablonou
        /// </summary>
        public double angle;
        /// <summary>
        /// Kolikrat je nalezeny prvek vetsi nez original
        /// </summary>
        public double scale;

        /// <summary>
        /// Nazev kontury
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
