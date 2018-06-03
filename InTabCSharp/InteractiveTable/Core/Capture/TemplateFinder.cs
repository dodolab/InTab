using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.Capture;
using InteractiveTable.Accessories;

namespace InteractiveTable.Core.Graphics
{
    /// <summary>
    /// Vyhledavani sablon pomoci danych parametru, porovnava je se sadou vzorku
    /// </summary>
    public class TemplateFinder
    {
        public double minACF = 0.96d; // minimalni auto-korelace
        public double minICF = 0.85d; // minimalni inter-korelace
        public bool checkICF = true;
        public bool checkACF = true;
        public double maxRotateAngle = Math.PI; // maximalni rotacni uhel
        public int maxACFDescriptorDeviation = 4;
        public string antiPatternName = "antipattern";

        /// <summary>
        /// Nalezne sablony mezi jiz znamymi sablonami
        /// </summary>
        /// <param name="templates">Zname sablony</param>
        /// <param name="sample">Nalezeny vzorek</param>
        /// <returns></returns>
        public FoundTemplateDesc FindTemplate(Templates templates, Template sample)
        {
            double rate = 0;
            double angle = 0; // uhel tolerance (pro 180° je jedno, jak bude kontura otocena)
            Complex interCorr = default(Complex);
            Template foundTemplate = null;
            foreach (var template in templates)
            { // pro vsechny sablony v templates::
                //vylouceni vsech vzoru, ktere jsou prilis male
                if (Math.Abs(sample.autoCorrDescriptor1 - template.autoCorrDescriptor1) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor2 - template.autoCorrDescriptor2) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor3 - template.autoCorrDescriptor3) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor4 - template.autoCorrDescriptor4) > maxACFDescriptorDeviation) continue;
                //
                double r = 0;
                if (checkACF) // vylouceni sablon podle podobnosti
                {
                    r = template.autoCorr.NormDot(sample.autoCorr).Norma;
                    if (r < minACF)
                        continue;
                }
                if (checkICF) // vylouceni sablon podle simetrie
                {
                    interCorr = template.contour.InterCorrelation(sample.contour).FindMaxNorma();
                    r = interCorr.Norma / (template.contourNorma * sample.contourNorma);
                    if (r < minICF)
                        continue;
                    if (Math.Abs(interCorr.Angle) > maxRotateAngle)
                        continue;
                }
                if (template.preferredAngleNoMore90 && Math.Abs(interCorr.Angle) >= Math.PI / 2)
                    continue;//nedosazitelny uhel
                //hledame max rate
                if (r >= rate)
                {
                    rate = r;
                    foundTemplate = template;
                    angle = interCorr.Angle;
                }
            }
            // ignorovat antipattern
            if (foundTemplate != null && foundTemplate.name == antiPatternName)
                foundTemplate = null;
            
            if (foundTemplate != null)
                return new FoundTemplateDesc() { template = foundTemplate, rate = rate, sample = sample, angle = angle };
            else
                return null;
        }
    }

}
