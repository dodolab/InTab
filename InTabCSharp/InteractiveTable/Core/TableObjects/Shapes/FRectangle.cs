using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Core.Data.TableObjects.Shapes
{
    /// <summary>
    /// Tvar typu obdelnik
    /// </summary>
     [Serializable]
    public class FRectangle : A_Shape
    {
         //sirka
        private double width;
         //vyska
        private double height;

         /// <summary>
         /// Vrati nebo nastavi sirku
         /// </summary>
        public double Width
        {
            get { return width; }
            set { this.width = value; }
        }

         /// <summary>
         /// Vrati nebo nastavi vysku
         /// </summary>
        public double Height
        {
            get { return height; }
            set { this.height = value; }
        }
    }
}
