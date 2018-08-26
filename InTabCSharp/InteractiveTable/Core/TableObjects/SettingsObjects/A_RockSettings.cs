using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// General settings for all stones
    /// </summary>
     [Serializable]
    public class A_RockSettings : A_TableObjectSettings
    {
         /// <summary>
         /// If true, the stone will throb
         /// </summary>
         public Boolean Energy_pulsing = false;
         
        /// <summary>
         /// Speed of throbing
         /// </summary>
        public  double Energy_pulse_speed = PhysicSettings.Instance().DEFAULT_ENERGY_GRAVITON_PULSING_SPEED; 
    }
}
