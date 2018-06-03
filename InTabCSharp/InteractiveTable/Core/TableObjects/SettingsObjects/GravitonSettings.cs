using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// Nastaveni pro graviton
    /// </summary>
     [Serializable]
    public class GravitonSettings : A_RockSettings
    {
        /// <summary>
        /// Hmotnost gravitonu
        /// </summary>
        public double weigh = PhysicSettings.Instance().DEFAULT_GRAVITON_WEIGH;
    }
}
