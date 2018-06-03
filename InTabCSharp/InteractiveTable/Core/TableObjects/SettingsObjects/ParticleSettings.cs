using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// Nastaveni pro castice
    /// </summary>
     [Serializable]
    public class ParticleSettings : A_TableObjectSettings
    {
        /// <summary>
        /// Vaha castice
        /// </summary>
         public double weigh = PhysicSettings.Instance().DEFAULT_PARTICLE_WEIGH;
        /// <summary>
        /// Velikost castice
        /// </summary>
         public double size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
        /// <summary>
        /// Puvodni velikost
        /// </summary>
         public double originSize = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
    }
}
