using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities.HealthSystemV2
{
    /// <summary>
    /// A nyu health with an immunity functionnality.
    /// </summary>
    public class NyuImmunitableHealth : NyuHealth, INyuUpdate
    {
        /// <summary>
        /// Length of the immunity in seconds.
        /// </summary>
        public virtual float ImmunityTime { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int DamageHealth(Damage damage, out float used)
        {
            used = 0f;
            if (pr_CurrentImmunityTime > 0)
                return -1;

            try
            {
                RemoveHealth(damage.Final(this), out used);
                if (damage is ImmunitableDamage immunitableDamage && immunitableDamage.TriggersImmunity)
                    TriggerImmunity();
            }
            catch
            {
                return -1;
            }

            return 0;
        }
        
        /// <summary>
        /// Triggers the immunity, or more specifically, set the immunity timer to <see cref="ImmunityTime"/>.
        /// </summary>
        protected void TriggerImmunity()
        {
            pr_CurrentImmunityTime = ImmunityTime;
        }

        /// <summary>
        /// Counts down the immunity, Called automatically; When overriding, make sure to include "base.NyuUpdate();" otherwise
        /// immunity time will be infinite.
        /// </summary>
        public virtual void NyuUpdate()
        {
            pr_CurrentImmunityTime = Hooks.MathF.Clamp(pr_CurrentImmunityTime - Time.deltaTime, 0f, ImmunityTime);
        }

#pragma warning disable CS1591
        #region Unity Serialized
        [SerializeField, Tooltip("The base immunity time.")]
        protected float pr_BaseImmunityTime;
        #endregion
#pragma warning restore

        /// <summary>
        /// The current immunity time left.
        /// </summary>
        protected float pr_CurrentImmunityTime;
    }
}
