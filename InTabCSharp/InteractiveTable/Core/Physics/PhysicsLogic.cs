using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using System.Windows;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Managers;

namespace InteractiveTable.Core.Physics.System
{
    /// <summary>
    /// Logika pro fyzikalni zpracovani a transformaci soustavy
    /// </summary>
    public class PhysicsLogic
    {
        /// <summary>
        /// Globalni citac
        /// </summary>
        public static double counter = 0; 
        /// <summary>
        /// Buffer, uklada do sebe vsechny castice, ktere budou po probehnuti cyklu smazany
        /// </summary>
        private static HashSet<Particle> deletedParticles = new HashSet<Particle>();
        /// <summary>
        /// Pomocna promenna pro ukladani pozic
        /// </summary>
        private static FVector temp = new FVector(0, 0);

        /// <summary>
        /// Transformuje soustavu ze stavu A do stavu B
        /// </summary>
        /// <param name="system"></param>
        public static void TransformSystem(TableDepositor system)
        {
            counter++;
            try
            {

               if (system.table.Settings.generatorSettings.enabled)
               {
                    ProcessGenerators(system); // vygeneruj nove castice
               }
                ProcessParticles(system); // postarej se o castice a jejich interakce

            }
            catch { 
            // invalid operation exception
           //  Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!! VYJIMKA PRI VYKRESLOVANI");
            }
        }

        /// <summary>
        /// Fyzikalne zpracuje vsechny generatory
        /// </summary>
        /// <param name="system"></param>
        private static void ProcessGenerators(TableDepositor system)
        {
            // vygeneruji nove castice
            foreach (Generator ab in system.generators)
            {
                Particle[] obj = ab.Generate(system,counter); // zkusim vygenerovat objekt/y
                if (obj != null) // objekt se podarilo vygenerovat, pridam ho
                {
                    for (int i = 0; i < obj.Length; i++)
                    {
                        system.particles.Add(obj[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Zpracuje vsechny castice
        /// </summary>
        /// <param name="system"></param>
        private static void ProcessParticles(TableDepositor system)
        {
            double tableWidth = CommonAttribService.ACTUAL_TABLE_WIDTH;
            double tableHeight = CommonAttribService.ACTUAL_TABLE_HEIGHT;


            foreach (Particle part in system.particles)
            {

                // zavislost velikosti castic
                if (PhysicsSettings.particle_sizeChanging_allowed)
                {
                    FixParticleSizeChange(part, system);
                }

                // Nyni se vynuluje zrychleni, aby mohlo byt pristi superponovano
                part.Vector_Acceleration.X = 0;
                part.Vector_Acceleration.Y = 0;

                // zpracuj kameny
                ProcessParticleStoneInterraction(part, system);

                // nastav vsechny nove udaje
                part.Position = new FPoint(part.Position.X + part.Vector_Velocity.X,
                    part.Position.Y + part.Vector_Velocity.Y);

                // osetrime gravitaci stolu
                if (system.table.Settings.gravity_allowed)
                {
                    part.Vector_Acceleration.X += 0.1 * system.table.Settings.gravity.X;
                    part.Vector_Acceleration.Y += 0.1 * system.table.Settings.gravity.Y;
                }

                // uprav rychlost podle zrychleni
                part.Vector_Velocity.Add(part.Vector_Acceleration);
 
                // ztrata energie
                if (system.table.Settings.energy_loosing || system.table.Settings.energy_loosing)
                {  
                    FixParticleEnergyLoosing(part, system);
                }

                // odrazeni od sten
                if (system.table.Settings.interaction)
                {
                    FixParticleTableInterraction(part, system, tableWidth, tableHeight);
                }
            }

            // smaz castice, co maji byt smazany
            DeleteBufferedParticles(system);
        }

        /// <summary>
        /// Zpracuje interakci mezi castici a kameny
        /// </summary>
        /// <param name="part"></param>
        /// <param name="system"></param>
        private static void ProcessParticleStoneInterraction(Particle part, TableDepositor system)
        {
            if (system.table.Settings.blackHoleSettings.enabled)
            {
                ProcessBlackHoles(system, part);
            }
            if (system.table.Settings.gravitonSettings.enabled)
            {
                ProcessGravitons(system, part);
            }
            if (system.table.Settings.magnetonSettings.enabled)
            {
                ProcessMagnetons(system, part);
            }
        }

        /// <summary>
        /// Osetri zavislost velikosti castice
        /// </summary>
        /// <param name="part"></param>
        /// <param name="system"></param>
        private static void FixParticleSizeChange(Particle part, TableDepositor system)
        {
            if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.GRAVITY)
            {
                part.Settings.size = 1 + (part.Vector_Acceleration.X * part.Vector_Acceleration.X + part.Vector_Acceleration.Y * part.Vector_Acceleration.Y);
            }
            else if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.VELOCITY)
            {
                part.Settings.size = 1 + part.Vector_Velocity.Size();
            }
            else if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.WEIGH)
            {
                part.Settings.size = part.Settings.weigh * 2;
            }
            else if (PhysicsSettings.particle_sizeMode == ParticleSizeMode.NONE)
            {
                part.Settings.size = part.Settings.originSize;
            }
        }

        /// <summary>
        /// Osetri ztratu enerige castice
        /// </summary>
        /// <param name="part"></param>
        /// <param name="system"></param>
        private static void FixParticleEnergyLoosing(Particle part, TableDepositor system)
        {
            // pokud je hodnota -1, bude platne globalni nastaveni, jinak lokalni
            if (system.table.Settings.energy_loosing_speed != -1)
            {
                part.Vector_Velocity.Mult(1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX,
                    1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX);
            }
            else
            {
                part.Vector_Velocity.Mult(1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX,
                    1 - system.table.Settings.energy_loosing_speed / PhysicSettings.Instance().DEFAULT_ENERGY_TABLE_LOOSING_SPEED_MAX);
            }
        }

        /// <summary>
        /// Osetri odrazeni castic od steny stolu
        /// </summary>
        /// <param name="part"></param>
        /// <param name="system"></param>
        /// <param name="tableWidth"></param>
        /// <param name="tableHeight"></param>
        private static void FixParticleTableInterraction(Particle part, TableDepositor system, double tableWidth, double tableHeight)
        {
            if (part.Position.X < -tableWidth / 2 && part.Vector_Velocity.X < 0) part.Vector_Velocity.Mult(-1, 1);
            if (part.Position.X > tableWidth / 2 && part.Vector_Velocity.X > 0) part.Vector_Velocity.Mult(-1, 1);
            if (part.Position.Y < -tableHeight / 2 && part.Vector_Velocity.Y < 0) part.Vector_Velocity.Mult(1, -1);
            if (part.Position.Y > tableHeight / 2 && part.Vector_Velocity.Y > 0) part.Vector_Velocity.Mult(1, -1);
        }

        /// <summary>
        /// Smaze castice z bufferu
        /// </summary>
        private static void DeleteBufferedParticles(TableDepositor system)
        {
            foreach (Particle part in deletedParticles)
            {
                system.particles.Remove(part);
            }
            deletedParticles.Clear();
        }

        /// <summary>
        /// Zpracuje cerne diry pro jednu castici
        /// </summary>
        /// <param name="system"></param>
        /// <param name="part"></param>
        public static void ProcessBlackHoles(TableDepositor system, Particle part)
        {
            foreach (BlackHole blackH in system.blackHoles)
            {
                BlackHoleSettings settings = (blackH.Settings_Allowed) ? blackH.Settings : system.table.Settings.blackHoleSettings;

                // vypocet vzdalenosti castice od kamene s druhou mocninou
                double length = Math.Sqrt((part.Position.X - blackH.Position.X) * (part.Position.X - blackH.Position.X) +
                                    (part.Position.Y - blackH.Position.Y) * (part.Position.Y - blackH.Position.Y));

                // pokud je castice dostatecne blizko, bude se mazat
                if (length < blackH.Radius)
                {
                    if (PhysicsSettings.absorptionMode == AbsorptionMode.BLACKHOLE)
                    {
                        deletedParticles.Add(part);
                    }
                    else if (PhysicsSettings.absorptionMode == AbsorptionMode.RECYCLE)
                    {
                        // v recyklacnim modu najdu nejaky nahodny generator a vratim mu castici...
                        if (system.generators.Count > 0)
                        {
                            // vyberu generator, nastavim castici polohu primo na nej a jeste ji upravim vektor rychlosti
                            system.generators.ElementAt(CommonAttribService.apiRandom.Next(system.generators.Count - 1)).generatingNumber--;
                            deletedParticles.Add(part);
                        }
                    }
                    else if (PhysicsSettings.absorptionMode == AbsorptionMode.SELECT)
                    {
                        // hodime si korunou
                        if (CommonAttribService.apiRandom.Next(50) == 40) deletedParticles.Add(part);
                    }
                    return;
                }

                // gravitacni zrychleni
                double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * settings.weigh) / (Math.Log(length * length) / 114 + 14) * (((A_Rock)blackH).Intensity / 100);
                // vzdalenost mezi cernou dirou a castici
                double dist_x = (blackH.Position.X - part.Position.X);
                double dist_y = (blackH.Position.Y - part.Position.Y);
                double celk = length * 1.5;

                // pulsar
                if (settings.Energy_pulsing)
                {
                    acc /= (1 + Math.Tan(counter * settings.Energy_pulse_speed) / (4+4*Math.Log10(length)));
                }

                part.Vector_Acceleration.Add(0.5*acc * ((dist_x) / length),
                    0.5* acc * ((dist_y) / length));

                // je to trochu divne, ale funguje to
                if (dist_x > 0 && (-dist_y) > 0) dist_y *= length/4;
                else if (dist_x > 0 && (-dist_y) < 0) dist_x *= length/4;
                else if (dist_x < 0 && (-dist_y) > 0) dist_x*= length/4;
                else if (dist_x < 0 && (-dist_y) < 0) dist_y *= length/4;
               
                // pricteni gravitacniho zrychleni k castici
                part.Vector_Acceleration.Add((-dist_y/celk)/30, (dist_x/celk)/30);
            }
        }


        /// <summary>
        /// Zpracuje gravitony pro jednu castici
        /// </summary>
        /// <param name="system"></param>
        /// <param name="part"></param>
        public static void ProcessGravitons(TableDepositor system, Particle part)
        {
            temp.X = 0;
            temp.Y = 0;

            //projdu vsechny gravitacni objekty a provedu interakci s casticemi
            foreach (Graviton ig in system.gravitons)
            {
                GravitonSettings settings = (ig.Settings_Allowed) ? ig.Settings : system.table.Settings.gravitonSettings;

                // vypocet vzdalenosti castice od kamene s druhou mocninou
                double length = (part.Position.X - ig.Position.X) * (part.Position.X - ig.Position.X) +
                                    (part.Position.Y - ig.Position.Y) * (part.Position.Y - ig.Position.Y);

                    //=================================================
                    // GRAVITACNI ZRYCHLENI JE ZDE ODVOZENE JAKO
                    // g = KONST*M/(polomer/11 + 14), pro kamen hmotnosti
                    // 20 je nejvetsi mozna gravitace 10 a nejmensi cca 0.1
                    //=================================================

                    // skalarni zrychleni, upravy na interval min-max
                double acc = (PhysicSettings.Instance().DEFAULT_GRAVITY_CONSTANT * ig.Settings.weigh) / (length / 114 + 14) * (((A_Rock)ig).Intensity / 100);
                    double sqrt_length = Math.Sqrt(length);
                    if (sqrt_length < 20)
                    {
                        acc *= -Math.Log(sqrt_length - Math.Min(20,sqrt_length-2));
                    }
                    // pulsar
                    if (settings.Energy_pulsing)
                    {
                        acc /= (1+Math.Tan(settings.Energy_pulse_speed)/10);
                        
                    }


                    // prepocet zrychleni na vektory a pridani slozky "ovlivneni"
                if(PhysicsSettings.gravitationMode == GravitationMode.ADITIVE){
                    temp.Add(acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length)),
                        acc * ((ig.Position.Y - part.Position.Y) / Math.Sqrt(length)));       
                }else if(PhysicsSettings.gravitationMode == GravitationMode.AVERAGE){
                    // tento mod funguje podobne jako scitani paralelnich rezistoru
                    temp.Add(1 / (acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length))),
                        1/(acc * ((ig.Position.Y - part.Position.Y) / Math.Sqrt(length))));       
                }else if (PhysicsSettings.gravitationMode == GravitationMode.MULTIPLY){
                    temp.Add(acc * ((ig.Position.X - part.Position.X) / Math.Log(length)),
                        acc * ((ig.Position.Y - part.Position.Y) / Math.Log(length)));       
                }
            
            }

            if (PhysicsSettings.gravitationMode == GravitationMode.AVERAGE) temp.Invert();

            part.Vector_Acceleration.Add(temp);
        }

        /// <summary>
        /// Zpracuje magnetony pro jednu castici
        /// </summary>
        /// <param name="system"></param>
        /// <param name="part"></param>
        public static void ProcessMagnetons(TableDepositor system, Particle part)
        {
            temp.X = 0;
            temp.Y = 0;

            // projdu vsechny magneticke objekty (prozatim to jsou vsechny castice) a provedu interakci
            foreach (Magneton ig in system.magnetons)
            {
                MagnetonSettings settings = (ig.Settings_Allowed) ? ig.Settings : system.table.Settings.magnetonSettings;

                   // vypocet vzdalenosti castice od kamene s druhou mocninou
                double length = (part.Position.X - ig.Position.X) * (part.Position.X - ig.Position.X) +
                                    (part.Position.Y - ig.Position.Y) * (part.Position.Y - ig.Position.Y);



                    // skalarni zrychleni, upravy na interval min-max
                double acc = (PhysicSettings.Instance().DEFAULT_MAGNETON_CONSTANT * ig.Settings.force) / (length / 114 + 14) * (((A_Rock)ig).Intensity / 100);


                    // pulsar
                    if (settings.Energy_pulsing)
                    {
                        acc /= (1 + Math.Tan(counter * settings.Energy_pulse_speed) / 10);
                    }


                          // prepocet zrychleni na vektory a pridani slozky "ovlivneni"
                if(PhysicsSettings.magnetismMode == MagnetismMode.ADITIVE){
                    temp.Add(-acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length)),
                        -acc *((ig.Position.Y - part.Position.Y) / Math.Sqrt(length)));    
                }else if(PhysicsSettings.magnetismMode == MagnetismMode.AVERAGE){
                    // tento mod funguje podobne jako scitani paralelnich rezistoru
                    temp.Add(1 / (-acc * ((ig.Position.X - part.Position.X) / Math.Sqrt(length))),
                        1/(-acc *((ig.Position.Y - part.Position.Y) / Math.Sqrt(length)))); 
                }else if (PhysicsSettings.magnetismMode == MagnetismMode.MULTIPLY){
                    temp.Add(-acc * ((ig.Position.X - part.Position.X) / Math.Log(length)),
                        -acc * ((ig.Position.Y - part.Position.Y) / Math.Log(length)));     
                }
            
            }

            if (PhysicsSettings.magnetismMode == MagnetismMode.AVERAGE) temp.Invert();
            part.Vector_Acceleration.Add(temp);
        }
    }
}
