using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// General settings for gravitons
    /// </summary>
     [Serializable]
    public class GravitonSettings : A_RockSettings
    {
        public double weigh = PhysicSettings.Instance().DEFAULT_GRAVITON_WEIGH;
    }
}
