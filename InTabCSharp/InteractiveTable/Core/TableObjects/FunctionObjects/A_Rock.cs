using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using System.Xml.Serialization;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Abstraktni trida pro kamen
    /// </summary>
    [Serializable]
    public abstract class A_Rock : A_TableObject
    {
        protected new A_RockSettings settings;
        protected new A_RockSettings baseSettings;

        protected double angle;
        protected double scale;
        // intenzita musi byt inicializovana na 100!!!
        protected double intensity = 100;
        // nazev kamene (stejny jako nazev kontury, pokud je spojen s konturou)
        protected String name;

        public new SettingsObjects.A_RockSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (value is A_RockSettings) this.settings = (A_RockSettings)value;
                else throw new Exception("Spatny argument");
            }
        }

        public new SettingsObjects.A_RockSettings BaseSettings
        {
            get { return baseSettings; }
            set
            {
                if (value is A_RockSettings) this.baseSettings = (A_RockSettings)value;
                else throw new Exception("Spatny argument");
            }
        }

        public override Shapes.A_Shape Shape
        {
            get
            {
                return (FCircle)this.shape;
            }
            set
            {
                if (value is FCircle) this.shape = value;
                else throw new Exception("Spatny argument");
            }
        }

        public double Radius
        {
            get
            { 
                return ((FCircle)shape).Radius;
            }
            set
            {
                ((FCircle)shape).Radius = value;
            }
        }

        public double Angle
        {
            get { return this.angle; }
            set { this.angle = value; }
        }

        public double Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        public double Intensity
        {
            get { return intensity; }
            set { this.intensity = value; }
        }

        public String Name
        {
            get { return name; }
            set { this.name = value; }
        }
    }
}
