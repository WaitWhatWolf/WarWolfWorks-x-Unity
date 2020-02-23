using System;
using System.Collections;
using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Statistics
{
    /// <summary>
    /// <see cref="IStat"/> which is removed after a given countdown.
    /// </summary>
    [Serializable]
    public sealed class CountdownStat : IStat
    {
        [SerializeField]
        private float value;
        float IStat.Value => value;
        /// <summary>
        /// Use this value to set the base value of this stat. (Set-Only)
        /// </summary>
        public float SetValue
        {
            set => this.value = value;
        }

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

        [SerializeField]
        private float countdown;
        /// <summary>
        /// Starting countdown.
        /// </summary>
        public float Countdown => countdown;

        /// <summary>
        /// Create a Stat.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stacking"></param>
        /// <param name="countdown"></param>
        /// <param name="affections"></param>
        public CountdownStat(float value, int stacking, float countdown, params int[] affections)
        {
            this.value = value;
            this.stacking = stacking;
            this.countdown = countdown;
            this.affections = affections;
        }

        /// <summary>
        /// Creates a duplicate of the given stat.
        /// </summary>
        /// <param name="stat"></param>
        public CountdownStat(CountdownStat stat)
        {
            value = stat.value;
            stacking = stat.stacking;
            affections = stat.affections;
            countdown = stat.countdown;
        }

        /// <summary>
        /// Creates a duplicate of the given stat with a different value.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        public CountdownStat(CountdownStat stat, float value)
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

        void IStat.OnAdded(Stats to)
        {
            to.EntityMain.StartCoroutine(IStartCountdown(to));
        }

        private IEnumerator IStartCountdown(Stats stats)
        {
            yield return new WaitForSeconds(countdown);
            stats.RemoveStat(this);
        }

        /// <summary>
        /// Returns the Stat's value implicitly.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator float(CountdownStat s) => s.value;
        /// <summary>
        /// Returns the Stat's value implicitly as int.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator int(CountdownStat s) => (int)s.value;
        /// <summary>
        /// Returns an equivalent Stat to this <see cref="CountdownStat"/>.
        /// </summary>
        /// <param name="stat"></param>
        public static implicit operator Stat(CountdownStat stat) => new Stat(stat.value, stat.Stacking, stat.Affections);


        /// <summary>
        /// Returns the <see cref="CountdownStat"/>'s value in string.
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
            baseText += $"|{Countdown}";
            return baseText;
        }
    }
}
