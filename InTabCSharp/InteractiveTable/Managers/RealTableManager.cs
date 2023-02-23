using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTable.Core.Data.Deposit;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Data.Capture;
using System.Windows.Forms;

namespace InteractiveTable.Managers
{

    /// <summary>
    /// Manager that merges table system between two time steps
    /// </summary>
    public class RealTableManager
    {
        /// <summary>
        /// List of stones on the table, sorted by contour types
        /// </summary>
        private IDictionary<String, HashSet<ContourRock>> rocks = new Dictionary<String, HashSet<ContourRock>>();

        private TableDepositor system; 


        public RealTableManager(TableDepositor system)
        {
            this.system = system;
        }

        public void Init(Templates templates)
        {
            rocks.Clear();

            if (templates.Count == 0) MessageBox.Show("You don't have configured any templates!'");

            foreach (Template tmp in templates)
            {
                rocks.Add(tmp.name, new HashSet<ContourRock>());
            }
        }

        /// <summary>
        /// Merges the system between two time steps
        /// </summary>
        /// <param name="foundRocks"></param>
        public void MergeSystems(HashSet<FoundRock> foundRocks)
        {
            lock (system)
            {
                foreach (String key in rocks.Keys)
                {
                    MergeRocks(SeparateRocks(rocks[key], key), SeparateRocks(foundRocks, key), key);
                }
            }
        }

        /// <summary>
        /// Separates new detected stones by the type of their contours
        /// </summary>
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
        /// Separates already placed stones by the type of their contours
        /// </summary>
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
        /// Merges one group of stones for one type of contours
        /// </summary>
        /// <param name="rocks">already placed stones</param>
        /// <param name="founds">detected stones</param>
        private void MergeRocks(HashSet<ContourRock> rocks, HashSet<FoundRock> founds, String contourName)
        {
            SolveIntensity(rocks);

            foreach (ContourRock rck in rocks)
            {
                if (!rck.isMissing) SetMinimumLengthForPlacedRock(rck, founds);
                if (rck.minLength == 100000) rck.isMissing = true;

                // rock detected but it's too far
                if (rck.minLength > 10) rck.isMissing = true;
            }

            HashSet<FoundRock> rockToAdd = new HashSet<FoundRock>();
            
            // for each detected stone, try to find the nearest already placed
            foreach (FoundRock rck in founds)
            {
                SetMinimumLengthForFoundRock(rck, rocks);
                // if there is no stone nearby, this one will likely be a new one
                if (rck.minLength == 100000 ||
                    rck.minLength > 10)
                {
                    rockToAdd.Add(rck);
                }
            }

            SolveTranslation(rocks, rockToAdd);
            RefreshRocks(rocks);

            foreach (FoundRock rock in rockToAdd)
            {
                CreateNewRock(rock);
            }
            
        }

        /// <summary>
        /// Resolves translation-issue -> an already added stone and a removed one can be the same but translated a bit
        /// </summary>
        private void SolveTranslation(HashSet<ContourRock> rocks,  HashSet<FoundRock> rocksToAdd)
        {
            HashSet<FoundRock> rockToRemove = new HashSet<FoundRock>();

            foreach (ContourRock rck in rocks)
            {

                if (rck.isMissing)
                {
                    foreach (FoundRock fnd in rocksToAdd)
                    {
                        // if the stone went missing and there is one stone in the collection of stones
                        // that will be added to the system during this time-step, having a similar size and the 
                        // same type, it will likely be the same stone
                        int delta = (int)(rck.rock.Radius - fnd.radius);
                        if (Math.Abs(delta) < 5)
                        {
                            rockToRemove.Add(fnd);
                            rck.isMissing = false;
                            // translate the original stone
                            rck.foundRock = fnd;
                            break;
                        }
                    }
                }
            }
            
            foreach (FoundRock rck in rockToRemove)
            {
                rocksToAdd.Remove(rck);
            }
        }

        /// <summary>
        /// Refreshes all attributes based on detected stones
        /// </summary>
        private void RefreshRocks(HashSet<ContourRock> rocks)
        {
            foreach (ContourRock rck in rocks)
            {
                if (rck.foundRock != null && rck.rock.Shape != null && !rck.isMissing)
                {
                    rck.rock.Position = rck.foundRock.position;
                    rck.rock.Angle = rck.foundRock.angle;
                    rck.rock.Scale = rck.foundRock.scale;
                    rck.rock.Radius = rck.foundRock.radius;
                }
            }
        }

        /// <summary>
        /// Creates a new stone and adds it to the collection of stones
        /// </summary>
        /// <param name="rock"></param>
        private void CreateNewRock(FoundRock rock)
        {
            A_Rock newRock = null;

            if (rock.type == FoundRockType.BLACKHOLE)
            {
                newRock = new BlackHole();
                ((BlackHole)newRock).BaseSettings = system.table.Settings.blackHoleSettings;
                ((BlackHole)newRock).Settings_Allowed = ((BlackHole)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((BlackHole)newRock).Settings = ((BlackHole)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((BlackHole)newRock).Position.X = rock.position.X;
                ((BlackHole)newRock).Position.Y = rock.position.Y;
            }
            if (rock.type == FoundRockType.GENERATOR)
            {
                newRock = new Generator();
                ((Generator)newRock).BaseSettings = system.table.Settings.generatorSettings;
                ((Generator)newRock).Settings_Allowed = ((Generator)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((Generator)newRock).Settings = ((Generator)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((Generator)newRock).Position.X = rock.position.X;
                ((Generator)newRock).Position.Y = rock.position.Y;
            }
            if (rock.type == FoundRockType.GRAVITON)
            {
                newRock = new Graviton();
                ((Graviton)newRock).BaseSettings = system.table.Settings.gravitonSettings;
                ((Graviton)newRock).Settings_Allowed = ((Graviton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((Graviton)newRock).Settings = ((Graviton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((Graviton)newRock).Position.X = rock.position.X;
                ((Graviton)newRock).Position.Y = rock.position.Y;
            }
            if (rock.type == FoundRockType.MAGNETON)
            {
                newRock = new Magneton();
                ((Magneton)newRock).BaseSettings = system.table.Settings.magnetonSettings;
                ((Magneton)newRock).Settings_Allowed = ((Magneton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings_Allowed;
                ((Magneton)newRock).Settings = ((Magneton)CommonAttribService.DEFAULT_TEMPLATES.rockSettings.Where(mrck => mrck.contour_name.Equals(rock.contour_name)).First().rock).Settings;
                ((Magneton)newRock).Position.X = rock.position.X;
                ((Magneton)newRock).Position.Y = rock.position.Y;
            }
            
            // initial intensity is 5 in order to have a smooth fade-in
            newRock.Intensity = 5;
            newRock.Name = rock.contour_name;
            system.InsertRock(newRock);
            rocks[rock.contour_name].Add(new ContourRock(newRock, rock.contour_name));
        }



        /// <summary>
        /// Sets a minimal distance between a freshly inserted stone and all others
        /// </summary>
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
        /// Sets a minimal distance between a newly detected stone and al others
        /// </summary>
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
        /// Calculates a distance between two stones
        /// </summary>
        /// <returns></returns>
        private int DistanceBetweenRocks(ContourRock rck, FoundRock fnd)
        {
            double deltaX = rck.rock.Position.X - fnd.position.X;
            double deltaY = rck.rock.Position.Y - fnd.position.Y;

            if (deltaX == 0 && deltaY == 0) return 0;
            return (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        /// <summary>
        /// Removes all stones that have intensity of zero
        /// </summary>
        private void SolveIntensity(HashSet<ContourRock> rocks)
        {
            HashSet<ContourRock> rockToDelete = new HashSet<ContourRock>();

            foreach(ContourRock rck in rocks){

                rck.ImproveIntensity();    
                if (rck.rock.Intensity <= 0)
                {
                    rockToDelete.Add(rck);
                }
            }

            foreach (ContourRock rck in rockToDelete)
            {
                rocks.Remove(rck);
                this.rocks[rck.contour_name].Remove(rck);
                system.RemoveRock(rck.rock);
            }
        }
    }
}
