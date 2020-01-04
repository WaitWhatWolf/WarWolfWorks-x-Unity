using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A float value which changes based on it's level.
    /// </summary>
    [Serializable]
    public struct LevelFloat : ICloneable, IEquatable<float>, IComparable<float>, IConvertible, IEqualityComparer<float>
    {
        [Serializable]
        public struct LevelValue
        {
            public float Value;
            public int Level;

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

        public int CompareTo(float other)
        {
            if (this > other)
                return 1;
            else if (this < other)
                return -1;
            else return 0;
        }

        public bool Equals(float other)
        {
            return other == this;
        }

        public bool Equals(float x, float y)
        {
            return x == y;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Single;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Value > 0;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Value);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Value);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Value);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Value);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Value);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Value);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Value);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Value);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Value);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Value;
        }

        public string ToString(IFormatProvider provider)
        {
            return Value.ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, conversionType);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value);
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
        /// Returns the given obj's hash code.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// Implicitly returns <see cref="LevelFloat.Value"/>.
        /// </summary>
        /// <param name="lf"></param>
        public static implicit operator float(LevelFloat lf)
        {
            return lf.Value;
        }
    }
}
