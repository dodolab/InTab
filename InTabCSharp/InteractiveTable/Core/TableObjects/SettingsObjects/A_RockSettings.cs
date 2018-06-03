using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// Spolecne nastaveni pro vsechny objekty typu ROCK
    /// </summary>
     [Serializable]
    public class A_RockSettings : A_TableObjectSettings
    {
         /// <summary>
         /// Pokud true, bude objekt pulzovat
         /// </summary>
         public Boolean Energy_pulsing = false;
         /// <summary>
         /// Rychlost pulzovani
         /// </summary>
        public  double Energy_pulse_speed = PhysicSettings.Instance().DEFAULT_ENERGY_GRAVITON_PULSING_SPEED; 
    }
}
