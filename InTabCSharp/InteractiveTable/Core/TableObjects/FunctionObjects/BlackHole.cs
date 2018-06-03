using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Objekt reprezentujici cernou diru
    /// </summary>
     [Serializable]
    public class BlackHole : A_Rock
    {
        // pocet pohlcenych objektu
        protected int absorbNumber = 0;
        // nastaveni
        protected new BlackHoleSettings settings;
        // zakladni nastaveni
        protected new BlackHoleSettings baseSettings;

         /// <summary>
         /// Vytvori novou cernou diru s defaultni vahou, kapacitou a polomerem
         /// </summary>
        public BlackHole()
        {
            this.shape = new FCircle();
            this.settings = new BlackHoleSettings();
            this.baseSettings = new BlackHoleSettings();
            settings.weigh = PhysicSettings.Instance().DEFAULT_BLACKHOLE_WEIGH;
            settings.capacity = PhysicSettings.Instance().DEFAULT_BLACKHOLE_CAPACITY;
            ((FCircle)shape).Radius = PhysicSettings.Instance().DEFAULT_BLACKHOLE_RADIUS;
        }

         /// <summary>
         /// Vrati pocet pohlcenych castic
         /// </summary>
        public int AbsorbNumber
        {
            get { return absorbNumber; }
            set { this.absorbNumber = value; }
        }

        public new BlackHoleSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (value is BlackHoleSettings) this.settings = (BlackHoleSettings)value;
                else throw new Exception("Spatny argument");
            }
        }

        public new BlackHoleSettings BaseSettings
        {
            get
            {
                return baseSettings;
            }
            set
            {
                if (value is BlackHoleSettings) this.baseSettings = (BlackHoleSettings)value;
                else throw new Exception("Spatny argument");
            }
        }
    }
}
