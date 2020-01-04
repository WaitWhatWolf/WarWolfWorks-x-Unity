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
        public int Min;
        public int Max;

        /// <summary>
        /// Returns the value given clamped between MinRange and MaxRange.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetClampedValue(int value)
        {
            return Mathf.Clamp(value, Min, Max);
        }

        /// <summary>
        /// Returns true if the given value is within Min (inclusive) and Max (inclusive).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsWithinRange(int value)
        {
            return value >= Min && value <= Max;
        }

        public (int min, int max) GetTupleRange() => (Min, Max);

        public bool Equals(IntRange other)
        {
            return other.Min == Min && other.Max == Max;
        }

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
