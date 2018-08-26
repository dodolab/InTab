using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Core.Data.TableObjects.Shapes
{
    /// <summary>
    /// Rectangle shape
    /// </summary>
     [Serializable]
    public class FRectangle : A_Shape
    {

        private double width;
        private double height;

        public double Width
        {
            get { return width; }
            set { this.width = value; }
        }

        public double Height
        {
            get { return height; }
            set { this.height = value; }
        }
    }
}
