using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    public enum GenerationMode { STANDARD, STRANGE }

    /// <summary>
    /// Nastaveni pro generator
    /// </summary>
    [Serializable]
    public class GeneratorSettings : A_RockSettings
    {
        /// <summary>
        /// Pokud true, bude se generovat pravidelne
        /// </summary>
        public Boolean Regular_generating = false;
        /// <summary>
        /// Rychlost generovani castic
        /// </summary>
        public double generatingSpeed = PhysicSettings.Instance().DEFAULT_GENERATING_SPEED;
        /// <summary>
        /// Offset pro uhel generovani
        /// </summary>
        public double angle_offset = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_OFFSET;
        /// <summary>
        /// Maximalni uhel generovani
        /// </summary>
        public double angle_maximum = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_MAX;
        /// <summary>
        /// Minimalni rychlost generovanych castic
        /// </summary>
        public double particle_minimum_speed = PhysicSettings.Instance().DEFAULT_MIN_GENERATING_VELOCITY;
        /// <summary>
        /// Maximalni rychlost generovanych castic
        /// </summary>
        public double particle_maximum_speed = PhysicSettings.Instance().DEFAULT_MAX_GENERATING_VELOCITY;
        /// <summary>
        /// Mod generovani
        /// </summary>
        public GenerationMode generationMode = PhysicSettings.Instance().DEFAULT_GENERATION_MODE;
        /// <summary>
        /// Minimalni velikost castic pro generovani
        /// </summary>
        public double particle_minimum_size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
        /// <summary>
        /// Maximalni velikost castic pro generovani
        /// </summary>
        public double particle_maximum_size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;



    }
}
