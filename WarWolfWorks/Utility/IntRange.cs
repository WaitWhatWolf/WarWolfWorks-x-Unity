using System;
using UnityEngine;
using static WarWolfWorks.Utility.Hooks;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Range struct for Clamping/Range utility in <see cref="Int32"/> value.
    /// </summary>
    [Serializable]
    public sealed record IntRange : IEquatable<IntRange>
    {
        /// <summary>
        /// The minimal value of this <see cref="IntRange"/>.
        /// </summary>
        public int Min;
        /// <summary>
        /// The maximal value of this <see cref="IntRange"/>.
        /// </summary>
        public int Max;

        /// <summary>
        /// Returns the value given clamped between <see cref="Min"/> (inclusive) and <see cref="Max"/> (exclusive).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetClampedValue(int value)
        {
            return MathF.Clamp(value, Min, Max - 1);
        }

        /// <summary>
        /// Returns true if the given value is within <see cref="Min"/> (inclusive) and <see cref="Max"/> (exclusive).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsWithinRange(int value)
        {
            return value >= Min && value < Max;
        }

        /// <summary>
        /// Returns a random value between <see cref="Min"/> (inclusive) and <see cref="Max"/> (exclusive).
        /// </summary>
        /// <returns></returns>
        public int GetRandom()
            => UnityEngine.Random.Range(Min, Max);

        /// <summary>
        /// Returns a <see cref="Tuple{T1, T2}"/> of <see cref="Min"/> and <see cref="Max"/>.
        /// </summary>
        /// <returns></returns>
        public (int min, int max) GetTupleRange() => (Min, Max);

        /// <summary>
        /// Returns true if Min and Max are of the exact same value for both IntRanges.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IntRange other)
        {
            return other.Min == Min && other.Max == Max;
        }

        /// <summary>
        /// Creates a new <see cref="IntRange"/>.
        /// </summary>
        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Creates a new <see cref="IntRange"/> with <see cref="Min"/> and <see cref="Max"/> set to the same value.
        /// </summary>
        public IntRange(int value)
        {
            Min = value;
            Max = value;
        }

        public static implicit operator FloatRange(IntRange range)
            => new FloatRange(range.Min, range.Max);

        public static IntRange operator +(IntRange i, IntRange i2)
            => new IntRange(i.Min + i2.Min, i.Max + i2.Max);
        public static IntRange operator -(IntRange i, IntRange i2)
            => new IntRange(i.Min - i2.Min, i.Max - i2.Max);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -117642446;
            hashCode = hashCode * -1521134295 + Min.GetHashCode();
            hashCode = hashCode * -1521134295 + Max.GetHashCode();
            return hashCode;
        }
    }
}
