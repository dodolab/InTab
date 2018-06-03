using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// Nastaveni pro magneton
    /// </summary>
     [Serializable]
    public class MagnetonSettings : A_RockSettings
    {
        /// <summary>
        /// Sila magnetonu
        /// </summary>
        public double force = PhysicSettings.Instance().DEFAULT_MAGNETON_FORCE;
    }
}
