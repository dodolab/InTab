using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Core.Data.TableObjects.Shapes
{
    /// <summary>
    /// Circle shape
    /// </summary>
     [Serializable]
    public class FCircle : A_Shape
    {
        private double radius;

        public double Radius
        {
            get { return radius; }
            set { this.radius = value; }
        }
    }
}
