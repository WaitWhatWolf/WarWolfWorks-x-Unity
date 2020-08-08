using System;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.YharonSystem
{
    /// <summary>
    /// A <see cref="Yharon"/> which uses a countdown, after which it will be automatically removed from it's parent.
    /// </summary>
    public abstract class CountdownYharon : Yharon, INyuUpdate
    {
        /// <summary>
        /// Invoked when <see cref="SetCountdown(float)"/> or <see cref="SetStartCountdown(float, bool)"/> with bool value as true are successfully called.
        /// </summary>
        public event Action<CountdownYharon> OnCountdownSet;
        
        /// <summary>
        /// Invoked when <see cref="SetStartCountdown(float, bool)"/> is successfully called.
        /// </summary>
        public event Action<CountdownYharon> OnStartCountdownSet;

        /// <summary>
        /// The starting countdown of this <see cref="CountdownYharon"/>.
        /// </summary>
        public float StartCountdown { get; private set; }

        private float ns_Countdown;

        /// <summary>
        /// Returns the current countdown of this <see cref="CountdownYharon"/>.
        /// </summary>
        /// <returns></returns>
        public float GetCountdown() => ns_Countdown;

        /// <summary>
        /// Returns the percent of the countdown on 0-1 range. (1 is beginning, 0 is ending)
        /// </summary>
        /// <returns></returns>
        public float GetCountdown01() => ns_Countdown / StartCountdown;

        /// <summary>
        /// Attempts to set the current countdown of the <see cref="CountdownYharon"/>.
        /// </summary>
        /// <param name="to"></param>
        public void SetCountdown(float to)
        {
            if(to != ns_Countdown)
            {
                ns_Countdown = Math.Min(to, StartCountdown);
                CountdownChanges++;
                OnCountdownSet?.Invoke(this);
            }
        }

        /// <summary>
        /// The amount of times <see cref="SetStartCountdown(float, bool)"/> was successfully called.
        /// </summary>
        public int StartCountdownChanges { get; private set; } = 0;
        /// <summary>
        /// The amount of times <see cref="SetCountdown(float)"/> or <see cref="SetStartCountdown(float, bool)"/> with the bool value as true 
        /// was successfully called.
        /// </summary>
        public int CountdownChanges { get; private set; } = 0;

        /// <summary>
        /// Attempts to set <see cref="StartCountdown"/> to the given value.
        /// </summary>
        /// <param name="to">The value used to set.</param>
        /// <param name="resetCountdown">If true,</param>
        public void SetStartCountdown(float to, bool resetCountdown)
        {
            if(to != StartCountdown)
            {
                StartCountdown = to;
                StartCountdownChanges++;

                OnStartCountdownSet?.Invoke(this);

                if(resetCountdown)
                {
                    SetCountdown(to);
                }
            }
        }

        /// <summary>
        /// Invoked when countdown reaches 0; The yharon will not remove itself as long as this method returns false.
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnCountdownEnd()
        {
            return true;
        }

        /// <summary>
        /// When overriding, make sure to include base.Update() as it is what handles the countdown.
        /// </summary>
        public virtual void NyuUpdate()
        {
            ns_Countdown -= Time.deltaTime;

            if(ns_Countdown <= 0 && OnCountdownEnd())
            {
                Parent.RemoveYharon(this);
            }
        }

        /// <summary>
        /// Creates a <see cref="CountdownYharon"/> with the specified countdown.
        /// </summary>
        /// <param name="countdown"></param>
        public CountdownYharon(float countdown)
        {
            ns_Countdown = StartCountdown = countdown;
        }
    }
}
