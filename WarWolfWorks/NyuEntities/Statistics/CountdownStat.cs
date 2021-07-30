using System;
using System.Collections;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.Statistics
{
    /// <summary>
    /// <see cref="INyuStat"/> which is removed after a given countdown.
    /// </summary>
    [Serializable]
    public sealed class CountdownStat : INyuStat
    {
        #region Unity Serialized
        [SerializeField]
        private float s_Value;
        [SerializeField]
        private int s_Stacking;
        [SerializeField]
        private int[] s_Affections;
        [SerializeField]
        private float s_Countdown;
        #endregion

        /// <summary>
        /// Gets or sets the value of this stat.
        /// </summary>
        public float Value
        {
            get => s_Value = Value;
            set => this.s_Value = value;
        }

        
        /// <summary>
        /// How this <see cref="CountdownStat"/> is calculated by a <see cref="INyuStacking"/>.
        /// </summary>
        public int Stacking
        {
            get => s_Stacking;
            set => s_Stacking = value;
        }

        /// <summary>
        /// Stats with which this <see cref="CountdownStat"/> will interact with.
        /// </summary>
        public int[] Affections
        {
            get => s_Affections;
            set => s_Affections = value;
        }

        /// <summary>
        /// Starting countdown.
        /// </summary>
        public float Countdown => s_Countdown;

        /// <summary>
        /// Create a Stat.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stacking"></param>
        /// <param name="countdown"></param>
        /// <param name="affections"></param>
        public CountdownStat(float value, int stacking, float countdown, params int[] affections)
        {
            this.s_Value = value;
            this.s_Stacking = stacking;
            this.s_Countdown = countdown;
            this.s_Affections = affections;
        }

        /// <summary>
        /// Creates a duplicate of the given stat.
        /// </summary>
        /// <param name="stat"></param>
        public CountdownStat(CountdownStat stat)
        {
            s_Value = stat.s_Value;
            s_Stacking = stat.s_Stacking;
            s_Affections = stat.s_Affections;
            s_Countdown = stat.s_Countdown;
        }

        /// <summary>
        /// Creates a duplicate of the given stat with a different value.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        public CountdownStat(CountdownStat stat, float value)
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
        /// Returns the <see cref="CountdownStat"/>'s value in string.
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
            baseText += $"|{Countdown}";
            return baseText;
        }

        /// <summary>
        /// Returns the Stat's value implicitly.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator float(CountdownStat s) => s.s_Value;
        /// <summary>
        /// Returns the Stat's value implicitly as int.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator int(CountdownStat s) => (int)s.s_Value;
        /// <summary>
        /// Returns an equivalent Stat to this <see cref="CountdownStat"/>.
        /// </summary>
        /// <param name="stat"></param>
        public static implicit operator Stat(CountdownStat stat) => new Stat(stat.s_Value, stat.Stacking, stat.Affections);

        void INyuStat.OnAdded(Stats to)
        {
            to.NyuMain.StartCoroutine(IStartCountdown(to));
        }

        void INyuStat.OnRemoved(Stats to)
        {
            to.NyuMain.StopCoroutine(IStartCountdown(to));
        }

        private IEnumerator IStartCountdown(Stats stats)
        {
            yield return new WaitForSeconds(s_Countdown);
            stats.RemoveStat(this);
        }
    }
}
