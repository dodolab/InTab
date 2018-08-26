using System;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// General settings for black hole
    /// </summary>
     [Serializable]
    public class BlackHoleSettings : A_RockSettings
    {
         public double weigh = PhysicSettings.Instance().DEFAULT_BLACKHOLE_WEIGH;
         public int capacity = -1;
    }
}
