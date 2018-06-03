using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Core.Data.TableObjects.Shapes
{
    /// <summary>
    /// Tvar typu kruh
    /// </summary>
     [Serializable]
    public class FCircle : A_Shape
    {
         //polomer
        private double radius;

         /// <summary>
         /// Vrati nebo nastavi polomer kruhu
         /// </summary>
        public double Radius
        {
            get { return radius; }
            set { this.radius = value; }
        }
    }
}
