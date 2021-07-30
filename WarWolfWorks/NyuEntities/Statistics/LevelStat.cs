using System;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities.Statistics
{
    /// <summary>
    /// Stat which uses <see cref="LevelFloat"/> as value.
    /// </summary>
    [Serializable]
    public sealed class LevelStat : INyuStat, IEquatable<Stat>
    {
        #region Unity Serialized
        [SerializeField]
        private LevelFloat s_Value;
        [SerializeField]
        private int s_Stacking;
        [SerializeField]
        private int[] s_Affections;
        #endregion

        /// <summary>
        /// Gets or sets a value based on the level.
        /// </summary>
        public float Value
        {
            get => s_Value.Value;
            set => s_Value.Value = value; 
        }

        /// <summary>
        /// This stat's level.
        /// </summary>
        public int Level
        {
            get => s_Value.Level;
            set => this.s_Value.Level = value;
        }

        /// <summary>
        /// All LevelValues of this stat.
        /// </summary>
        public LevelFloat.LevelValue[] Values
        {
            get => s_Value.Values;
            set => this.s_Value.Values = value;
        }

        /// <summary>
        /// Sets the default value of the <see cref="LevelFloat"/> contained by this <see cref="LevelStat"/>.
        /// </summary>
        public float SetDefaultValue { set => this.s_Value.DefaultValue = value; }

        /// <summary>
        /// How this <see cref="LevelStat"/> is calculated by a <see cref="INyuStacking"/>.
        /// </summary>
        public int Stacking { get => s_Stacking; set => s_Stacking = value; }

        
        /// <summary>
        /// Stats with which this <see cref="LevelStat"/> will interact with.
        /// </summary>
        public int[] Affections { get => s_Affections; set => s_Affections = value; }

        /// <summary>
        /// Creates a <see cref="LevelStat"/> with a default value, a level, stacking and affections.
        /// </summary>
        /// <param name="defaultVal"></param>
        /// <param name="level"></param>
        /// <param name="stacking"></param>
        /// <param name="affections"></param>
        public LevelStat(float defaultVal, int level, int stacking, params int[] affections)
        {
            s_Value = new LevelFloat(defaultVal, level);
            this.s_Stacking = stacking;
            this.s_Affections = affections;
        }

        /// <summary>
        /// Creates a LevelStat instance with multiple affections.
        /// </summary>
        /// <param name="levelval"></param>
        /// <param name="stacking"></param>
        /// <param name="affections"></param>
        public LevelStat(LevelFloat levelval, int stacking, params int[] affections)
        {
            s_Value = levelval;
            this.s_Stacking = stacking;
            this.s_Affections = affections;
        }

        /// <summary>
        /// Creates a duplicate of a LevelStat with the specified level.
        /// </summary>
        /// <param name="levelStat"></param>
        /// <param name="level"></param>
        public LevelStat(LevelStat levelStat, int level)
        {
            s_Value = levelStat.s_Value;
            s_Stacking = levelStat.s_Stacking;
            s_Affections = levelStat.s_Affections;
            Level = level;
        }

        /// <summary>
        /// Returns the <see cref="LevelStat"/>'s value in string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string baseText = $"{(float)s_Value}|{Stacking}|";
            for (int i = 0; i < Affections.Length; i++)
            {
                baseText += $"{Affections[i]}";
                if (i != Affections.Length - 1)
                    baseText += ',';
            }
            baseText += $"|{Level}";
            return baseText;
        }

        /// <summary>
        /// Returns true if the LevelStat's value is equal to other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(float other)
        {
            return s_Value == other;
        }

        /// <summary>
        /// Returns true if all <see cref="INyuStat"/> variables from this <see cref="LevelStat"/> equals to all <see cref="INyuStat"/> variables from other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Stat other)
        {
            INyuStat compared = this;
            INyuStat comparing = other;
            return compared.Value == comparing.Value &&
                compared.Stacking == comparing.Stacking &&
                compared.Affections == comparing.Affections;
        }

        /// <summary>
        /// Returns the <see cref="LevelStat"/>'s value implicitly.
        /// </summary>
        /// <param name="stat"></param>
        public static implicit operator float(LevelStat stat) => stat.s_Value;
        /// <summary>
        /// Returns the <see cref="LevelStat"/>'s value implicitly as int.
        /// </summary>
        /// <param name="stat"></param>
        public static explicit operator int(LevelStat stat) => (int)stat.s_Value;
        /// <summary>
        /// Returns an equivalent Stat to this <see cref="LevelStat"/>.
        /// </summary>
        /// <param name="stat"></param>
        public static explicit operator Stat(LevelStat stat) => new Stat(stat.s_Value, stat.Stacking, stat.Affections);
        /// <summary>
        /// Returns explicitly the <see cref="LevelStat"/>'s <see cref="LevelFloat"/> value.
        /// </summary>
        /// <param name="stat"></param>
        public static explicit operator LevelFloat(LevelStat stat) => stat.s_Value;

        void INyuStat.OnAdded(Stats to) { }
        void INyuStat.OnRemoved(Stats to) { }
    }
}