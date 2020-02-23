﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A float value which changes based on it's level.
    /// </summary>
    [Serializable]
    public struct LevelFloat : ICloneable, IEquatable<float>, IComparable<float>
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

        private float Value
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
        }

        [SerializeField]
        private float defaultValue;
        /// <summary>
        /// Value which is used when the current level is 0.
        /// </summary>
        public float DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        [SerializeField]
        private int level;
        /// <summary>
        /// This LevelFloat's current level.
        /// </summary>
        public int Level
        {
            get => level;
            set => level = value;
        }

        [SerializeField]
        private LevelValue[] levelValues;
        /// <summary>
        /// All values used to return the LevelFloat's value.
        /// </summary>
        public LevelValue[] Values
        {
            get => levelValues;
            set => levelValues = value;
        }

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

        private void HandleCloned(LevelFloat clone)
        {
            for(int i = 0; i < levelValues.Length; i++)
            {
                clone.levelValues[i] = new LevelValue(levelValues[i].Value, levelValues[i].Level);
            }
        }

        /// <summary>
        /// Implementation of <see cref="IComparable{T}"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(float other)
        {
            if (this > other)
                return 1;
            else if (this < other)
                return -1;
            else return 0;
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
        /// Returns <see cref="TypeCode.Single"/>.
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Single;
        }

        /// <summary>
        /// Creates a <see cref="LevelFloat"/>.
        /// </summary>
        /// <param name="_DefaultValue"></param>
        /// <param name="_Level"></param>
        /// <param name="_Values"></param>
        public LevelFloat(float _DefaultValue, int _Level, params LevelValue[] _Values)
        {
            defaultValue = _DefaultValue;
            level = _Level;
            levelValues = _Values;
        }

        /// <summary>
        /// Returns this <see cref="LevelFloat"/>'s hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 1756548625;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + defaultValue.GetHashCode();
            hashCode = hashCode * -1521134295 + level.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<LevelValue[]>.Default.GetHashCode(levelValues);
            return hashCode;
        }

        /// <summary>
        /// Implicitly returns <see cref="Value"/>.
        /// </summary>
        /// <param name="lf"></param>
        public static implicit operator float(LevelFloat lf)
        {
            return lf.Value;
        }

        /// <summary>
        /// Returns a <see cref="LevelFloat"/> with no level values, level at 0 and defaultVal as the implicit value given.
        /// </summary>
        /// <param name="defval"></param>
        public static implicit operator LevelFloat(float defval)
        {
            return new LevelFloat(defval, 0);
        }
    }
}
