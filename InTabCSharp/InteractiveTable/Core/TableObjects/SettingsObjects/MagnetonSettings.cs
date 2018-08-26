using System;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// General settings for magneton
    /// </summary>
     [Serializable]
    public class MagnetonSettings : A_RockSettings
    {
        /// <summary>
        /// Power of the magneton
        /// </summary>
        public double force = PhysicSettings.Instance().DEFAULT_MAGNETON_FORCE;
    }
}
