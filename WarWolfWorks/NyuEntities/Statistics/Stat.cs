using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.Statistics
{
    /// <summary>
    /// Base class used for all <see cref="Nyu"/> statistics.
    /// </summary>
    [Serializable]
    public sealed class Stat : INyuStat, IEquatable<Stat>
    {
        #region Unity Serialized
        [SerializeField]
        private float s_Value;
        [SerializeField]
        private int s_Stacking;
        [SerializeField]
        private int[] s_Affections;
        #endregion

        /// <summary>
        /// Gets or sets the value of this stat.
        /// </summary>
        public float Value
        {
            get => s_Value;
            set => s_Value = value;
        }

        /// <summary>
        /// How this <see cref="Stat"/> is calculated by a <see cref="INyuStacking"/>.
        /// </summary>
        public int Stacking
        {
            get => s_Stacking;
            set => s_Stacking = value;
        }

        /// <summary>
        /// Stats with which this <see cref="Stat"/> will interact with.
        /// </summary>
        public int[] Affections
        {
            get => s_Affections;
            set => s_Affections = value;
        }


        /// <summary>
        /// Create a Stat.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stacking"></param>
        /// <param name="affections"></param>
        public Stat(float value, int stacking, params int[] affections)
        {
            this.s_Value = value;
            this.s_Stacking = stacking;
            this.s_Affections = affections;
        }

        /// <summary>
        /// Creates a duplicate of the given stat.
        /// </summary>
        /// <param name="stat"></param>
        public Stat(Stat stat)
        {
            s_Value = stat.s_Value;
            s_Stacking = stat.s_Stacking;
            s_Affections = stat.s_Affections;
        }

        /// <summary>
        /// Creates a duplicate of the given stat with a different value.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        public Stat(Stat stat, float value)
        {
            this.s_Value = value;
            s_Stacking = stat.s_Stacking;
            s_Affections = stat.s_Affections;
        }

        /// <summary>
        /// Returns the value in string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string baseText = $"{s_Value}|{Stacking}|";
            for (int i = 0; i < Affections.Length; i++)
            {
                baseText += $"{Affections[i]}";
                if (i != Affections.Length - 1)
                    baseText += ',';
            }
            return baseText;
        }

        /// <summary>
        /// Returns the Stat's value implicitly.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator float(Stat s) => s.s_Value;
        /// <summary>
        /// Returns the Stat's value implicitly as int.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator int(Stat s) => (int)s.s_Value;

        void INyuStat.OnAdded(Stats to) { }
        void INyuStat.OnRemoved(Stats to) { }

        /// <summary>
        /// Returns true if this <see cref="Stat"/> is equal to another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Stat other)
        {
            return other != null &&
                   Value == other.Value &&
                   Stacking == other.Stacking &&
                   EqualityComparer<int[]>.Default.Equals(Affections, other.Affections);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = -1432848062;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + Stacking.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<int[]>.Default.GetHashCode(Affections);
            return hashCode;
        }
    }
}
