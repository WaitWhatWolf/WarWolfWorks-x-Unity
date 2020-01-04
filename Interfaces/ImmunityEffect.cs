using System.Diagnostics;
using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem
{
    /// <summary>
    /// <see cref="ScriptableObject"/> used for custom effect when an <see cref="Entity"/> enters immunity through <see cref="EntityHealth"/>.
    /// </summary>
    public abstract class ImmunityEffect : ScriptableObject, IImmunityEffect
    {
        internal Stopwatch StopwatchDuration { get; set; }
        /// <summary>
        /// Time since the immunity was triggered after <see cref="OnTrigger(Entity)"/> (read-only).
        /// </summary>
        protected float TriggerDuration => StopwatchDuration.Elapsed.Milliseconds / 1000;
        /// <summary>
        /// What happens when Immunity first triggers.
        /// </summary>
        /// <param name="entity"></param>
        public abstract void OnTrigger(Entity entity);

        /// <summary>
        /// What happens when Immunity stops.
        /// </summary>
        public abstract void OnEnd();
    }
}
