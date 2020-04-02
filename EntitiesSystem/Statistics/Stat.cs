using System;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Statistics
{
    /// <summary>
    /// Base class used for all <see cref="Entity"/> statistics.
    /// </summary>
    [Serializable]
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public sealed class Stat : IStat
    {
        [SerializeField][FormerlySerializedAs("Value")]
        private float value;
        float IStat.Value => value;
        /// <summary>
        /// Use this value to set the base value of this stat. (Set-Only)
        /// </summary>
        public float SetValue
        {
            set => this.value = value;
        }

        [FormerlySerializedAs("Stacks")]
        [SerializeField]
        private int stacking;
        /// <summary>
        /// How the Stat should be calculated.
        /// </summary>
        public int Stacking
        {
            get => stacking;
            set => stacking = value;
        }

        [FormerlySerializedAs("Affects")]
        [FormerlySerializedAs("affection")]
        [SerializeField]
        private int[] affections;
        /// <summary>
        /// Which stats will this stat interact with.
        /// </summary>
        public int[] Affections
        {
            get => affections;
            set => affections = value;
        }

        void IStat.OnAdded(Stats to) { }

        /// <summary>
        /// Create a Stat.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stacking"></param>
        /// <param name="affections"></param>
        public Stat(float value, int stacking, params int[] affections)
        {
            this.value = value;
            this.stacking = stacking;
            this.affections = affections;
        }

        /// <summary>
        /// Creates a duplicate of the given stat.
        /// </summary>
        /// <param name="stat"></param>
        public Stat(Stat stat)
        {
            value = stat.value;
            stacking = stat.stacking;
            affections = stat.affections;
        }

        /// <summary>
        /// Creates a duplicate of the given stat with a different value.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        public Stat(Stat stat, float value)
        {
            this.value = value;
            stacking = stat.stacking;
            affections = stat.affections;
        }

        /// <summary>
        /// Returns true if the Stat's value returns other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(float other) => value == other;

        /// <summary>
        /// Returns the value in string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string baseText = $"{value}|{Stacking}|";
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
        public static implicit operator float(Stat s) => s.value;
        /// <summary>
        /// Returns the Stat's value implicitly as int.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator int(Stat s) => (int)s.value;

    }
}