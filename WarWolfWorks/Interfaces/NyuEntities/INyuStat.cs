using WarWolfWorks.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Interface for implementing a stat to be used with an <see cref="Nyu"/>.
    /// </summary>
    public interface INyuStat
    {
        // <summary>
        // Tells a <see cref="INyuStacking"/> if this stat should be optimized.
        // </summary>
        //bool Optimize { get; set; }

        /// <summary>
        /// <see cref="float"/> Value returned by this stat.
        /// </summary>
        float Value { get; set; }
        /// <summary>
        /// Which other stats will it interact with.
        /// </summary>
        int[] Affections { get; set; }
        /// <summary>
        /// How the value is calculated by a <see cref="INyuStacking"/>.
        /// </summary>
        int Stacking { get; set; }
        /// <summary>
        /// Invoked when this <see cref="INyuStat"/> is added to <see cref="Stats"/>.
        /// </summary>
        /// <param name="to"></param>
        void OnAdded(Stats to);
        /// <summary>
        /// Invoked when this <see cref="INyuStat"/> is removed from <see cref="Stats"/>.
        /// </summary>
        /// <param name="to"></param>
        void OnRemoved(Stats to);
    }
}
