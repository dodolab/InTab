using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Managers;

namespace InteractiveTable.Core.Physics.System {
    /// <summary>
    /// Physics engine
    /// </summary>
    public class PhysicsLogic {
        /// <summary>
        /// Global counter
        /// </summary>
        public static double counter = 0;
        /// <summary>
        /// Buffer for particles that will be deleted at the end of the loop
        /// </summary>
        private static HashSet<Particle> deletedParticles = new HashSet<Particle>();
        /// <summary>
        /// Helper for temporary positions calculation
        /// </summary>
        private static FVector temp = new FVector(0, 0);

        /// <summary>
        /// Transforms the system from to another state
        /// </summary>
        /// <param name="system"></param>
        public static void TransformSystem(TableDepositor system) {
            counter++;
            try {

                if (system.table.Settings.generatorSettings.enabled) {
                    ProcessGenerators(system); // generate new particles
                }
                ProcessParticles(system); // process particles

            } catch {
                // .... no-op here
            }
        }

        /// <summary>
        /// Processes all particle generators
        /// </summary>
        /// <param name="system"></param>
        private static void ProcessGenerators(TableDepositor system) {
            foreach (Generator ab in system.generators) {
                Particle[] obj = ab.Generate(system, counter); // try to generate new particles
                if (obj != null) // add new particles to the system
                {
                    for (int i = 0; i < obj.Length; i++) {
                        system.particles.Add(obj[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Processes all particles
        /// </summary>
        private static void ProcessParticles(TableDepositor system) {
            double tableWidth = CommonAttribService.ACTUAL_TABLE_WIDTH;
            double tableHeight = CommonAttribService.ACTUAL_TABLE_HEIGHT;


            foreach (Particle part in system.particles) {

                // size change
                if (PhysicsSettings.particle_sizeChanging_allowed) {
                    FixParticleSizeChange(part, system);
                }

                // reset acceleration - it will be recalculated
                part.Vector_Acceleration.X = 0;
                part.Vector_Acceleration.Y = 0;

                // process stones
                ProcessParticleStoneInterraction(part, system);

                // set all new values
                part.Position = new FPoint(part.Position.X + part.Vector_Velocity.X,
                    part.Position.Y + part.Vector_Velocity.Y);

                // table gravity
                if (system.table.Settings.gravity_allowed) {
                    part.Vector_Acceleration.X += 0.1 * system.table.Settings.gravity.X;
                    part.Vector_Acceleration.Y += 0.1 * system.table.Settings.gravity.Y;
                }

                part.Vector_Velocity.Add(part.Vector_Acceleration);

                // loosing energy
                if (system.table.Settings.energy_loosing || system.table.Settings.energy_loosing) {
                    FixParticleEnergyLoosing(part, system);
                }

                // collision with the borders of the table
                if (system.table.Settings.interaction) {
                    FixParticleTableInterraction(part, system, tableWidth, tableHeight);
                }
            }

            // delete particles that are to be deleted
            DeleteBufferedParticles(system);
        }

        /// <summary>
        /// Process interaction between particles and stones
        /// </summary>
        private static void ProcessParticleStoneInterraction(Particle part, TableDepositor system) {
            if (system.table.Settings.blackHoleSettings.enabled) {
                ProcessBlackHoles(system, part);
            }
            if (system.table.Settings.gravitonSettings.enabled) {
                ProcessGravitons(system, part);
            }
            if (system.table.Settings.magnetonSettings.enabled) {
                ProcessMagnetons(system, part);
            }
        }

        /// <summary>
        /// Change particle sizes due various attributse
        /// </summary>
        private static void FixParticleSizeChange(Particle part, TableDepositor system) {
            if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.GRAVITY) {
                part.Settings.size = 1 + (part.Vector_Acceleration.X * part.Vector_Acceleration.X + part.Vector_Acceleration.Y * part.Vector_Acceleration.Y);
            } else if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.VELOCITY) {
                part.Settings.size = 1 + part.Vector_Velocity.Size();
            } else if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.WEIGH) {
                part.Settings.size = part.Settings.weigh * 2;
            } else if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.NONE) {
                part.Settings.size = part.Settings.originSize;
            }
        }

        /// <summary>
        /// Change particle energy
        /// </summary>
        /// <param name="part"></param>
        /// <param name="system"></param>
        private static void FixParticleEnergyLoosing(Particle part, TableDepositor system) {
            // if the value is -1, we will use global settings
            if (system.table.Settings.energy_loosing_speed != -1) {
                part.Vector_Velocity.Mult(1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX,
                    1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX);
            } else {
                part.Vector_Velocity.Mult(1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX,
                    1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX);
            }
        }

        /// <summary>
        /// Change particle velocity if it interacts with borders of the table
        /// </summary>
        private static void FixParticleTableInterraction(Particle part, TableDepositor system, double tableWidth, double tableHeight) {
            if (part.Position.X < -tableWidth / 2 && part.Vector_Velocity.X < 0) part.Vector_Velocity.Mult(-1, 1);
            if (part.Position.X > tableWidth / 2 && part.Vector_Velocity.X > 0) part.Vector_Velocity.Mult(-1, 1);
            if (part.Position.Y < -tableHeight / 2 && part.Vector_Velocity.Y < 0) part.Vector_Velocity.Mult(1, -1);
            if (part.Position.Y > tableHeight / 2 && part.Vector_Velocity.Y > 0) part.Vector_Velocity.Mult(1, -1);
        }

        private static void DeleteBufferedParticles(TableDepositor system) {
            foreach (Particle part in deletedParticles) {
                system.particles.Remove(part);
            }
            deletedParticles.Clear();
        }

        /// <summary>
        /// Process black holes for one particle
        /// </summary>
        public static void ProcessBlackHoles(TableDepositor system, Particle part) {
            foreach (BlackHole blackH in system.blackHoles) {
                BlackHoleSettings settings = (blackH.Settings_Allowed) ? blackH.Settings : system.table.Settings.blackHoleSettings;

                // distance between the particle and the stone
                double length = Math.Sqrt((part.Position.X - blackH.Position.X) * (part.Position.X - blackH.Position.X) +
                                    (part.Position.Y - blackH.Position.Y) * (part.Position.Y - blackH.Position.Y));

                // if the distance is within the treshold, we will remove it
                if (length < blackH.Radius) {
                    if (PhysicsSettings.absorptionMode == AbsorptionMode.BLACKHOLE) {
                        deletedParticles.Add(part);
                    } else if (PhysicsSettings.absorptionMode == AbsorptionMode.RECYCLE) {
                        // if the recycling is enabled, just find an appropriate generator and give it the deleted particle
                        if (system.generators.Count > 0) {
                            system.generators.ElementAt(CommonAttribService.apiRandom.Next(system.generators.Count - 1)).generatingNumber--;
                            deletedParticles.Add(part);
                        }
                    } else if (PhysicsSettings.absorptionMode == AbsorptionMode.SELECT) {
                        if (CommonAttribService.apiRandom.Next(50) == 40) deletedParticles.Add(part);
                    }
                    return;
                }

                // creepy gravity acceleration
                double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * settings.weigh) / (Math.Log(length * length) / 114 + 14) * (((A_Rock)blackH).Intensity / 100);
                double dist_x = (blackH.Position.X - part.Position.X);
                double dist_y = (blackH.Position.Y - part.Position.Y);
                double celk = length * 1.5;

                // pulsar
                if (settings.Energy_pulsing) {
                    acc /= (1 + Math.Tan(counter * settings.Energy_pulse_speed) / (4 + 4 * Math.Log10(length)));
                }

                part.Vector_Acceleration.Add(0.5 * acc * ((dist_x) / length),
                    0.5 * acc * ((dist_y) / length));

                // this is really creepy but it works
                if (dist_x > 0 && (-dist_y) > 0) dist_y *= length / 4;
                else if (dist_x > 0 && (-dist_y) < 0) dist_x *= length / 4;
                else if (dist_x < 0 && (-dist_y) > 0) dist_x *= length / 4;
                else if (dist_x < 0 && (-dist_y) < 0) dist_y *= length / 4;

                // add gravityy acceleration
                part.Vector_Acceleration.Add((-dist_y / celk) / 30, (dist_x / celk) / 30);
            }
        }


        /// <summary>
        /// Processes gravitons for one particle
        /// </summary>
        /// <param name="system"></param>
        /// <param name="part"></param>
        public static void ProcessGravitons(TableDepositor system, Particle part) {
            temp.X = 0;
            temp.Y = 0;

            foreach (Graviton ig in system.gravitons) {
                GravitonSettings settings = (ig.Settings_Allowed) ? ig.Settings : system.table.Settings.gravitonSettings;

                double length = (part.Position.X - ig.Position.X) * (part.Position.X - ig.Position.X) +
                                    (part.Position.Y - ig.Position.Y) * (part.Position.Y - ig.Position.Y);

                //=================================================
                // GRAVITY:
                // g = CONST*M/(radius/11 + 14), 
                //=================================================

                // fix treshold
                double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * ig.Settings.weigh) / (length / 114 + 14) * (((A_Rock)ig).Intensity / 100);
                double sqrt_length = Math.Sqrt(length);
                if (sqrt_length < 20) {
                    acc *= -Math.Log(sqrt_length - Math.Min(20, sqrt_length - 2));
                }

                // pulsar
                if (settings.Energy_pulsing) {
                    acc /= (1 + Math.Tan(settings.Energy_pulse_speed) / 10);

                }

                // calculate velocity 
                if (PhysicsSettings.gravitationMode == GravitationMode.ADITIVE) {
                    temp.Add(acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length)),
                        acc * ((ig.Position.Y - part.Position.Y) / Math.Sqrt(length)));
                } else if (PhysicsSettings.gravitationMode == GravitationMode.AVERAGE) {
                    temp.Add(1 / (acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length))),
                        1 / (acc * ((ig.Position.Y - part.Position.Y) / Math.Sqrt(length))));
                } else if (PhysicsSettings.gravitationMode == GravitationMode.MULTIPLY) {
                    temp.Add(acc * ((ig.Position.X - part.Position.X) / Math.Log(length)),
                        acc * ((ig.Position.Y - part.Position.Y) / Math.Log(length)));
                }

            }

            if (PhysicsSettings.gravitationMode == GravitationMode.AVERAGE) temp.Invert();

            part.Vector_Acceleration.Add(temp);
        }

        /// <summary>
        /// Processes magnetons for one particle
        /// </summary>
        public static void ProcessMagnetons(TableDepositor system, Particle part) {
            temp.X = 0;
            temp.Y = 0;

            foreach (Magneton ig in system.magnetons) {
                MagnetonSettings settings = (ig.Settings_Allowed) ? ig.Settings : system.table.Settings.magnetonSettings;

                // calculate distance between the particle and the stone
                double length = (part.Position.X - ig.Position.X) * (part.Position.X - ig.Position.X) +
                                    (part.Position.Y - ig.Position.Y) * (part.Position.Y - ig.Position.Y);

                double acc = (PhysicSettings.Instance().DEFAULT_MAGNETON_CONSTANT * ig.Settings.force) / (length / 114 + 14) * (((A_Rock)ig).Intensity / 100);


                // pulsar
                if (settings.Energy_pulsing) {
                    acc /= (1 + Math.Tan(counter * settings.Energy_pulse_speed) / 10);
                }

                if (PhysicsSettings.magnetismMode == MagnetismMode.ADITIVE) {
                    temp.Add(-acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length)),
                        -acc * ((ig.Position.Y - part.Position.Y) / Math.Sqrt(length)));
                } else if (PhysicsSettings.magnetismMode == MagnetismMode.AVERAGE) {
                    temp.Add(1 / (-acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length))),
                        1 / (-acc * ((ig.Position.Y - part.Position.Y) / Math.Sqrt(length))));
                } else if (PhysicsSettings.magnetismMode == MagnetismMode.MULTIPLY) {
                    temp.Add(-acc * ((ig.Position.X - part.Position.X) / Math.Log(length)),
                        -acc * ((ig.Position.Y - part.Position.Y) / Math.Log(length)));
                }

            }

            if (PhysicsSettings.magnetismMode == MagnetismMode.AVERAGE) temp.Invert();
            part.Vector_Acceleration.Add(temp);
        }
    }
}
