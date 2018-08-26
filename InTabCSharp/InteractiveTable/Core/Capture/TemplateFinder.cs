using System;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.Accessories;

namespace InteractiveTable.Core.Graphics
{
    /// <summary>
    /// Class that looks for templates according to given parameters
    /// </summary>
    public class TemplateFinder
    {
        public double minACF = 0.96d; // min auto-correlation
        public double minICF = 0.85d; // min inter-correlation
        public bool checkICF = true;
        public bool checkACF = true;
        public double maxRotateAngle = Math.PI; 
        public int maxACFDescriptorDeviation = 4;
        public string antiPatternName = "antipattern";

        /// <summary>
        /// Tries to find templates among known templates
        /// </summary>
        /// <param name="templates">Known templates</param>
        /// <param name="sample">Detected sample</param>
        /// <returns></returns>
        public FoundTemplateDesc FindTemplate(Templates templates, Template sample)
        {
            double rate = 0;
            double angle = 0; // toleration angle (for 180° we can detect the contour independently on rotation)
            Complex interCorr = default(Complex);
            Template foundTemplate = null;
            foreach (var template in templates)
            { 
                // discard too small samples
                if (Math.Abs(sample.autoCorrDescriptor1 - template.autoCorrDescriptor1) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor2 - template.autoCorrDescriptor2) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor3 - template.autoCorrDescriptor3) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor4 - template.autoCorrDescriptor4) > maxACFDescriptorDeviation) continue;
                
                double r = 0;
                if (checkACF) // discard by symmetry 
                {
                    r = template.autoCorr.NormDot(sample.autoCorr).Norma;
                    if (r < minACF)
                        continue;
                }
                if (checkICF) // discard by similarity
                {
                    interCorr = template.contour.InterCorrelation(sample.contour).FindMaxNorma();
                    r = interCorr.Norma / (template.contourNorma * sample.contourNorma);
                    if (r < minICF)
                        continue;
                    if (Math.Abs(interCorr.Angle) > maxRotateAngle)
                        continue;
                }
                if (template.preferredAngleNoMore90 && Math.Abs(interCorr.Angle) >= Math.PI / 2)
                    continue;
                
                // try to find max rate
                if (r >= rate)
                {
                    rate = r;
                    foundTemplate = template;
                    angle = interCorr.Angle;
                }
            }
            // ignore antipatterns
            if (foundTemplate != null && foundTemplate.name == antiPatternName)
                foundTemplate = null;
            
            if (foundTemplate != null)
                return new FoundTemplateDesc() { template = foundTemplate, rate = rate, sample = sample, angle = angle };
            else
                return null;
        }
    }

}
