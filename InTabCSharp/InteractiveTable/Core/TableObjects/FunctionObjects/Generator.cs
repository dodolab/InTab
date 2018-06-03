using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Settings;
using System.Windows;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Core.Physics.System;
using InteractiveTable.Managers;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Generator castic
    /// </summary>
     [Serializable]
    public class Generator : A_Rock
    {

        // pocet neuspesnych volani metody generate
        protected double generatingStep = 0;
        // pocet vygenerovanych objektu
        public int generatingNumber = 0;
        // nastaveni
        protected new GeneratorSettings settings;
         // globalni nastaveni
        protected new GeneratorSettings baseSettings;

        public Generator()
        {
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
                else throw new Exception("Spatny argument");
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
                else throw new Exception("Spatny argument");
            }
        }

         /// <summary>
         /// Vrati pole vygenerovanych objektu; zavisi to na poctu volani teto metody
         /// objekt se vygeneruje kazdy n-ty krok
         /// </summary>
         /// <param name="system"></param>
         /// <param name="counter">update time</param>
         /// <returns></returns>
        public Particle[] Generate(TableDepositor system, double counter)
        {
            GeneratorSettings settings = (settings_allowed) ? this.settings : this.baseSettings;

            generatingStep++;
            if (generatingStep > (PhysicSettings.Instance().DEFAULT_GENERATING_SPEED - settings.generatingSpeed))
            {
                // koeficient PRAVIDELNOSTI
                int coeff = settings.Regular_generating ? 1 : (CommonAttribService.apiRandom.Next(5)+1);
                int arrayLength = (int)((settings.generatingSpeed/coeff)*((A_Rock)this).Intensity/100);
                if (arrayLength <= 0) return null;
                Particle[] output = new Particle[arrayLength];



                for (int i = 0; i < output.Length; i++)
                {
                    // vygeneruje castici
                    output[i] = GenerateParticle(counter);
                    // nastavime na 0, protoze byla metoda uspesne provedena
                    generatingStep = 0;

                }
                generatingNumber++;
                return output;
            }
            else return null;
        }

         /// <summary>
         /// Vygeneruje castici
         /// </summary>
         /// <param name="counter">update time pro podivne generovani</param>
         /// <returns></returns>
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

            // spocitame uhel generovani
            double angle = 0;

            if (settings.generationMode == GenerationMode.STRANGE)
            {
                angle = (Math.PI * 2) / 360 * (counter * 5);
            }
            else if (settings.generationMode == GenerationMode.STANDARD)
            {
                angle = (Math.PI * 2) / 360 * (CommonAttribService.apiRandom.Next((int)(((int)settings.angle_offset) / 5) * 5, (int)(((int)settings.angle_offset + settings.angle_maximum) / 5) * 5));
            }

            // spocitame rychlost castice
            velocity_x = Math.Cos(angle) * (CommonAttribService.apiRandom.NextDouble() * (settings.particle_maximum_speed - settings.particle_minimum_speed) + settings.particle_minimum_speed);
            velocity_y = Math.Sin(angle) * (CommonAttribService.apiRandom.NextDouble() * (settings.particle_maximum_speed - settings.particle_minimum_speed) + settings.particle_minimum_speed);

            output.Vector_Velocity = new FVector(velocity_x, velocity_y);

            return output;
        }
    }
}
