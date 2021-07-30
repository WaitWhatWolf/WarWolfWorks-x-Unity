using System;
using System.Text.RegularExpressions;
using UnityEngine;
using static WarWolfWorks.Utility.Hooks;
using static WarWolfWorks.WWWResources;

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
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        /// <summary>
        /// Creates a new <see cref="IntRange"/> with <see cref="Min"/> and <see cref="Max"/> set to the same value.
        /// </summary>
        public IntRange(int value)
        {
            Min = value;
            Max = value;
        }

        /// <summary>
        /// Creates a new <see cref="IntRange"/> based on a string.
        /// </summary>
        /// <example>"0-53", will set <see cref="Min"/> to 0 and <see cref="Max"/> to 53. "0,53" is also acceptable; Any form of brackets such as {0,53} or [0-53] is also acceptable. Negative values are acceptable, as long as the "-" character precedes a number.</example>
        /// <param name="range">The string to pass, it shouldn't contain any special characters outside of one '-' or ','.</param>
        /// <exception cref="FormatException"/>
        public IntRange(string range)
        {
            Match match = Expression_IntRange_Range.Match(range);
            if (match == null || !match.Success)
                throw new FormatException(range + "is invalid.");

            int rangeType = 0;

            if (match.Value.Contains(","))
                rangeType = 1;

            string[] splits = rangeType switch 
            {
                1 => range.Split(','),
                _ => range.Split('-')
            };

            int val0 = Convert.ToInt32(splits[0]);
            int val1 = Convert.ToInt32(splits[1]);

            Min = Math.Min(val0, val1);
            Max = Math.Max(val0, val1);
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
