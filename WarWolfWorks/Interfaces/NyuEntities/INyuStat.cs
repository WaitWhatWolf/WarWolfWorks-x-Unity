using WarWolfWorks.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Interface for implementing a stat to be used with an <see cref="Nyu"/>.
    /// </summary>
    public interface INyuStat
    {
        /// <summary>
        /// <see cref="float"/> Value returned by this stat.
        /// </summary>
        float Value { get; }
        /// <summary>
        /// Used to set the base value of the Stat.
        /// </summary>
        float SetValue { set; }
        /// <summary>
        /// Which other stats will it interact with.
        /// </summary>
        int[] Affections { get; set; }
        /// <summary>
        /// How the value should be calculated.
        /// </summary>
        int Stacking { get; set; }
        /// <summary>
        /// Invoked when added to <see cref="Stats"/> with <see cref="Stats.AddStat(INyuStat)"/>.
        /// </summary>
        /// <param name="to"></param>
        void OnAdded(Stats to);
    }
}
