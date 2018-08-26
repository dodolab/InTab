using System;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Black hole
    /// </summary>
     [Serializable]
    public class BlackHole : A_Rock
    {
        // number of absorbed objects
        protected int absorbNumber = 0;
        // advanced settings
        protected new BlackHoleSettings settings;
        // base settings
        protected new BlackHoleSettings baseSettings;

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
         /// Gets or sets number of absorbed particles
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
                else throw new Exception("Bad argument");
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
                else throw new Exception("Bad argument");
            }
        }
    }
}
