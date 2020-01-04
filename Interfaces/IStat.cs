using System;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Base interface used for Stat calculations.
    /// </summary>
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
    }
}
