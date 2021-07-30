using System;
using System.Collections.Generic;
using System.Linq;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.NyuEntities.Statistics
{
    /// <summary>
    /// Default stacking which is used if <see cref="Stats.Stacking"/> is not set.
    /// </summary>
    public sealed class DefaultStacking : Stacking, INyuCacheableStacking
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IReadOnlyDictionary<INyuStat, float> Cached => pv_Results;

        /// <summary>
        /// Returns the calculated value. See <see cref="DefaultStacking"/>'s constant values located in <see cref="WWWResources"/> for more info.
        /// </summary>
        /// <param name="BaseStat"></param>
        /// <returns></returns>
        public override float CalculatedValue(INyuStat BaseStat)
        {
            if(pv_Used.Contains(BaseStat) && pv_Results.TryGetValue(BaseStat, out float resultVal))
            {
                return resultVal;
            }

            float toReturn = BaseStat.Value;

            INyuStat[] usableStats = GetStatsByAffections(BaseStat.Affections);

            foreach (INyuStat overriderCandidate in usableStats)
            {
                if (overriderCandidate.Stacking == NYU_STATS_STACKING_PWNER)
                    return overriderCandidate.Value;
                else if (overriderCandidate.Stacking == NYU_STATS_STACKING_OVERRIDER)
                {
                    toReturn = overriderCandidate.Value;
                    break;
                }
            }

            float adder = GetStatValuesByStacking(usableStats, NYU_STATS_STACKING_ADDITIVE).Sum();
            float multiplier = GetStatValuesByStacking(usableStats, NYU_STATS_STACKING_BASEMULT).Sum() + 1f;

            toReturn += adder;
            toReturn *= multiplier;

            foreach (float f in GetStatValuesByStacking(usableStats, NYU_STATS_STACKING_TOTALMULT))
            {
                toReturn *= (f + 1f);
            }

            pv_Used.Add(BaseStat);
            pv_Results.Add(BaseStat, toReturn);

            return toReturn;
        }

        /// <summary>
        /// Refreshes an existing stat, effectively removing it's cache. Should be called when a property stat's
        /// value(s) have been changed.
        /// Note: Does not need to be called when a stat is added/removed, as this is taken care of automatically.
        /// </summary>
        /// <param name="stat"></param>
        public void RefreshStatCache(INyuStat stat)
        {
            pv_Used.Remove(stat);
            pv_Results.Remove(stat);
        }

        /// <summary>
        /// Clears the calculation cache of all stats previously passed through <see cref="CalculatedValue(INyuStat)"/>.
        /// </summary>
        public void ClearStatsCache()
        {
            pv_Used.Clear();
            pv_Results.Clear();
        }

        /// <summary>
        /// Base constructor of the default stacking.
        /// </summary>
        /// <param name="parent"></param>
        public DefaultStacking(Stats parent) : base(parent)
        {
            Parent.OnStatAdded += Event_OnStatsUpdated;
            Parent.OnStatRemoved += Event_OnStatsUpdated;
        }

        private void Event_OnStatsUpdated(INyuStat obj)
        {
            if(obj.Stacking != NYU_STATS_STACKING_BASE)
            {
                HashSet<INyuStat> toReset = new();
                foreach(INyuStat stat in pv_Used)
                {
                    //In a nutshell, it checks if a stat has been added which would affect any of the existing "calculated" stats,
                    //and removes it to recalculate it the next time a CalculatedValue is requested.
                    if(stat.Affections.Intersect(obj.Affections).Count() > 0)
                    {
                        toReset.Add(stat);
                    }
                }

                foreach(INyuStat stat in toReset)
                {
                    pv_Used.Remove(stat);
                    pv_Results.Remove(stat);
                }
            }
        }

        private readonly HashSet<INyuStat> pv_Used = new();
        private readonly Dictionary<INyuStat, float> pv_Results = new();
    }
}
