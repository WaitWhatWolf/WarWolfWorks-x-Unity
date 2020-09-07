using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystemV2
{
    /// <summary>
    /// A velocity which removes itself after it's countdown has ran out.
    /// </summary>
    public class CountdownVelocity : Velocity, INyuFixedUpdate
    {
        /// <summary>
        /// Value at which the countdown started.
        /// </summary>
        public float StartCountdown;
        /// <summary>
        /// The current countdown.
        /// </summary>
        public float Countdown;

        /// <summary>
        /// Returns the progress of the countdown in 0-1.
        /// </summary>
        public float Time01 => Countdown / StartCountdown;

        /// <summary>
        /// Creates a simple countdown velocity without affections.
        /// </summary>
        public CountdownVelocity(Vector3 value, float countdown) : base(value)
        {
            StartCountdown = Countdown = countdown;
        }

        /// <summary>
        /// Creates a duplicate of a countdown velocity.
        /// </summary>
        public CountdownVelocity(CountdownVelocity original) : base(original)
        {
            StartCountdown = original.StartCountdown;
            Countdown = original.Countdown;
        }

        /// <summary>
        /// Creates a countdown velocity with affections.
        /// </summary>
        public CountdownVelocity(Vector3 value, float countdown, params int[] affections) : base(value, affections)
        {
            StartCountdown = Countdown = countdown;
        }

        /// <summary>
        /// Manages the timer.
        /// </summary>
        public virtual void NyuFixedUpdate()
        {
            Countdown = Mathf.Clamp(Countdown - Time.deltaTime, 0, StartCountdown);
            if (Countdown == 0)
                Parent.RemoveVelocity(this);
            /*hs_Velocities[i].Time = Mathf.Clamp(hs_Velocities[i].Time - Time.deltaTime, 0, hs_Velocities[i].StartTime);
                if (hs_Velocities[i].Time <= 0 && hs_Velocities[i].DeleteOnCount0)
                    RemoveVelocity(hs_Velocities[i]);*/
        }
    }
}
