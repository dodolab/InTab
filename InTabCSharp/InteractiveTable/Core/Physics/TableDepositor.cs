using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using InteractiveTable.Core.Physics.System;

namespace InteractiveTable.Core.Data.Deposit
{
    /// <summary>
    /// Trida uchovavajici data pro virtualni stul, obsahuje
    /// konkretni virtualni objekty
    /// </summary>
    [Serializable]
    public class TableDepositor
    {
        /// <summary>
        /// Stul
        /// </summary>
        public Table table;
        /// <summary>
        /// Seznam gravitonu
        /// </summary>
        public HashSet<Graviton> gravitons = new HashSet<Graviton>();
        /// <summary>
        /// Seznam magnetonu
        /// </summary>
        public HashSet<Magneton> magnetons = new HashSet<Magneton>();
        /// <summary>
        /// Seznam castic
        /// </summary>
        public HashSet<Particle> particles = new HashSet<Particle>();
        /// <summary>
        /// Seznam generatoru
        /// </summary>
        public HashSet<Generator> generators = new HashSet<Generator>();
        /// <summary>
        /// Seznam cernych der
        /// </summary>
        public HashSet<BlackHole> blackHoles = new HashSet<BlackHole>();


        /// <summary>
        /// Vrati vsechny kameny
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
        /// Smaze vsechny objekty
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
