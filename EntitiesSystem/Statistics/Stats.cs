using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Statistics
{
    /// <summary>
    /// Class used by entities to get Stat values.
    /// </summary>
    [System.Serializable]
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public sealed class Stats : IEntity
    {
        /// <summary>
        /// Pointer to <see cref="Stacking.CalculatedValue(IStat)"/>.
        /// </summary>
        /// <param name="BaseStat"></param>
        /// <returns></returns>
        public float CalculatedValue(IStat BaseStat)
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
        public IStacking Stacking
        {
            get => stackingUsed;
            set
            {
                if (value is Stacking)
                {
                    stackingUsed = (Stacking)value;
                }
            }
        }

        /// <summary>
        /// Invoked when a stat is added.
        /// </summary>
        public event System.Action<IStat> OnStatAdded;

        /// <summary>
        /// Invoked when a stat is removed.
        /// </summary>
        public event System.Action<IStat> OnStatRemoved;
        internal List<IStat> すべてスタット { get; set; } = new List<IStat>();

        /// <summary>
        /// Entity of which these stats belong to.
        /// </summary>
        public Entity EntityMain { get; private set; }

        /// <summary>
        /// Gets all stats returned in an array.
        /// </summary>
        /// <returns></returns>
        public IStat[] GetAllStats() => すべてスタット.ToArray();
        /// <summary>
        /// Adds an <see cref="IStat"/> to stats to be calculated.
        /// </summary>
        /// <param name="toAdd"></param>
        public void AddStat(IStat toAdd)
        {
            すべてスタット.Add(toAdd);
            toAdd.OnAdded(this);
            OnStatAdded?.Invoke(toAdd);
        }
        /// <summary>
        /// Removes the given <see cref="IStat"/> from stats.
        /// </summary>
        /// <param name="toRemove"></param>
        public void RemoveStat(IStat toRemove)
        {
            すべてスタット.Remove(toRemove);
            OnStatRemoved?.Invoke(toRemove);
        }
        /// <summary>
        /// Adds a range of stats to calculated stats.
        /// </summary>
        /// <param name="stats"></param>
        public void AddStats(IEnumerable<IStat> stats)
        {
            すべてスタット.AddRange(stats);

            if (OnStatAdded == null)
                return;

            foreach (IStat @is in stats)
            {
                @is.OnAdded(this);
                OnStatAdded.Invoke(@is);
            }
        }
        /// <summary>
        /// Removes a range of stats from calculated stats.
        /// </summary>
        /// <param name="stats"></param>
        public void RemoveStats(IEnumerable<IStat> stats)
        {
            すべてスタット = すべてスタット.Except(stats).ToList();

            if (OnStatRemoved == null)
                return;

            foreach (IStat @is in stats)
            {
                OnStatRemoved.Invoke(@is);
            }
        }

        /// <summary>
        /// Returns true if the given <see cref="IStat"/> was inside the calculated stats list.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public bool Contains(IStat stat) => すべてスタット.Contains(stat);

        internal void Initiate(Entity parent)
        {
            EntityMain = parent;
            if (Stacking == null) Stacking = ScriptableObject.CreateInstance<WWWStacking>();
            Stacking.Parent = this;
        }
    }
}