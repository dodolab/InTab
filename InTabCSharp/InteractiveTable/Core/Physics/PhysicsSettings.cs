using System;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Physics.System
{
    public enum GravitationMode {ADITIVE,AVERAGE,MULTIPLY}
    public enum MagnetismMode { ADITIVE, AVERAGE, MULTIPLY }
    public enum ParticleSizeMode { GRAVITY, VELOCITY, WEIGH, NONE }
    public enum AbsorptionMode {BLACKHOLE, RECYCLE, SELECT}
    
    /// <summary>
    /// Physics engine settings
    /// </summary>
    public class PhysicsSettings
    {
        public static GravitationMode gravitationMode = PhysicSettings.Instance().DEFAULT_GRAVITATION_MODE;
        public static MagnetismMode magnetismMode = PhysicSettings.Instance().DEFAULT_MAGNETISM_MODE;
        public static ParticleSizeMode particle_sizeMode = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE_MODE;
        public static AbsorptionMode absorptionMode = PhysicSettings.Instance().DEFAULT_ABSORPTION_MODE;
        public static Boolean particle_sizeChanging_allowed = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZING_ENABLED;
        public static double particle_size = PhysicSettings.Instance().DEFAULT_PARTICLE_SIZE;

    }
}
