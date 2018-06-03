using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Accessories;
using InteractiveTable.Core.Data.Capture;
using System.Windows.Forms;

namespace InteractiveTable.Managers
{

    /// <summary>
    /// Manazer starajici se o mergovani soustav mezi iteracemi
    /// v ramci toho, co kamera zachytila
    /// </summary>
    public class RealTableManager
    {
        /// <summary>
        /// Slovnik kamenu; obsahuje kameny na stole, neradi je podle typu kamenu ale podle typu kontury
        /// </summary>
        private IDictionary<String, HashSet<ContourRock>> rocks = new Dictionary<String, HashSet<ContourRock>>();

        private TableDepositor system; // virtualni system


        public RealTableManager(TableDepositor system)
        {
            this.system = system;
        }

        // inicializuje slovniky
        public void Init(Templates templates)
        {
            rocks.Clear();

            if (templates.Count == 0) MessageBox.Show("Nemáte nastavené šablony!");

            foreach (Template tmp in templates)
            {
                rocks.Add(tmp.name, new HashSet<ContourRock>());
            }
        }

        /// <summary>
        /// Slouci virtualni system; dostane kolekci nalezenych kamenu
        /// a upravi system v TableDepositor tride
        /// </summary>
        /// <param name="foundRocks"></param>
        public void MergeSystems(HashSet<FoundRock> foundRocks)
        {
            lock (system)
            {
              //  Console.WriteLine("============================================================================Slucuji fyzikalni system");
                foreach (String key in rocks.Keys)
                {
                    MergeRocks(SeparateRocks(rocks[key], key), SeparateRocks(foundRocks, key), key);
                }
               // Console.WriteLine("===============================================================================System byl sloucen");
            }
        }

        /// <summary>
        /// Rozdelni vstupni mnozinu foundRocks podle typu kontury
        /// </summary>
        /// <param name="input"></param>
        /// <param name="contourName"></param>
        /// <returns></returns>
        private HashSet<FoundRock> SeparateRocks(HashSet<FoundRock> input, String contourName)
        {
            HashSet<FoundRock> output = new HashSet<FoundRock>();

            foreach (FoundRock rck in input)
            {
                if (rck.contour_name.Equals(contourName)) output.Add(rck);
            }

            return output;
        }


        /// <summary>
        /// Rozdelni vstupni mnozinu placedRocks podle typu kontury
        /// </summary>
        /// <param name="input"></param>
        /// <param name="contourName"></param>
        /// <returns></returns>
        private HashSet<ContourRock> SeparateRocks(HashSet<ContourRock> input, String contourName)
        {
            HashSet<ContourRock> output = new HashSet<ContourRock>();

            foreach (ContourRock rck in input)
            {
                if (rck.contour_name.Equals(contourName)) output.Add(rck);
            }

            return output;
        }

        /// <summary>
        /// Provede merge nad pouze jednou skupinou kamenu
        /// Provadi pouze pro jeden typ kontury
        /// </summary>
        /// <param name="rocks">jiz umistene kameny</param>
        /// <param name="founds">nalezene kameny</param>
        private void MergeRocks(HashSet<ContourRock> rocks, HashSet<FoundRock> founds, String contourName)
        {
          //  Console.WriteLine("===========Merge rock pro konturu "+contourName);
           // Console.WriteLine("@@@ Pocet jiz pridanych kamenu: "+rocks.Count);
           // Console.WriteLine("@@@ Pocet nalezenych kamenu: " + founds.Count);

            SolveIntensity(rocks);
            int rockSize = rocks.Count;
            int foundSize = founds.Count;

           // Console.WriteLine("--------- Hledam pary VLOZENY-NALEZENY: ");
            // pro kazdy nechybejici kamen nalezne nejblizsi nalezeny
            foreach (ContourRock rck in rocks)
            {
                if (!rck.isMissing) SetMinimumLengthForPlacedRock(rck, founds);
              //  Console.WriteLine("----- Pozice kamene: " + rck.rock.Position.X + ", " + rck.rock.Position.Y);
               // Console.WriteLine("----- Vypocitana vzdalenost je "+rck.minLength);
                // pokud se nepodarilo najit zadny nalezeny, tento kamen urcite chybi!
                if (rck.minLength == 100000) rck.isMissing = true;

                // byl nalezen sice sparovany kamen, je ale uz trochu daleko
                if (rck.minLength > 10) rck.isMissing = true;
            }

            HashSet<FoundRock> rockToAdd = new HashSet<FoundRock>();

           // Console.WriteLine("--------- Hledam pary NALEZENY-VLOZENY: ");
            // pro kazdy nalezeny kamen nalezne nejblizsi umisteny
            foreach (FoundRock rck in founds)
            {
                SetMinimumLengthForFoundRock(rck, rocks);
               // Console.WriteLine("----- Pozice kamene: " + rck.position.X + ", " + rck.position.Y);
                //Console.WriteLine("----- Vypocitana vzdalenost je " + rck.minLength);
                // pokud nebyl nalezen jiz pridany kamen,
                // ktery je dostatecne blizko, zcela jiste toto bude novy kamen
                if (rck.minLength == 100000 ||
                    rck.minLength > 10)
                {
                    //Console.WriteLine("Kamen bude pridan");
                    rockToAdd.Add(rck);
                }
            }
            //Console.WriteLine("--------- Ukonceno hledani paru ");

            SolveTranslation(rocks, rockToAdd);
            // refreshneme atributy jako pozice, natoceni atd.
            RefreshRocks(rocks);

            foreach (FoundRock rock in rockToAdd)
            {
                CreateNewRock(rock);
            }
            
        }

        /// <summary>
        /// Vyresi problem translace dvou kamenu - pridany kamen a ubrany kamen muze byt
        /// ve skutecnosti jeden a ten samy, akorat posunuty
        /// </summary>
        /// <param name="rocks"></param>
        /// <param name="rocksToAdd"></param>
        private void SolveTranslation(HashSet<ContourRock> rocks,  HashSet<FoundRock> rocksToAdd)
        {
          //  Console.WriteLine("######### Resim translaci ");
            HashSet<FoundRock> rockToRemove = new HashSet<FoundRock>();

            foreach (ContourRock rck in rocks)
            {

                if (rck.isMissing)
                {
                    foreach (FoundRock fnd in rocksToAdd)
                    {
                        // pokud byl kamen prohlasen za chybejici a mezi temi, ktere
                        // maji byt pridany, je takovy, u nehoz je velmi podobna velikost,
                        //jedna se o tentyz kamen
                        int delta = (int)(rck.rock.Radius - fnd.radius);
                        if (Math.Abs(delta) < 5)
                        {
                            //Console.WriteLine("#### Nalezena translace ");
                            rockToRemove.Add(fnd);
                            rck.isMissing = false;
                            // provedeme translaci puvodniho kamene
                            rck.foundRock = fnd;
                            break;
                        }
                    }
                }
            }

            // smazeme ty kameny, ktere se mely pridavat ale zjistili jsme, ze se jedna
            // jen o posun
            foreach (FoundRock rck in rockToRemove)
            {
                rocksToAdd.Remove(rck);
            }
        }

        /// <summary>
        /// Refresh vsech atributu podle nalezeneho kamene
        /// </summary>
        /// <param name="rocks"></param>
        private void RefreshRocks(HashSet<ContourRock> rocks)
        {
           // Console.WriteLine("__________Upravuji kamenum atributy ");
            foreach (ContourRock rck in rocks)
            {
                if (rck.foundRock != null && !rck.isMissing)
                {
                    rck.rock.Position = rck.foundRock.position;
                    rck.rock.Angle = rck.foundRock.angle;
                    rck.rock.Scale = rck.foundRock.scale;
                    rck.rock.Radius = rck.foundRock.radius;
                }
            }
        }

        /// <summary>
        /// Vytvori novy kamen a da jej k ostatnim
        /// </summary>
        /// <param name="rock"></param>
        private void CreateNewRock(FoundRock rock)
        {
           // Console.WriteLine("_______Vytvarim novy kamen- ");
            A_Rock newRock = null;
            // zde je treba rozlisovat typy kamene, protoze budeme
            // pridavat do ruznych hashsetu v systemu
            if (rock.type == FoundRockType.BLACKHOLE)
            {
                newRock = new BlackHole();
                ((BlackHole)newRock).BaseSettings = system.table.Settings.blackHoleSettings;
                ((BlackHole)newRock).Settings_Allowed = ((BlackHole)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((BlackHole)newRock).Settings = ((BlackHole)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((BlackHole)newRock).Position.X = rock.position.X;
                ((BlackHole)newRock).Position.Y = rock.position.Y;
               // system.blackHoles.Add((BlackHole)newRock);
            }
            if (rock.type == FoundRockType.GENERATOR)
            {
                newRock = new Generator();
                ((Generator)newRock).BaseSettings = system.table.Settings.generatorSettings;
                ((Generator)newRock).Settings_Allowed = ((Generator)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((Generator)newRock).Settings = ((Generator)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((Generator)newRock).Position.X = rock.position.X;
                ((Generator)newRock).Position.Y = rock.position.Y;
               // system.generators.Add((Generator)newRock);
            }
            if (rock.type == FoundRockType.GRAVITON)
            {
                newRock = new Graviton();
                ((Graviton)newRock).BaseSettings = system.table.Settings.gravitonSettings;
                ((Graviton)newRock).Settings_Allowed = ((Graviton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((Graviton)newRock).Settings = ((Graviton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((Graviton)newRock).Position.X = rock.position.X;
                ((Graviton)newRock).Position.Y = rock.position.Y;
               // system.gravitons.Add((Graviton)newRock);
            }
            if (rock.type == FoundRockType.MAGNETON)
            {
                newRock = new Magneton();
                ((Magneton)newRock).BaseSettings = system.table.Settings.magnetonSettings;
                ((Magneton)newRock).Settings_Allowed = ((Magneton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((Magneton)newRock).Settings = ((Magneton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((Magneton)newRock).Position.X = rock.position.X;
                ((Magneton)newRock).Position.Y = rock.position.Y;
               // system.magnetons.Add((Magneton)newRock);
            }

            // dulezite!! intenzita bude nastavena na 5, aby mela plynuly fade-in
            newRock.Intensity = 5;
            newRock.Name = rock.contour_name;
            system.InsertRock(newRock);
            rocks[rock.contour_name].Add(new ContourRock(newRock, rock.contour_name));
        }



        /// <summary>
        /// Nastavi minimalni vzdalenost pridaneho kamene se vsemi nalezenymi
        /// </summary>
        /// <param name="rocks"></param>
        /// <param name="founds"></param>
        private void SetMinimumLengthForPlacedRock(ContourRock rock, HashSet<FoundRock> founds)
        {
            rock.minLength = 100000;
            rock.foundRock = null;

            foreach (FoundRock found in founds)
            {
                int distance = DistanceBetweenRocks(rock, found);
                if (distance < rock.minLength)
                {
                    rock.minLength = distance;
                    rock.foundRock = found;
                }
            }
        }


        /// <summary>
        /// Nastavi minimalni vzdalenost nalezeneho kamene se vsemi jiz pridanymi
        /// </summary>
        /// <param name="found"></param>
        /// <param name="rocks"></param>
        private void SetMinimumLengthForFoundRock(FoundRock found, HashSet<ContourRock> rocks)
        {
            found.minLength = 100000;
            found.placedRock = null;

            foreach (ContourRock rock in rocks)
            {
                int distance = DistanceBetweenRocks(rock, found);
                if (distance < found.minLength)
                {
                    found.minLength = distance;
                    found.placedRock = rock;
                }
            }
        }

        /// <summary>
        /// Vrati vzdalenost mezi dvema kameny
        /// </summary>
        /// <param name="rck"></param>
        /// <param name="fnd"></param>
        /// <returns></returns>
        private int DistanceBetweenRocks(ContourRock rck, FoundRock fnd)
        {
            double deltaX = rck.rock.Position.X - fnd.position.X;
            double deltaY = rck.rock.Position.Y - fnd.position.Y;

            if (deltaX == 0 && deltaY == 0) return 0;
            return (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        /// <summary>
        /// Najde vsechny kameny, jejichz intenzita je je mensi
        /// nez nula a vymaze je
        /// </summary>
        private void SolveIntensity(HashSet<ContourRock> rocks)
        {
           // Console.WriteLine("......Resim Intenzitu kamenu.......");
            HashSet<ContourRock> rockToDelete = new HashSet<ContourRock>();

            foreach(ContourRock rck in rocks){

                rck.ImproveIntensity();
                
                if (rck.rock.Intensity <= 0)
                {
                   // Console.WriteLine("Mazu kamen!!!!");
                    rockToDelete.Add(rck);
                }
            }

            foreach (ContourRock rck in rockToDelete)
            {
                rocks.Remove(rck);
                this.rocks[rck.contour_name].Remove(rck);
                system.RemoveRock(rck.rock);
            }
           // Console.WriteLine("......Intenzita vyresena.......");
        }



    }


}
