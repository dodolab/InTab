using System;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// General settings for particles
    /// </summary>
     [Serializable]
    public class ParticleSettings : A_TableObjectSettings
    {
        public double weigh = PhysicSettings.Instance().DEFAULT_PARTICLE_WEIGH;
        public double size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
        /// <summary>
        /// Size of the particle at the time it was generated
        /// </summary>
         public double originSize = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
    }
}
