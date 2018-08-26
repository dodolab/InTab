using System;

namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Descriptor for detected template
    /// </summary>
    public class FoundTemplateDesc
    {
        /// <summary>
        /// Radius of the template
        /// </summary>
        public double rate;

        /// <summary>
        /// Original template
        /// </summary>
        public Template template;

        /// <summary>
        /// Template sample
        /// </summary>
        public Template sample;
        /// <summary>
        /// Angle between the original and detected template
        /// </summary>
        public double angle;
        /// <summary>
        /// Scale between the sample and the original template
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
