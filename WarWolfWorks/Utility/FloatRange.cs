using System;
using System.Text.RegularExpressions;
using UnityEngine;
using static WarWolfWorks.Utility.Hooks;
using static WarWolfWorks.WWWResources;

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
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        /// <summary>
        /// Creates a new <see cref="FloatRange"/> with <see cref="Min"/> and <see cref="Max"/> set to the same value.
        /// </summary>
        public FloatRange(float value)
        {
            Min = value;
            Max = value;
        }

        /// <summary>
        /// Creates a new <see cref="IntRange"/> based on a string.
        /// </summary>
        /// <example>"0,5-53,6", will set <see cref="Min"/> to 0.5f and <see cref="Max"/> to 53.6f. Any form of brackets such as {0-53} or [0-53,7] is also acceptable. Negative values are acceptable, as long as the "-" character precedes a number.</example>
        /// <param name="range">The string to pass, it shouldn't contain any special characters outside of one '-' or ','.</param>
        /// <exception cref="FormatException"/>
        public FloatRange(string range)
        {
            Match match = Expression_FloatRange_Range.Match(range);
            if (match == null || !match.Success)
                throw new FormatException(range + "is invalid.");

            int rangeType = 0;

            string[] splits = rangeType switch //Still keeping this in in case I want to add more range separators, doesn't really cost any performance so why not
            {
                _ => range.Split('-')
            };

            float val0 = Convert.ToSingle(splits[0]);
            float val1 = Convert.ToSingle(splits[1]);

            Min = Math.Min(val0, val1);
            Max = Math.Max(val0, val1);
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
