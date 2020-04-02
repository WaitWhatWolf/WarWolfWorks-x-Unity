using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem
{
    /// <summary>
    /// <see cref="ScriptableObject"/> used for custom effect when an <see cref="Entity"/> enters immunity through <see cref="EntityHealth"/>.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public abstract class ImmunityEffect : ScriptableObject, IImmunityEffect<EntityHealth>
    {
        /// <summary>
        /// Duration at which the current immunity will last.
        /// </summary>
        public float ImmunityTime { get; internal set; }

        /// <summary>
        /// The countdown/cooldown of the current immunity timer; To be used with ImmunityTime for percentage duration.
        /// </summary>
        public float ImmunityCountdown { get; internal set; }

        internal EntityHealth internalParent;
        /// <summary>
        /// Parent of this <see cref="ImmunityEffect"/>.
        /// </summary>
        public EntityHealth Parent => internalParent;

        /// <summary>
        /// What happens when Immunity first triggers.
        /// </summary>
        public abstract void OnTrigger();

        /// <summary>
        /// What happens when Immunity stops.
        /// </summary>
        public abstract void OnEnd();

        /// <summary>
        /// What happens when Immunity is either added to a <see cref="EntityHealth"/>, or when it was already attached to it on instantiation.
        /// </summary>
        public abstract void OnAdded();

        /// <summary>
        /// Equivalent to Unity's update method.
        /// </summary>
        public virtual void WhileTrigger() { }
    }
}
