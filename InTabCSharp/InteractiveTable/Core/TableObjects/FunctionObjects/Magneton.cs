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
    /// Magnet stone
    /// </summary>
     [Serializable]
    public class Magneton : A_Rock
    {
       protected new MagnetonSettings settings;
       protected new MagnetonSettings baseSettings;

      public Magneton()
      {
          settings = new MagnetonSettings();
          baseSettings = new MagnetonSettings();
          this.shape = new FCircle();
          ((FCircle)shape).Radius = PhysicSettings.Instance().DEFAULT_MAGNETON_RADIUS;
          settings.force = PhysicSettings.Instance().DEFAULT_MAGNETON_FORCE;
      }

      public new MagnetonSettings Settings
      {
          get
          {
              return settings;
          }
          set
          {
              if (value is MagnetonSettings) this.settings = (MagnetonSettings)value;
              else throw new Exception("Bad argument");
          }
      }

      public new MagnetonSettings BaseSettings
      {
          get
          {
              return baseSettings;
          }
          set
          {
              if (value is MagnetonSettings) this.baseSettings = (MagnetonSettings)value;
              else throw new Exception("Bad argument");
          }
      }


    }
}
