using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.Statistics
{
    /// <summary>
    /// The class handling all statistics of an entity.
    /// </summary>
    [System.Serializable]
    public sealed class Stats : INyuReferencable
    {
        /// <summary>
        /// The parent entity.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

        /// <summary>
        /// Pointer to <see cref="Stacking.CalculatedValue(INyuStat)"/>.
        /// </summary>
        /// <param name="BaseStat"></param>
        /// <returns></returns>
        public float CalculatedValue(INyuStat BaseStat)
            => Stacking.CalculatedValue(BaseStat);

        /// <summary>
        /// Creates a temporary stat and uses it to retrieve the calculated value (Temporary Stat has a default stacking of 0).
        /// </summary>
        /// <param name="original"></param>
        /// <param name="affections"></param>
        /// <returns></returns>
        public float CalculatedValue(float original, int[] affections)
        {
            Stat toUse = new Stat(original, 0, affections);
                return CalculatedValue(toUse);
        }

        [UnityEngine.SerializeField]
        private Stacking stackingUsed;
        /// <summary>
        /// The object which calculates all stats to return a final value.
        /// </summary>
        public INyuStacking Stacking
        {
            get => stackingUsed;
            set
            {
                if (value is Stacking asStacking)
                {
                    stackingUsed = asStacking;
                    asStacking.SetParent(this);
                }
            }
        }

        /// <summary>
        /// Invoked when a stat is added.
        /// </summary>
        public event Action<INyuStat> OnStatAdded;

        /// <summary>
        /// Invoked when a stat is removed.
        /// </summary>
        public event Action<INyuStat> OnStatRemoved;
        internal List<INyuStat> ns_Stats = new List<INyuStat>();

        /// <summary>
        /// Gets all stats returned in an array.
        /// </summary>
        /// <returns></returns>
        public INyuStat[] GetAllStats() => ns_Stats.ToArray();
        /// <summary>
        /// Adds an <see cref="INyuStat"/> to stats to be calculated.
        /// </summary>
        /// <param name="toAdd"></param>
        public void AddStat(INyuStat toAdd)
        {
            ns_Stats.Add(toAdd);
            toAdd.OnAdded(this);
            OnStatAdded?.Invoke(toAdd);
        }
        /// <summary>
        /// Removes the given <see cref="INyuStat"/> from stats.
        /// </summary>
        /// <param name="toRemove"></param>
        public void RemoveStat(INyuStat toRemove)
        {
            ns_Stats.Remove(toRemove);
            OnStatRemoved?.Invoke(toRemove);
        }
        /// <summary>
        /// Adds a range of stats to calculated stats.
        /// </summary>
        /// <param name="stats"></param>
        public void AddStats(IEnumerable<INyuStat> stats)
        {
            ns_Stats.AddRange(stats);

            if (OnStatAdded == null)
                return;

            foreach (INyuStat @is in stats)
            {
                @is.OnAdded(this);
                OnStatAdded.Invoke(@is);
            }
        }
        /// <summary>
        /// Removes a range of stats from calculated stats.
        /// </summary>
        /// <param name="stats"></param>
        public void RemoveStats(IEnumerable<INyuStat> stats)
        {
            ns_Stats = ns_Stats.Except(stats).ToList();

            if (OnStatRemoved == null)
                return;

            foreach (INyuStat @is in stats)
            {
                OnStatRemoved.Invoke(@is);
            }
        }

        /// <summary>
        /// Returns true if the given <see cref="INyuStat"/> was inside the calculated stats list.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public bool Contains(INyuStat stat) => ns_Stats.Contains(stat);

        internal Stats(Nyu entity)
        {
            NyuMain = entity;
            if (Stacking == null) Stacking = ScriptableObject.CreateInstance<WWWStacking>();
            Stacking.SetParent(this);
        }
    }
}
