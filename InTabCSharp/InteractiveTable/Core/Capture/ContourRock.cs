using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;

namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Objekt propojujici identifikaci kontury a typ kamene
    /// </summary>
    [Serializable]
    public class ContourRock
    {
        public const double INCREASE_HOP = 1.35;
        /// <summary>
        /// Odkaz na kamen i s nastavenim
        /// </summary>
        public A_Rock rock;
        /// <summary>
        /// Nazev kontury
        /// </summary>
        public String contour_name;
        /// <summary>
        /// Je true, pokud kamen chyby - spusti se animace ubirani potencialu
        /// </summary>
        public Boolean isMissing = false;
        /// <summary>
        /// Pouziva se pro mergovani soustav;
        /// foundrock je pripadny nalezeny kamen,
        /// ktery byl prohlasen za identicky
        /// s timto kamenem
        /// </summary>
       [NonSerialized()]
        public FoundRock foundRock;
        /// <summary>
        /// Minimalni vzdalenost mezi vsemi
        /// nalezenymi kameny v ramci jedne iterace
        /// </summary>
        public int minLength;



        public void ImproveIntensity()
        {
           // Console.WriteLine("Upravuji intenzitu " + rock.Intensity + " pro kamen na pozici " + rock.Position.X + ", " + rock.Position.Y);
            if (!isMissing)
            {
               // Console.WriteLine("Kamen nechybi");
                if ((rock.Intensity * INCREASE_HOP) > 100) rock.Intensity = 100;
                else rock.Intensity *= INCREASE_HOP;
            }
            else
            {
               // Console.WriteLine("Kamen chybi");
                rock.Intensity /= INCREASE_HOP;
                if (rock.Intensity  < 10) rock.Intensity = 0;
            }
 
        }

        public ContourRock()
        {
           
        }

        public ContourRock(A_Rock rock, String contour_name)
        {
            this.rock = rock;
            this.contour_name = contour_name;
        }
    }
}
