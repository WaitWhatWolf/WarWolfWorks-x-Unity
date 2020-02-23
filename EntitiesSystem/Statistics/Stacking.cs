using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem.Statistics
{
    /// <summary>
    /// Base class to use to apply to a <see cref="Stats.Stacking"/>.
    /// </summary>
    public abstract class Stacking : ScriptableObject, IStacking
    {
        /// <summary>
        /// All stats of the parent <see cref="Stats"/> class.
        /// </summary>
        protected List<IStat> AllStats => ((IStacking)this).Parent.すべてスタット;
        Stats IStacking.Parent { get; set; }
        /// <summary>
        /// What will be used inside <see cref="Stats"/>.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public abstract float CalculatedValue(IStat stat);
        internal void SetParent(Stats parent)
        {
            ((IStacking)this).Parent = parent;
        }
        /// <summary>
        /// Gets all <see cref="IStat"/> with the given affection from <see cref="AllStats"/>.
        /// </summary>
        /// <param name="affection"></param>
        /// <returns></returns>
        [Obsolete("Use GetStatsByAffection(int, int[]) instead.", true)]
        public IStat[] GetStatsByAffection(int affection) => GetStatsByAffection(AllStats, affection);
        /// <summary>
        /// Gets all <see cref="IStat"/> with the given affection from a custom list.
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="affection"></param>
        /// <returns></returns>
        [Obsolete("Use GetStatsByAffection(IEnumerable<IStat>, int, int[]) instead.", true)]
        public static IStat[] GetStatsByAffection(IEnumerable<IStat> stats, int affection)
        {
            throw new MissingMethodException();
            /*List<IStat> toReturn = new List<IStat>();
            foreach (IStat s in stats)
            {
                if (s.Affection == affection || s.Affections.Contains(affection)) toReturn.Add(s);
            }

            return toReturn.ToArray();*/
        }
        /// <summary>
        /// Gets all <see cref="IStat"/> with the given affections from <see cref="AllStats"/>.
        /// </summary>
        /// <param name="affections"></param>
        /// <returns></returns>
        public IStat[] GetStatsByAffections(int[] affections) => GetStatsByAffections(AllStats, affections);

        /// <summary>
        /// Gets all <see cref="IStat"/> with the given affections from a custom list.
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="affections"></param>
        /// <returns></returns>
        public static IStat[] GetStatsByAffections(IEnumerable<IStat> stats, int[] affections)
        {
            List<IStat> toReturn = new List<IStat>();
            foreach (IStat s in stats)
            {
                if (s.Affections.Intersect(affections).Any()) toReturn.Add(s);
            }

            return toReturn.ToArray();
        }
        /// <summary>
        /// Gets all <see cref="IStat"/> with the given stacking from <see cref="AllStats"/>.
        /// </summary>
        /// <param name="stacking"></param>
        /// <returns></returns>
        public float[] GetStatValuesByStacking(int stacking) => GetStatValuesByStacking(AllStats, stacking);
        /// <summary>
        /// Gets all <see cref="IStat"/> with the given stacking from a custom list.
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="stacking"></param>
        /// <returns></returns>
        public static float[] GetStatValuesByStacking(IEnumerable<IStat> stats, int stacking)
        {
            List<float> toReturn = new List<float>();
            foreach (IStat s in stats)
            {
                if (s.Stacking == stacking) toReturn.Add(s.Value);
            }

            return toReturn.ToArray();
        }
    }
}