using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace InteractiveTable.Settings
{
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


        // maximalni generovatelny uhel
        public double DEFAULT_GENERATOR_ANGLE_MAX;
        // defaultni offset uhlu generovanych castic
        public double DEFAULT_GENERATOR_ANGLE_OFFSET;
        // pravidelne generovani
        public Boolean DEFAULT_GENERATING_REGULAR;
        // defaultni rychlost pro generovani castic (rychlost generovani)
        public double DEFAULT_GENERATING_SPEED ;
        // minimalni rychlost pro castici letici z generatoru
        public int DEFAULT_MIN_GENERATING_VELOCITY;
        // maximalni ryclhost pro castici letici z generatoru
        public int DEFAULT_MAX_GENERATING_VELOCITY;
        // defaultni polomer kamene
        public double DEFAULT_GRAVITON_RADIUS;
        // defaultni vaha kamene
        public double DEFAULT_GRAVITON_WEIGH;
        // defaultni polomer magnetonu
        public double DEFAULT_MAGNETON_RADIUS;
        // defaultni sila magnetonu
        public double DEFAULT_MAGNETON_FORCE;
        // defaultni polomer cerne diry
        public double DEFAULT_BLACKHOLE_RADIUS;
        // defaultni vaha cerne diry
        public double DEFAULT_BLACKHOLE_WEIGH;
        // defaultni kapacity cerne diry
        public int DEFAULT_BLACKHOLE_CAPACITY;

        // defaultni poloha objektu
        public FPoint DEFAULT_TABLEOBJECT_POINT ;
        // defaultni velikost vektoru
        public FVector DEFAULT_TABLEOBJECT_VELOCITY;
        // defaultni vaha castice
        public double DEFAULT_PARTICLE_WEIGH;
        // defaultni gravitacni konstanta (mensi nez obvykle, protoze tu mame mala telesa
        public double DEFAULT_GRAVITY_CONSTANT;
        // defaultni magneticka konstanta
        public double DEFAULT_MAGNETON_CONSTANT;

        // defaultni tolerance, kde castice muze byt, napr. 3 = max 3x dale nez je velikost stolu
        public double DEFAULT_PARTICLE_DISTANCE_TOLERATION;

        // defaultni velikost trajektorie, ktera se bude ukladat
        public int DEFAULT_TRAJECTORY_SIZE;

        // pokud true, bude mit stul vlastni gravitaci
        public Boolean DEFAULT_TABLE_GRAVITY;
        // defaultni gravitace stolu
        public FVector DEFAULT_TABLE_GRAVITY_VECTOR;
        // pokud true, budou se castice odrazet od sten
        public Boolean DEFAULT_INTERACTION_ALLOWED;

        // ztrata energie obecne platna pro stul
        public Boolean DEFAULT_ENERGY_TABLE_LOOSING;
        // rychlost ztraty energie obecne platne pro stul
        public double DEFAULT_ENERGY_TABLE_LOOSING_SPEED;
        // maximalni rychlost ztraty energie obecne platne pro stul
        public double DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX ;


        // pokud true, muzou castice menit velikost
        public Boolean DEFAULT_PARTICLE_SIZING_ENABLED;
        // defaultni velikost castice
        public double DEFAULT_PARTICLE_SIZE;


        // pulsovani jednotlivych kamenu:
        public Boolean DEFAULT_ENERGY_TABLE_PULSING;
        public double DEFAULT_ENERGY_TABLE_PULSING_SPEED;
        public Boolean DEFAULT_ENERGY_GRAVITON_PULSING;
        public double DEFAULT_ENERGY_GRAVITON_PULSING_SPEED;
        public Boolean DEFAULT_ENERGY_MAGNETON_PULSING;
        public double DEFAULT_ENERGY_MAGNETON_PULSING_SPEED;

        // povoleni jednotlivych kamenu:::
        public Boolean DEFAULT_TABLE_ENABLED;
        public Boolean DEFAULT_GENERATOR_ENABLED;
        public Boolean DEFAULT_GRAVITON_ENABLED;
        public Boolean DEFAULT_MAGNETON_ENABLED;
        public Boolean DEFAULT_PARTICLE_ENABLED;
        public Boolean DEFAULT_BLACKHOLE_ENABLED;


        // defaultni typ gravitacniho pusobeni
        public GravitationMode DEFAULT_GRAVITATION_MODE = GravitationMode.ADITIVE;
        // defaultni typ magnetickeho pusobeni
        public MagnetismMode DEFAULT_MAGNETISM_MODE = MagnetismMode.ADITIVE;
        // defaultni zpusob zvetsovani castic
        public ParticleSizeMode DEFAULT_PARTICLE_SIZE_MODE = ParticleSizeMode.VELOCITY;
        // defaultni zpusob generovani
        public GenerationMode DEFAULT_GENERATION_MODE = GenerationMode.STANDARD;
        // defaultni zpusob pohlcovani
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
                    MessageBox.Show("Vznikla chyba při načítání fyzikálního nastavení");
                }
            }
            else // soubor neexistuje
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
                MessageBox.Show("Vznikla chyba při ukládání fyzikálního nastavení");
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
