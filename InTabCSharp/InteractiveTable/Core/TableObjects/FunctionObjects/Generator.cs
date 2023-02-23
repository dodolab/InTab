using System;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Managers;
using InteractiveTable.Core.Data.TableObjects.Shapes;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Particle generator
    /// </summary>
     [Serializable]
    public class Generator : A_Rock
    {
        
        // number of failed attempts to create particles (used for probability distribution
        protected double generatingStep = 0;
        // number of generated particles
        public int generatingNumber = 0;
        // settings
        protected new GeneratorSettings settings;
         // global settings
        protected new GeneratorSettings baseSettings;

        public Generator()
        {
            this.shape = new FCircle();
            settings = new GeneratorSettings();
            settings.generatingSpeed = PhysicSettings.Instance().DEFAULT_GENERATING_SPEED;
            baseSettings = new GeneratorSettings();
        }

        public new GeneratorSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (value is GeneratorSettings) this.settings = (GeneratorSettings)value;
                else throw new Exception("Bad argument");
            }
        }

        public new GeneratorSettings BaseSettings
        {
            get
            {
                return baseSettings;
            }
            set
            {
                if (value is GeneratorSettings) this.baseSettings = (GeneratorSettings)value;
                else throw new Exception("Bad argument");
            }
        }

         /// <summary>
         /// Tries to generate particles. May return null if the generator is eiher empty
         /// or the random attempt failed
         /// </summary>
         /// <returns></returns>
        public Particle[] Generate(TableDepositor system, double counter)
        {
            GeneratorSettings settings = (settings_allowed) ? this.settings : this.baseSettings;

            generatingStep++;
            if (generatingStep > (PhysicSettings.Instance().DEFAULT_GENERATING_SPEED - settings.generatingSpeed))
            {
                // coefficient of regularity
                int coeff = settings.Regular_generating ? 1 : (CommonAttribService.apiRandom.Next(5)+1);
                int arrayLength = (int)((settings.generatingSpeed/coeff)*((A_Rock)this).Intensity/100);
                if (arrayLength <= 0) return null;
                Particle[] output = new Particle[arrayLength];



                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = GenerateParticle(counter);
                    // start over
                    generatingStep = 0;

                }
                generatingNumber++;
                return output;
            }
            else return null;
        }
        
        private Particle GenerateParticle(double counter)
        {

            Particle output = new Particle();
            output.Position = new FPoint(this.Position.X, this.Position.Y);

            try
            {
                output.Settings.size = CommonAttribService.apiRandom.Next((int)settings.particle_minimum_size, (int)settings.particle_maximum_size);
                output.Settings.originSize = output.Settings.size;
            }
            catch { }

            double velocity_x = 0;
            double velocity_y = 0;

            // calculate angle
            double angle = 0;

            if (settings.generationMode == GenerationMode.STRANGE)
            {
                angle = (Math.PI * 2) / 360 * (counter * 5);
            }
            else if (settings.generationMode == GenerationMode.STANDARD)
            {
                angle = (Math.PI * 2) / 360 * (CommonAttribService.apiRandom.Next((int)(((int)settings.angle_offset) / 5) * 5, (int)(((int)settings.angle_offset + settings.angle_maximum) / 5) * 5));
            }

            // calculate velocity
            velocity_x = Math.Cos(angle) * (CommonAttribService.apiRandom.NextDouble() * (settings.particle_maximum_speed - settings.particle_minimum_speed) + settings.particle_minimum_speed);
            velocity_y = Math.Sin(angle) * (CommonAttribService.apiRandom.NextDouble() * (settings.particle_maximum_speed - settings.particle_minimum_speed) + settings.particle_minimum_speed);

            output.Vector_Velocity = new FVector(velocity_x, velocity_y);

            return output;
        }
    }
}
