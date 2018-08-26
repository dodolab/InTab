using System;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects {
    
    /// <summary>
    /// Gravity stone
    /// </summary>
    [Serializable]
    public class Graviton : A_Rock {

        protected new GravitonSettings settings;
        protected new GravitonSettings baseSettings;

        public Graviton() {
            settings = new GravitonSettings();
            baseSettings = new GravitonSettings();
            this.shape = new FCircle();
            ((FCircle)shape).Radius = PhysicSettings.Instance().DEFAULT_GRAVITON_RADIUS;
            settings.weigh = PhysicSettings.Instance().DEFAULT_GRAVITON_WEIGH;
        }

        public new GravitonSettings Settings {
            get {
                return settings;
            }
            set {
                if (value is GravitonSettings) this.settings = (GravitonSettings)value;
                else throw new Exception("Bad argument");
            }
        }

        public new GravitonSettings BaseSettings {
            get {
                return baseSettings;
            }
            set {
                if (value is GravitonSettings) this.baseSettings = (GravitonSettings)value;
                else throw new Exception("Bad argument");
            }
        }
    }
}
