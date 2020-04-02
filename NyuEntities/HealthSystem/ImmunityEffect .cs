using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.NyuEntities.HealthSystem
{
    /// <summary>
    /// <see cref="ScriptableObject"/> used for custom effect when an <see cref="Nyu"/> enters immunity through <see cref="NyuHealth"/>.
    /// </summary>
    public abstract class ImmunityEffect : ScriptableObject, IImmunityEffect<NyuHealth>
    {
        /// <summary>
        /// Duration at which the current immunity will last.
        /// </summary>
        public float ImmunityTime { get; internal set; }

        /// <summary>
        /// The countdown/cooldown of the current immunity timer; To be used with ImmunityTime for percentage duration.
        /// </summary>
        public float ImmunityCountdown { get; internal set; }

        internal NyuHealth internalParent;
        /// <summary>
        /// Parent of this <see cref="ImmunityEffect"/>.
        /// </summary>
        public NyuHealth Parent => internalParent;

        /// <summary>
        /// What happens when Immunity first triggers.
        /// </summary>
        public abstract void OnTrigger();

        /// <summary>
        /// What happens when Immunity stops.
        /// </summary>
        public abstract void OnEnd();

        /// <summary>
        /// What happens when Immunity is either added to a <see cref="NyuHealth"/>, or when it was already attached to it on instantiation.
        /// </summary>
        public abstract void OnAdded();

        /// <summary>
        /// Equivalent to Unity's update method.
        /// </summary>
        public virtual void WhileTrigger() { }
    }
}
