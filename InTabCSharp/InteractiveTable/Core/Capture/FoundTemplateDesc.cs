using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Deskriptor pro nalezenou sablonu
    /// </summary>
    public class FoundTemplateDesc
    {
        /// <summary>
        /// Polomer nalezene kontury
        /// </summary>
        public double rate;
        /// <summary>
        /// Originalni sablona
        /// </summary>
        public Template template;
        /// <summary>
        /// Vzorek sablony
        /// </summary>
        public Template sample;
        /// <summary>
        /// Uhel mezi originalem a nalezenou sablonou
        /// </summary>
        public double angle;

        /// <summary>
        /// Vrati, kolikrat je nalezeny prvek vetsi nez original
        /// </summary>
        public double scale
        {
            get
            {
                return Math.Sqrt(sample.sourceArea / template.sourceArea);
            }
        }
    }
}
