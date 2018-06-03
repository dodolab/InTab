using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Accessories;
using System.Xml.Serialization;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Abstraktni trida pro tableObject, zde jsou veskere vlastnosti spolecne vsem objektum na stole
    /// </summary>
    [Serializable]
    public abstract class A_TableObject
    {
        /// <summary>
        /// Auto-inkrementor identifikatoru
        /// </summary>
        protected static int id_counter = 0;


        protected FPoint position; // aktualni pozice
        protected int id; // identifikator
        [XmlIgnore]
        protected A_TableObjectSettings settings; // konkretni nastaveni
        [XmlIgnore]
        protected A_TableObjectSettings baseSettings; // obecne nastaveni
        protected bool settings_allowed = false; // DULEZITE -> pokud true, bude se brat nastaveni objektu; pokud false, bude se brat obecne nastaveni
        protected A_Shape shape; // tvar objektu



        /// <summary>
        /// Vytvori novy tableObject, priradi mu identifikator a defaultni pozici
        /// </summary>
        public A_TableObject()
        {
            id = id_counter++;
            this.position = PhysicSettings.Instance().DEFAULT_TABLEOBJECT_POINT;
        }

        /// <summary>
        /// Vrati identifikator objektu
        /// </summary>
        public int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Vrati nebo nastavi hodnotu stanovujici, zda bude mit objekt sve nebo globalni nastaveni
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
        /// Vrati nebo nastavi tvar objektu
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

        /// <summary>
        /// Vrati nebo nastavi pozici objektu
        /// </summary>
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
