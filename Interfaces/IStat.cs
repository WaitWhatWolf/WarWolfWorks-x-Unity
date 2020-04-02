using System;
using WarWolfWorks.EntitiesSystem.Statistics;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Base interface used for Stat calculations.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public interface IStat : IEquatable<float>
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
        /// Invoked when added to <see cref="Stats"/> with <see cref="Stats.AddStat(IStat)"/>.
        /// </summary>
        /// <param name="to"></param>
        void OnAdded(Stats to);
    }
}
