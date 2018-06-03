using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// Nastaveni pro cernou diru
    /// </summary>
     [Serializable]
    public class BlackHoleSettings : A_RockSettings
    {
        
         /// <summary>
         /// Vaha cerne diry
         /// </summary>
         public double weigh = PhysicSettings.Instance().DEFAULT_BLACKHOLE_WEIGH;
        /// <summary>
        /// Kapacita cerne diry
        /// </summary>
         public int capacity = -1;
    }
}
