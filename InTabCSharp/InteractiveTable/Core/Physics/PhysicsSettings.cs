using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;

namespace InteractiveTable.Core.Physics.System
{
    /// <summary>
    /// Gravitacni mod
    /// </summary>
    public enum GravitationMode {ADITIVE,AVERAGE,MULTIPLY}
    /// <summary>
    /// Magneticky mod
    /// </summary>
    public enum MagnetismMode { ADITIVE, AVERAGE, MULTIPLY }
    /// <summary>
    /// Mod pro velikosti castic
    /// </summary>
    public enum ParticleSizeMode { GRAVITY, VELOCITY, WEIGH, NONE }
    /// <summary>
    /// Absorpcni mod
    /// </summary>
    public enum AbsorptionMode {BLACKHOLE, RECYCLE, SELECT}
    
    /// <summary>
    /// Individualni nastaveni pro fyziku
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
