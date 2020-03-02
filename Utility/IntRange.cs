using System;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Range struct for Clamping/Range utility in <see cref="Int32"/> value.
    /// </summary>
    [Serializable]
    public struct IntRange : IEquatable<IntRange>
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
            return Mathf.Clamp(value, Min, Max - 1);
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

        public bool Equals(IntRange other)
        {
            return other.Min == Min && other.Max == Max;
        }

        /// <summary>
        /// Creates a new <see cref="IntRange"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public static implicit operator FloatRange(IntRange range)
            => new FloatRange(range.Min, range.Max);

        public static IntRange operator +(IntRange i, IntRange i2)
            => new IntRange(i.Min + i2.Min, i.Max + i2.Max);
        public static IntRange operator -(IntRange i, IntRange i2)
            => new IntRange(i.Min - i2.Min, i.Max - i2.Max);

        public static bool operator ==(IntRange i, IntRange i2)
            => i.Equals(i2);

        public static bool operator !=(IntRange i, IntRange i2)
            => !i.Equals(i2);

        public override bool Equals(object obj)
        {
            try
            {
                return Equals((IntRange)obj);
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -117642446;
            hashCode = hashCode * -1521134295 + Min.GetHashCode();
            hashCode = hashCode * -1521134295 + Max.GetHashCode();
            return hashCode;
        }
    }
}
