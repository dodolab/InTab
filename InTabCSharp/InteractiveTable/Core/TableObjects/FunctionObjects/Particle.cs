using System;
using InteractiveTable.Settings;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Particle entity
    /// </summary>
   [Serializable]
   public class Particle : A_TableObject
    {
       protected FVector vector_Velocity;
       protected FVector vector_Acceleration;
       protected FPoint vector_Position;

       protected new ParticleSettings settings;

       public Particle()
       {
           settings = new ParticleSettings();
           baseSettings = new ParticleSettings();
           this.vector_Velocity = PhysicSettings.Instance().DEFAULT_TABLEOBJECT_VELOCITY;
           this.vector_Acceleration = new FVector(0, 0);
           settings.weigh = PhysicSettings.Instance().DEFAULT_PARTICLE_WEIGH;
       }

       public new ParticleSettings Settings
       {
           get
           {
               return settings;
           }
           set
           {
               if (value is ParticleSettings) this.settings = (ParticleSettings)value;
               else throw new Exception("Bad argument");
           }
       }

       public override FPoint Position
       {
           get
           {
               return vector_Position;
           }
           set
           {
               vector_Position = value;
           }
       }

        public FVector Vector_Velocity
        {
            get
            {
                return vector_Velocity;
            }
            set
            {
                this.vector_Velocity = value;
            }
        }

        public FVector Vector_Acceleration
        {
            get
            {
                return vector_Acceleration;
            }
            set
            {
                vector_Acceleration = value;
            }
        }

 
    }
}
