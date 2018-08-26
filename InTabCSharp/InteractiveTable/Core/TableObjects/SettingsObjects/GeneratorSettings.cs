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
    /// Generator settings
    /// </summary>
    [Serializable]
    public class GeneratorSettings : A_RockSettings
    {
        /// <summary>
        /// Indicator whether or not the generating is regular
        /// </summary>
        public Boolean Regular_generating = false;
        /// <summary>
        /// Particle generating speed
        /// </summary>
        public double generatingSpeed = PhysicSettings.Instance().DEFAULT_GENERATING_SPEED;
        /// <summary>
        /// Generating angle offset
        /// </summary>
        public double angle_offset = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_OFFSET;
        /// <summary>
        /// Max generating angle
        /// </summary>
        public double angle_maximum = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_MAX;
        /// <summary>
        /// Min velocity of generated particles
        /// </summary>
        public double particle_minimum_speed = PhysicSettings.Instance().DEFAULT_MIN_GENERATING_VELOCITY;
        /// <summary>
        /// Max velocity of generated particles
        /// </summary>
        public double particle_maximum_speed = PhysicSettings.Instance().DEFAULT_MAX_GENERATING_VELOCITY;
        /// <summary>
        /// Generating mode
        /// </summary>
        public GenerationMode generationMode = PhysicSettings.Instance().DEFAULT_GENERATION_MODE;
        /// <summary>
        /// Min particles size for generating
        /// </summary>
        public double particle_minimum_size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
        /// <summary>
        /// Max particles size for generating
        /// </summary>
        public double particle_maximum_size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;



    }
}
