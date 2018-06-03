using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
   /// <summary>
   /// Kamen predstavujici gravitaci
   /// </summary>
     [Serializable]
  public class Graviton : A_Rock
    {
      // nastaveni
      protected new GravitonSettings settings;
      // globalni nastaveni
      protected new GravitonSettings baseSettings;

      public Graviton()
      {
          settings = new GravitonSettings();
          baseSettings = new GravitonSettings();
          this.shape = new FCircle();
          ((FCircle)shape).Radius = PhysicSettings.Instance().DEFAULT_GRAVITON_RADIUS;
          settings.weigh = PhysicSettings.Instance().DEFAULT_GRAVITON_WEIGH;
      }

      public new GravitonSettings Settings
      {
          get
          {
              return settings;
          }
          set
          {
              if (value is GravitonSettings) this.settings = (GravitonSettings)value;
              else throw new Exception("Spatny argument");
          }
      }

      public new GravitonSettings BaseSettings
      {
          get
          {
              return baseSettings;
          }
          set
          {
              if (value is GravitonSettings) this.baseSettings = (GravitonSettings)value;
              else throw new Exception("Spatny argument");
          }
      }
    }
}
