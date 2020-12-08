using System;
using UnityEngine;
using static WarWolfWorks.Utility.Hooks;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Range struct for Clamping/Range utility in <see cref="Single"/> value.
    /// </summary>
    [Serializable]
    public sealed record FloatRange : IEquatable<FloatRange>
    {
        /// <summary>
        /// The minimal value of this <see cref="FloatRange"/>.
        /// </summary>
        public float Min;
        /// <summary>
        /// The maximal value of this <see cref="FloatRange"/>.
        /// </summary>
        public float Max;

        /// <summary>
        /// Returns the value given clamped between MinRange and MaxRange.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float GetClampedValue(float value)
        {
            return MathF.Clamp(value, Min, Max);
        }

        /// <summary>
        /// Returns true if the given value is within Min (inclusive) and Max (inclusive).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsWithinRange(float value)
        {
            return value >= Min && value <= Max;
        }

        /// <summary>
        /// Returns a random value between <see cref="Min"/> (inclusive) and <see cref="Max"/> (inclusive).
        /// </summary>
        /// <returns></returns>
        public float GetRandom()
            => UnityEngine.Random.Range(Min, Max);
        
        /// <summary>
        /// Returns a <see cref="Tuple{T1, T2}"/> of <see cref="Min"/> and <see cref="Max"/>.
        /// </summary>
        /// <returns></returns>
        public (float min, float max) GetTupleRange() => (Min, Max);

        /// <summary>
        /// Returns true if Min and Max are of the exact same value for both FloatRanges.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FloatRange other)
            => other.Min == Min && other.Max == Max;

        /// <summary>
        /// Creates a new <see cref="FloatRange"/>.
        /// </summary>
        public FloatRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Creates a new <see cref="FloatRange"/> with <see cref="Min"/> and <see cref="Max"/> set to the same value.
        /// </summary>
        public FloatRange(float value)
        {
            Min = value;
            Max = value;
        }

        public static implicit operator IntRange(FloatRange range)
            => new IntRange(Convert.ToInt32(range.Min), Convert.ToInt32(range.Max));

        public static FloatRange operator +(FloatRange f, FloatRange f2)
            => new FloatRange(f.Min + f2.Min, f.Max + f2.Max);
        public static FloatRange operator -(FloatRange f, FloatRange f2)
            => new FloatRange(f.Min - f2.Min, f.Max - f2.Max);

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
