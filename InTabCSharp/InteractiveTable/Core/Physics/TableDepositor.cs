using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Physics.System;

namespace InteractiveTable.Core.Data.Deposit
{
    /// <summary>
    /// Holds data for interactive table
    /// </summary>
    [Serializable]
    public class TableDepositor
    {
        public Table table;
        public HashSet<Graviton> gravitons = new HashSet<Graviton>();
        public HashSet<Magneton> magnetons = new HashSet<Magneton>();
        public HashSet<Particle> particles = new HashSet<Particle>();
        public HashSet<Generator> generators = new HashSet<Generator>();
        public HashSet<BlackHole> blackHoles = new HashSet<BlackHole>();


        /// <summary>
        /// Returns all stones
        /// </summary>
        /// <returns></returns>
        public List<A_Rock> GetAllRocks()
        {
            List<A_Rock> output = new List<A_Rock>();

            try
            {
                foreach (Graviton gr in gravitons) output.Add(gr);
                foreach (Magneton mg in magnetons) output.Add(mg);
                foreach (Generator gn in generators) output.Add(gn);
                foreach (BlackHole bh in blackHoles) output.Add(bh);
            }
            catch
            {
            }
                return output;
        }

        public void InsertRock(A_Rock rock)
        {
            if (rock is Graviton) gravitons.Add((Graviton)rock);
            if (rock is Magneton) magnetons.Add((Magneton)rock);
            if (rock is Generator) generators.Add((Generator)rock);
            if (rock is BlackHole) blackHoles.Add((BlackHole)rock);
        }

        public void RemoveRock(A_Rock rock)
        {
            if (rock is Graviton) gravitons.Remove((Graviton)rock);
            if (rock is Magneton) magnetons.Remove((Magneton)rock);
            if (rock is Generator) generators.Remove((Generator)rock);
            if (rock is BlackHole) blackHoles.Remove((BlackHole)rock);
        }

        public TableDepositor()
        {
            table = new Table();
        }

        /// <summary>
        /// Removes all stones
        /// </summary>
        public void ClearTable()
        {
            particles.Clear();
            gravitons.Clear();
            generators.Clear();
            magnetons.Clear();
            blackHoles.Clear();
        }

    }
}
