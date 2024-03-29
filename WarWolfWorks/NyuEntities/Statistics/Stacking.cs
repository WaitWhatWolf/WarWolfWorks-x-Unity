﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.Statistics
{
    /// <summary>
    /// Base class to use to apply to a <see cref="Stats.Stacking"/>.
    /// </summary>
    public abstract class Stacking : INyuStacking
    {
        /// <summary>
        /// The parent stats to be handled.
        /// </summary>
        public Stats Parent { get; }

        /// <summary>
        /// What will be used inside <see cref="Stats"/>.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public abstract float CalculatedValue(INyuStat stat);
        
        /// <summary>
        /// Gets all <see cref="INyuStat"/> with the given affections from <see cref="AllStats"/>.
        /// </summary>
        /// <param name="affections"></param>
        /// <returns></returns>
        public INyuStat[] GetStatsByAffections(int[] affections) => GetStatsByAffections(AllStats, affections);

        /// <summary>
        /// Gets all <see cref="INyuStat"/> with the given affections from a custom list.
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="affections"></param>
        /// <returns></returns>
        public static INyuStat[] GetStatsByAffections(IEnumerable<INyuStat> stats, int[] affections)
        {
            List<INyuStat> toReturn = new List<INyuStat>();
            foreach (INyuStat s in stats)
            {
                if (s.Affections.Intersect(affections).Any()) toReturn.Add(s);
            }

            return toReturn.ToArray();
        }
        /// <summary>
        /// Gets all <see cref="INyuStat"/> with the given stacking from <see cref="AllStats"/>.
        /// </summary>
        /// <param name="stacking"></param>
        /// <returns></returns>
        public float[] GetStatValuesByStacking(int stacking) => GetStatValuesByStacking(AllStats, stacking);
        /// <summary>
        /// Gets all <see cref="INyuStat"/> with the given stacking from a custom list.
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="stacking"></param>
        /// <returns></returns>
        public static float[] GetStatValuesByStacking(IEnumerable<INyuStat> stats, int stacking)
        {
            List<float> toReturn = new List<float>();
            foreach (INyuStat s in stats)
            {
                if (s.Stacking == stacking) toReturn.Add(s.Value);
            }

            return toReturn.ToArray();
        }

        /// <summary>
        /// Base constructor of the stacking class.
        /// </summary>
        /// <param name="parent"></param>
        public Stacking(Stats parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// All stats of the parent <see cref="Stats"/> class.
        /// </summary>
        protected HashSet<INyuStat> AllStats => Parent.pv_Stats;
    }
}
