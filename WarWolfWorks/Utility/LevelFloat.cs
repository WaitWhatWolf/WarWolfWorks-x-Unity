using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A float value which changes based on it's level.
    /// </summary>
    [Serializable]
    public struct LevelFloat : ICloneable, IEquatable<float>, IComparable<LevelFloat>, IComparable<float>
    {
        /// <summary>
        /// Used with <see cref="LevelFloat"/>.
        /// </summary>
        [Serializable]
        public struct LevelValue
        {
            /// <summary>
            /// Value of this <see cref="LevelValue"/>.
            /// </summary>
            public float Value;
            /// <summary>
            /// Level at which <see cref="Value"/> will be used.
            /// </summary>
            public int Level;

            /// <summary>
            /// Creates a new <see cref="LevelValue"/>.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="level"></param>
            public LevelValue(float value, int level)
            {
                Value = value;
                Level = level;
            }
        }

        /// <summary>
        /// Returns or sets the final value of this <see cref="LevelFloat"/> based on it's <see cref="Level"/>.
        /// </summary>
        public float Value
        {
            get
            {
                try
                {
                    if (Level >= Values.Length)
                        return Values[Values.Length - 1].Value;
                    else if (Level != 0)
                    {
                        int lvl = Level;
                        return Values.Find(v => v.Level == lvl).Value;
                    }

                    return DefaultValue;
                }
                catch
                {
                    return DefaultValue;
                }
            }
            set
            {
                int lv = Level;
                int index = Array.FindIndex(Values, v => v.Level == lv);
                if (index != -1)
                    Values[index] = new LevelValue(value, lv);
            }
        }

        /// <summary>
        /// Value which is used when the current level is 0 or when an exception occurs.
        /// </summary>
        [FormerlySerializedAs("s_DefaultValue")]
        public float DefaultValue;

        /// <summary>
        /// The current level of the <see cref="LevelFloat"/>.
        /// </summary>
        [FormerlySerializedAs("s_Level")]
        public int Level;

        /// <summary>
        /// All level values used for this <see cref="LevelFloat"/> to return it's <see cref="Value"/>.
        /// </summary>
        [FormerlySerializedAs("s_LevelValues")]
        public LevelValue[] Values;

        /// <summary>
        /// Clones this LevelFloat.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = MemberwiseClone();
            HandleCloned((LevelFloat)clone);
            return clone;
        }

        /// <summary>
        ///  Implement the generic CompareTo method with <see cref="Single"/> as argument.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(float other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Implement the generic CompareTo method with <see cref="LevelFloat"/> as argument.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(LevelFloat other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Returns true if this <see cref="LevelFloat"/>'s current value equals other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(float other)
        {
            return other == this;
        }

        /// <summary>
        /// Creates a <see cref="LevelFloat"/>.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="level"></param>
        /// <param name="values"></param>
        public LevelFloat(float defaultValue, int level, params LevelValue[] values)
        {
            DefaultValue = defaultValue;
            Level = level;
            Values = values;
        }

        /// <summary>
        /// Creates a <see cref="LevelFloat"/> using a collection.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="level"></param>
        /// <param name="collection"></param>
        public LevelFloat(float defaultValue, int level, IEnumerable<LevelValue> collection)
        {
            DefaultValue = defaultValue;
            Level = level;
            Values = collection.ToArray();
        }

        /// <summary>
        /// Implicitly returns <see cref="Value"/>.
        /// </summary>
        /// <param name="lf"></param>
        public static implicit operator float(LevelFloat lf)
        {
            return lf.Value;
        }

        private void HandleCloned(LevelFloat clone)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                clone.Values[i] = new LevelValue(Values[i].Value, Values[i].Level);
            }
        }
    }
}
