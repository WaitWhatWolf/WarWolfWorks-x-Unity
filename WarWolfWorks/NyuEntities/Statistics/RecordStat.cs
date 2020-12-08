using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.Statistics
{
    /// <summary>
    /// A <see cref="INyuStat"/> which uses the record reference type from C#9.0.
    /// </summary>
    public sealed record RecordStat : INyuStat
    {
        [SerializeField]
        private float s_Value;
        float INyuStat.Value => s_Value;
        /// <summary>
        /// Use this value to set the base value of this stat. (Set-Only)
        /// </summary>
        public float SetValue
        {
            set => this.s_Value = value;
        }

        [SerializeField]
        private int s_Stacking;
        /// <summary>
        /// How the Stat should be calculated.
        /// </summary>
        public int Stacking
        {
            get => s_Stacking;
            set => s_Stacking = value;
        }

        [SerializeField]
        private int[] s_Affections;
        /// <summary>
        /// Which stats will this stat interact with.
        /// </summary>
        public int[] Affections
        {
            get => s_Affections;
            set => s_Affections = value;
        }

        void INyuStat.OnAdded(Stats to) { }

        /// <summary>
        /// Create a Stat.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stacking"></param>
        /// <param name="affections"></param>
        public RecordStat(float value, int stacking, params int[] affections)
            => (s_Value, s_Stacking, s_Affections) = (value, stacking, affections);

        /// <summary>
        /// Creates a duplicate of the given stat.
        /// </summary>
        /// <param name="stat"></param>
        public RecordStat(RecordStat stat)
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
        public RecordStat(RecordStat stat, float value)
        {
            this.s_Value = value;
            s_Stacking = stat.s_Stacking;
            s_Affections = stat.s_Affections;
        }

        /// <summary>
        /// Returns true if the Stat's value returns other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(float other) => s_Value == other;

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
        public static implicit operator float(RecordStat s) => s.s_Value;
        /// <summary>
        /// Returns the Stat's value implicitly as int.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator int(RecordStat s) => (int)s.s_Value;
    }
}
