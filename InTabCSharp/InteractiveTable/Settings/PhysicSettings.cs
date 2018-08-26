using System;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace InteractiveTable.Settings
{
    /// <summary>
    /// Settings for physics
    /// </summary>
    public class PhysicSettings
    {

        private PhysicSettings()
        {
        }

        private static PhysicSettings phys = new PhysicSettings();

        public static PhysicSettings Instance()
        {
            return phys;
        }


        
        public double DEFAULT_GENERATOR_ANGLE_MAX;
        public double DEFAULT_GENERATOR_ANGLE_OFFSET;
        public Boolean DEFAULT_GENERATING_REGULAR;
        public double DEFAULT_GENERATING_SPEED ;
        public int DEFAULT_MIN_GENERATING_VELOCITY;
        public int DEFAULT_MAX_GENERATING_VELOCITY;
        public double DEFAULT_GRAVITON_RADIUS;
        public double DEFAULT_GRAVITON_WEIGH;
        public double DEFAULT_MAGNETON_RADIUS;
        public double DEFAULT_MAGNETON_FORCE;
        public double DEFAULT_BLACKHOLE_RADIUS;
        public double DEFAULT_BLACKHOLE_WEIGH;
        public int DEFAULT_BLACKHOLE_CAPACITY;

        public FPoint DEFAULT_TABLEOBJECT_POINT ;
        public FVector DEFAULT_TABLEOBJECT_VELOCITY;
        public double DEFAULT_PARTICLE_WEIGH;
        public double DEFAULT_GRAVITY_CONSTANT;
        public double DEFAULT_MAGNETON_CONSTANT;
        
        // default toleration of particle position if it goes beyond the borders of the table
        public double DEFAULT_PARTICLE_DISTANCE_TOLERATION;
        
        // default size of a trajectory that will be memoized
        public int DEFAULT_TRAJECTORY_SIZE;

        // if true, the table will have its own gravity
        public Boolean DEFAULT_TABLE_GRAVITY;
        
        public FVector DEFAULT_TABLE_GRAVITY_VECTOR;
        // if true, particle will collide with the borders of the table
        public Boolean DEFAULT_INTERACTION_ALLOWED;

        public Boolean DEFAULT_ENERGY_TABLE_LOOSING;
        public double DEFAULT_ENERGY_TABLE_LOOSING_SPEED;
        public double DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX ;

        // if true, particles may change their size
        public Boolean DEFAULT_PARTICLE_SIZING_ENABLED;
        // default sizes of particles
        public double DEFAULT_PARTICLE_SIZE;

        // pulsing of stones
        public Boolean DEFAULT_ENERGY_TABLE_PULSING;
        public double DEFAULT_ENERGY_TABLE_PULSING_SPEED;
        public Boolean DEFAULT_ENERGY_GRAVITON_PULSING;
        public double DEFAULT_ENERGY_GRAVITON_PULSING_SPEED;
        public Boolean DEFAULT_ENERGY_MAGNETON_PULSING;
        public double DEFAULT_ENERGY_MAGNETON_PULSING_SPEED;

        // we can also disable particular stone types
        public Boolean DEFAULT_TABLE_ENABLED;
        public Boolean DEFAULT_GENERATOR_ENABLED;
        public Boolean DEFAULT_GRAVITON_ENABLED;
        public Boolean DEFAULT_MAGNETON_ENABLED;
        public Boolean DEFAULT_PARTICLE_ENABLED;
        public Boolean DEFAULT_BLACKHOLE_ENABLED;


        public GravitationMode DEFAULT_GRAVITATION_MODE = GravitationMode.ADITIVE;
        public MagnetismMode DEFAULT_MAGNETISM_MODE = MagnetismMode.ADITIVE;
        public ParticleSizeMode DEFAULT_PARTICLE_SIZE_MODE = ParticleSizeMode.VELOCITY;
        public GenerationMode DEFAULT_GENERATION_MODE = GenerationMode.STANDARD;
        public AbsorptionMode DEFAULT_ABSORPTION_MODE = AbsorptionMode.BLACKHOLE;


        public void Load()
        {
            if (File.Exists("settings_physic.xml"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(PhysicSettings), new Type[] { typeof(FPoint), typeof(FVector) });
                    StreamReader reader = new StreamReader("settings_physic.xml");
                    PhysicSettings ph = (PhysicSettings)serializer.Deserialize(reader);
                    PhysicSettings.phys = ph;
                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error ocurred during loading physics settings");
                }
            }
            else // file doesn't exist
            {
                Restart();
            }
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PhysicSettings), new Type[] { typeof(FPoint), typeof(FVector) });
                StreamWriter writer = new StreamWriter("settings_physic.xml");
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch
            {
                MessageBox.Show("An error ocurred during saving of physical settings");
            }
        }

        public void Restart()
        {
          DEFAULT_GENERATOR_ANGLE_MAX = 360;
          DEFAULT_GENERATOR_ANGLE_OFFSET = 0;
          DEFAULT_GENERATING_REGULAR = false;
          DEFAULT_GENERATING_SPEED = 10;
          DEFAULT_MIN_GENERATING_VELOCITY = 1;
          DEFAULT_MAX_GENERATING_VELOCITY = 5;
          DEFAULT_GRAVITON_RADIUS = 3;
          DEFAULT_GRAVITON_WEIGH = 30;
          DEFAULT_MAGNETON_RADIUS = 3;
          DEFAULT_MAGNETON_FORCE = 10;
          DEFAULT_BLACKHOLE_RADIUS = 20;
          DEFAULT_BLACKHOLE_WEIGH = 8;
          DEFAULT_BLACKHOLE_CAPACITY = 100;
          DEFAULT_TABLEOBJECT_POINT = new FPoint(0, 0);
          DEFAULT_TABLEOBJECT_VELOCITY = new FVector(0, 0);
          DEFAULT_PARTICLE_WEIGH = 1;
          DEFAULT_GRAVITY_CONSTANT = 1.2;
          DEFAULT_MAGNETON_CONSTANT = 1.2;
          DEFAULT_PARTICLE_DISTANCE_TOLERATION = 3;
          DEFAULT_TRAJECTORY_SIZE = 10;
          DEFAULT_TABLE_GRAVITY = false;
          DEFAULT_TABLE_GRAVITY_VECTOR = new FVector(2, 0);
          DEFAULT_INTERACTION_ALLOWED = true;
          DEFAULT_ENERGY_TABLE_LOOSING = true;
          DEFAULT_ENERGY_TABLE_LOOSING_SPEED = 1.1;
          DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX = 10;
          DEFAULT_PARTICLE_SIZING_ENABLED = false;
          DEFAULT_PARTICLE_SIZE = 7;
          DEFAULT_ENERGY_TABLE_PULSING = false;
          DEFAULT_ENERGY_TABLE_PULSING_SPEED = 5;
          DEFAULT_ENERGY_GRAVITON_PULSING = false;
          DEFAULT_ENERGY_GRAVITON_PULSING_SPEED = 5;
          DEFAULT_ENERGY_MAGNETON_PULSING = false;
          DEFAULT_ENERGY_MAGNETON_PULSING_SPEED = 5;
          DEFAULT_TABLE_ENABLED = true;
          DEFAULT_GENERATOR_ENABLED = true;
          DEFAULT_GRAVITON_ENABLED = true;
          DEFAULT_MAGNETON_ENABLED = true;
          DEFAULT_PARTICLE_ENABLED = true;
          DEFAULT_BLACKHOLE_ENABLED = true;
          DEFAULT_GRAVITATION_MODE = GravitationMode.ADITIVE;
          DEFAULT_MAGNETISM_MODE = MagnetismMode.ADITIVE;
          DEFAULT_PARTICLE_SIZE_MODE = ParticleSizeMode.VELOCITY;
          DEFAULT_GENERATION_MODE = GenerationMode.STANDARD;
          DEFAULT_ABSORPTION_MODE = AbsorptionMode.BLACKHOLE;
        }
    }
}
