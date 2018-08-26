using System;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Accessories;
using System.Xml.Serialization;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Base class for all table objects
    /// </summary>
    [Serializable]
    public abstract class A_TableObject
    {
        /// <summary>
        /// Static counter for all identifiers
        /// </summary>
        protected static int id_counter = 0;


        protected FPoint position; // current position
        protected int id; // identifier
        [XmlIgnore]
        protected A_TableObjectSettings settings; // current settings
        [XmlIgnore]
        protected A_TableObjectSettings baseSettings; // general settings
        protected bool settings_allowed = false; // if true, we will take the settings into account. If false, we will take global settings instead
        protected A_Shape shape; // shape of the object

        
        public A_TableObject()
        {
            id = id_counter++;
            this.position = PhysicSettings.Instance().DEFAULT_TABLEOBJECT_POINT;
        }

        public int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets or sets an identifier whether the local settings of this object should be taken into accoutn
        /// </summary>
        public bool Settings_Allowed
        {
            get { return settings_allowed; }
            set { this.settings_allowed = value; }
        }

        [XmlIgnore]
        public virtual A_TableObjectSettings BaseSettings
        {
            get { return baseSettings; }
            set { this.baseSettings = value; }
        }

        [XmlIgnore]
        public virtual A_TableObjectSettings Settings
        {
            get { return settings; }
            set { this.settings = value; }
        }

        /// <summary>
        /// Gets or sets shape of the object
        /// </summary>
        public virtual A_Shape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                this.shape = value;
            }
        }

        public virtual FPoint Position
        {
            get
            {
                return position;
            }
            set
            {
                this.position = value;
            }
        }
    }
}
