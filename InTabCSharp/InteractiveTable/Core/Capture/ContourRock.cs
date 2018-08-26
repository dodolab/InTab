using System;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;

namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Entity that contains a stone and a contour it belongs to
    /// </summary>
    [Serializable]
    public class ContourRock
    {
        public const double INCREASE_HOP = 1.35;
        
        public A_Rock rock;

        public String contour_name;
        /// <summary>
        /// If true, the stone has disapperad from the table -> this will trigger an animation that will slowly
        /// decrease it's presence to zero
        /// </summary>
        public Boolean isMissing = false;

        /// <summary>
        /// This is used for merging the system -> if the stone disappears, it might be just because
        /// of the fact that the camera can't detect the stone properly (too much lighting). Thus,
        /// if we detect a stone of the same type at the same spot, we will try to restore it
        /// </summary>
       [NonSerialized()]
        public FoundRock foundRock;

        /// <summary>
        /// Min length among all found stones during one iteration
        /// </summary>
        public int minLength;

        /// <summary>
        /// Improves an intensity of a presence of the stone
        /// It is used when the stone goes missing -> we will decrease its presence down to zero
        /// </summary>
        public void ImproveIntensity()
        {
            if (!isMissing)
            {
                if ((rock.Intensity * INCREASE_HOP) > 100) rock.Intensity = 100;
                else rock.Intensity *= INCREASE_HOP;
            }
            else
            {
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
