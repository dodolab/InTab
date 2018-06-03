using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using InteractiveTable.Settings;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.GUI.Other;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Kamen predstavujici castici
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
               else throw new Exception("Spatny argument");
           }
       }

         /// <summary>
         /// Vrati nebo nastavi pozici castice
         /// </summary>
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

         /// <summary>
         /// Vrati nebo nastavi rychlost castice
         /// </summary>
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

         /// <summary>
         /// Vrati nebo nastavi zrychleni castice
         /// </summary>
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
