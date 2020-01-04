using System;
using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// An advanced version of <see cref="IHealth"/>.
    /// </summary>
    public interface IAdvancedHealth : IHealth
    {
        /// <summary>
        /// Should be invoked inside <see cref="DamageHealth(object)"/>.
        /// </summary>
        event Action<IAdvancedHealth, object, IHealthDamage> OnDamaged;
        /// <summary>
        /// Immunity duration in seconds.
        /// </summary>
        float ImmunityDuration { get; }
        /// <summary>
        /// Defense applies to Damage calculation.
        /// </summary>
        float Defense { get; }
        /// <summary>
        /// Determines if this instance implements Immunity.
        /// </summary>
        bool UsesImmunity { get; }
        /// <summary>
        /// Should determine when the immunity is triggered or not.
        /// </summary>
        bool IsImmune { get; }
#if WWW2_5_OR_HIGHER
        /// <summary>
        /// Separate component which should calculate the final value in <see cref="DamageHealth(object)"/>.
        /// </summary>
        IHealthDamage Calculator { get; set; }
#endif
        /// <summary>
        /// Separate component which should be used with the immunity system.
        /// </summary>
        IImmunityEffect ImmunityEffect { get; set; }
        /// <summary>
        /// What calculates health and triggers all other events in this interface.
        /// </summary>
        /// <param name="damage"></param>
        void DamageHealth(object damage);
        /// <summary>
        /// Method to trigger immunity.
        /// </summary>
        void TriggerImmunity(float @for);
        /// <summary>
        /// Stops an immunity triggered with <see cref="TriggerImmunity(float)"/>.
        /// </summary>
        void StopImmunity();
	}
}