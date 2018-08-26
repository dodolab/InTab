using System;
using InteractiveTable.Accessories;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// General settings for the table
    /// </summary>
     [Serializable]
    public class TableSettings : A_TableObjectSettings
    {
         /// <summary>
         /// Flag for collision with borders of the table
         /// </summary>
        public  Boolean interaction;
        public  Boolean gravity_allowed; 
         /// <summary>
         /// Table gravity
         /// </summary>
        public  FVector gravity; 
        public double energy_loosing_speed = -1; 
        public Boolean energy_loosing = false;

        public BlackHoleSettings blackHoleSettings;
        public GeneratorSettings generatorSettings;
        public GravitonSettings gravitonSettings;
        public MagnetonSettings magnetonSettings;
        public ParticleSettings particleSettings;

        public TableSettings()
        {
            energy_loosing = PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING;
            energy_loosing_speed = PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED;

            gravity_allowed = PhysicSettings.Instance().DEFAULT_TABLE_GRAVITY;
            gravity = PhysicSettings.Instance().DEFAULT_TABLE_GRAVITY_VECTOR;
            interaction = PhysicSettings.Instance().DEFAULT_INTERACTION_ALLOWED;

            blackHoleSettings = new BlackHoleSettings();
            blackHoleSettings.capacity = PhysicSettings.Instance().DEFAULT_BLACKHOLE_CAPACITY;
            blackHoleSettings.weigh = PhysicSettings.Instance().DEFAULT_BLACKHOLE_WEIGH;
            blackHoleSettings.enabled = PhysicSettings.Instance().DEFAULT_BLACKHOLE_ENABLED;

            generatorSettings = new GeneratorSettings();
            generatorSettings.angle_maximum = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_MAX;
            generatorSettings.angle_offset = PhysicSettings.Instance().DEFAULT_GENERATOR_ANGLE_OFFSET;
            generatorSettings.generatingSpeed = PhysicSettings.Instance().DEFAULT_GENERATING_SPEED;
            generatorSettings.particle_maximum_speed = PhysicSettings.Instance().DEFAULT_MAX_GENERATING_VELOCITY;
            generatorSettings.particle_minimum_speed = PhysicSettings.Instance().DEFAULT_MIN_GENERATING_VELOCITY;
            generatorSettings.Regular_generating = PhysicSettings.Instance().DEFAULT_GENERATING_REGULAR;
            generatorSettings.enabled = PhysicSettings.Instance().DEFAULT_GENERATOR_ENABLED;

            gravitonSettings = new GravitonSettings();
            gravitonSettings.Energy_pulse_speed = PhysicSettings.Instance().DEFAULT_ENERGY_GRAVITON_PULSING_SPEED;
            gravitonSettings.Energy_pulsing = PhysicSettings.Instance().DEFAULT_ENERGY_GRAVITON_PULSING;
            gravitonSettings.weigh = PhysicSettings.Instance().DEFAULT_GRAVITON_WEIGH;
            gravitonSettings.enabled = PhysicSettings.Instance().DEFAULT_GRAVITON_ENABLED;

            magnetonSettings = new MagnetonSettings();
            magnetonSettings.Energy_pulse_speed = PhysicSettings.Instance().DEFAULT_ENERGY_MAGNETON_PULSING_SPEED;
            magnetonSettings.Energy_pulsing = PhysicSettings.Instance().DEFAULT_ENERGY_MAGNETON_PULSING;
            magnetonSettings.force = PhysicSettings.Instance().DEFAULT_MAGNETON_FORCE;
            magnetonSettings.enabled = PhysicSettings.Instance().DEFAULT_MAGNETON_ENABLED;

            particleSettings = new ParticleSettings();
            particleSettings.weigh = PhysicSettings.Instance().DEFAULT_PARTICLE_WEIGH;
            particleSettings.enabled = PhysicSettings.Instance().DEFAULT_PARTICLE_ENABLED;
            particleSettings.size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;
        }
    }
}
