using System;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem.Statistics
{
    /// <summary>
    /// Stat which uses <see cref="LevelFloat"/> as value.
    /// </summary>
    [Serializable]
    public sealed class LevelStat : IStat, IEquatable<Stat>
    {
        [SerializeField]
        private LevelFloat value;

        float IStat.Value => value;
        /// <summary>
        /// This stat's level.
        /// </summary>
        public int Level
        {
            get => value.Level;
            set => this.value.Level = value;
        }

        /// <summary>
        /// All LevelValues of this stat.
        /// </summary>
        public LevelFloat.LevelValue[] Values
        {
            get => value.Values;
            set => this.value.Values = value;
        }
        /// <summary>
        /// Sets the default value of the <see cref="LevelFloat"/> contained by this <see cref="LevelStat"/>.
        /// </summary>
        public float SetValue { set => this.value.DefaultValue = value; }

        [SerializeField]
        [FormerlySerializedAs("Stacks")]
        private int stacking;
        /// <summary>
        /// How is this LevelStat calculated.
        /// </summary>
        public int Stacking { get => stacking; set => stacking = value; }

        [FormerlySerializedAs("Affects")]
        [FormerlySerializedAs("affection")]
        [SerializeField]
        private int[] affections;
        /// <summary>
        /// Which stats will this LevelStat interact with.
        /// </summary>
        public int[] Affections { get => affections; set => affections = value; }

        /// <summary>
        /// Creates a LevelStat instance with multiple affections.
        /// </summary>
        /// <param name="levelval"></param>
        /// <param name="stacking"></param>
        /// <param name="affections"></param>
        public LevelStat(LevelFloat levelval, int stacking, int[] affections)
        {
            value = levelval;
            this.stacking = stacking;
            this.affections = affections;
        }

        /// <summary>
        /// Creates a duplicate of a LevelStat with the specified level.
        /// </summary>
        /// <param name="levelStat"></param>
        /// <param name="level"></param>
        public LevelStat(LevelStat levelStat, int level)
        {
            value = levelStat.value;
            stacking = levelStat.stacking;
            affections = levelStat.affections;
            Level = level;
        }

        /// <summary>
        /// Returns the LevelStat's value implicitly.
        /// </summary>
        /// <param name="stat"></param>
        public static implicit operator float(LevelStat stat) => stat.value;
        /// <summary>
        /// Returns the LevelStat's value implicitly as int.
        /// </summary>
        /// <param name="stat"></param>
        public static implicit operator int(LevelStat stat) => (int)stat.value;
        /// <summary>
        /// Returns an equivalent Stat to this LevelStat.
        /// </summary>
        /// <param name="stat"></param>
        public static implicit operator Stat(LevelStat stat) => new Stat(stat.value, stat.Stacking, stat.Affections);
        /// <summary>
        /// Returns explicitly the <see cref="LevelStat"/>'s <see cref="LevelFloat"/> value.
        /// </summary>
        /// <param name="stat"></param>
        public static explicit operator LevelFloat(LevelStat stat) => stat.value;

        /// <summary>
        /// Returns the LevelStat's value in string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return value.ToString();
        }

        /// <summary>
        /// Returns true if the LevelStat's value is equal to other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(float other)
        {
            return value == other;
        }

        /// <summary>
        /// Returns true if all <see cref="IStat"/> variables from this LevelStat equals to all <see cref="IStat"/> variables from other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Stat other)
        {
            IStat compared = this;
            IStat comparing = other;
            return compared.Value == comparing.Value &&
                compared.Stacking == comparing.Stacking &&
                compared.Affections == comparing.Affections;
        }
    }
}