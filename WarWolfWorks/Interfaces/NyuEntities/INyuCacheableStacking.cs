using System;
using System.Collections.Generic;
using System.Text;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// A <see cref="INyuStacking"/> which uses a cache system for it's calculations.
    /// </summary>
    public interface INyuCacheableStacking : INyuStacking
    {
        /// <summary>
        /// Returns the cache.
        /// </summary>
        IReadOnlyDictionary<INyuStat, float> Cached { get; }

        /// <summary>
        /// Removes a specified stat's cache so it can be re-calcuated. Should be called when a property stat's value(s) are changed.
        /// </summary>
        /// <param name="stat"></param>
        void RefreshStatCache(INyuStat stat);

        /// <summary>
        /// Clears the cache of all previously calculated stats.
        /// </summary>
        void ClearStatsCache();
    }
}
