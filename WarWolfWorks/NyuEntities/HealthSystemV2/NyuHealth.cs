using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities.HealthSystemV2
{
    /// <summary>
    /// Base class used as the health component.
    /// </summary>
    public class NyuHealth : NyuComponent, INyuAwake, IHealth
    {
        /// <summary>
        /// The current health of this entity. (overridable)
        /// </summary>
        public virtual float CurrentHealth 
        { 
            get => pr_BaseCurrentHealth; 
            protected set => pr_BaseCurrentHealth = Hooks.MathF.Clamp(value, 0f, MaxHealth); 
        }

        /// <summary>
        /// The maximum amount that <see cref="CurrentHealth"/> can reach; By default, it does not set the value if it is at or below 0. (overridable)
        /// </summary>
        public virtual float MaxHealth 
        { 
            get => pr_BaseMaxHealth;
            protected set
            {
                if(value > 0f)
                    pr_BaseMaxHealth = value;
            }
        }

        /// <summary>
        /// Attempts to add the given amount to <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount">Amount used.</param>
        /// <param name="added">
        /// The added amount of health; 
        /// As an example, if <see cref="CurrentHealth"/> is at 980 and <see cref="MaxHealth"/> is at 1000,
        /// but the amount given is 100, the amount added will be 20 instead of 100.
        /// </param>
        /// <remarks>If the final amount added does not change health, it will return false.</remarks>
        /// <returns>True if addition was successful.</returns>
        public virtual bool AddHealth(float amount, out float added)
        {
            try
            {
                amount = amount.ToPositive();
                float prevHealth = CurrentHealth;
                CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
                added = CurrentHealth - prevHealth;

                return added > 0;
            }
            catch { added = 0; return false; }
        }

        /// <summary>
        /// Attempts to remove the given amount from <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount">Amount used.</param>
        /// <param name="removed">
        /// The removed amount of health; As an example, 
        /// if <see cref="CurrentHealth"/> is at 100 but amount given is 120, the amount removed will be 100 instead of 120.
        /// </param>
        /// <remarks>If the final amount removed does not change health, it will return false.</remarks>
        /// <returns>True if removal was successful.</returns>
        public virtual bool RemoveHealth(float amount, out float removed)
        {
            try
            {
                float useAmount = amount.ToPositive();
                removed = useAmount > CurrentHealth ? CurrentHealth : useAmount;
                CurrentHealth -= removed;
                return true;
            }
            catch { removed = 0; return false; }
        }

        /// <summary>
        /// Attempts to remove an amount from <see cref="CurrentHealth"/> based on <see cref="Damage.Final(NyuHealth)"/>.
        /// </summary>
        /// <param name="damage">The value used for removal; Any damage type is acceptable, but specific handling can only be done by classes which support it.</param>
        /// <param name="used">If successfully damaged, returns the final amount used to remove health.</param>
        /// <returns>0 if value removal was successful, -1 otherwise.</returns>
        public virtual int DamageHealth(Damage damage, out float used)
        {
            try
            {
                RemoveHealth(damage.Final(this), out used);
                PreviousDamage = damage;
                return 0;
            }
            catch
            {
                used = 0f;
                return -1;
            }
        }

        /// <summary>
        /// Sets the <see cref="CurrentHealth"/> to <see cref="MaxHealth"/>.
        /// </summary>
        public virtual void NyuAwake()
        {
            CurrentHealth = MaxHealth;
        }

        /// <summary>
        /// The previous damage used in <see cref="DamageHealth(Damage, out float)"/> which successfully triggered.
        /// </summary>
        public Damage PreviousDamage { get; protected set; }

        /// <summary>
        /// The base value used by <see cref="CurrentHealth"/>.
        /// </summary>
        protected float pr_BaseCurrentHealth;
        /// <summary>
        /// The base value used by <see cref="MaxHealth"/>.
        /// </summary>
        protected float pr_BaseMaxHealth;
    }
}
