using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Zjednodusena trida pro popis nalezeneho kamene;
    /// obsahuje pouze ty nejnutnejsi informace tak,
    /// aby jej bylo mozno velmi snadno serializovat do proudu bytu
    /// </summary>
    public class RockDesc
    {
        public const byte ROCKDESC_TYPE_GRAVITON = 1;
        public const byte ROCKDESC_TYPE_MAGNETON = 2;
        public const byte ROCKDESC_TYPE_GENERATOR = 3;
        public const byte ROCKDESC_TYPE_BLACKHOLE = 4;

        /// <summary>
        /// pozice kamene v X-ove souradnici
        /// </summary>
        public short positionX;
        /// <summary>
        /// pozice kamene v Y-ove souradnici
        /// </summary>
        public short positionY;
        /// <summary>
        /// identifikator typu kamene
        /// </summary>
        public byte rockType;
        /// <summary>
        /// Polomer nalezene kontury
        /// </summary>
        public double rate;
        /// <summary>
        /// Uhel mezi originalem a nalezenou sablonou
        /// </summary>
        public double angle;
        /// <summary>
        /// Kolikrat je nalezeny prvek vetsi nez original
        /// </summary>
        public double scale;
        /// <summary>
        /// Intenzita, pouziva se pro ztraceni kamenu z mapy
        /// </summary>
        public byte intensity;

        public RockDesc()
        {

        }

        public RockDesc(FoundTemplateDesc template)
        {
            this.rate = template.rate;
            this.angle = template.angle;
            this.scale = template.scale;
        }
    }
}
