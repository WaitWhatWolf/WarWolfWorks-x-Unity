using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem
{
    /// <summary>
    /// Base class to use with <see cref="EntityHealth"/>.
    /// </summary>
    public abstract class HealthDamage : ScriptableObject, IHealthDamage
    {
        /// <summary>
        /// Should determine if the type of object is correct.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public abstract bool AcceptableValue(object damage);
        /// <summary>
        /// Returns the final calculated value.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="entity"></param>
        /// <param name="triggersImmunity"></param>
        /// <returns></returns>
        public abstract float FinalValue(object damage, EntityHealth entity, out bool triggersImmunity);

        /// <summary>
        /// Returns the final calculated value.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="health"></param>
        /// <param name="triggersImmunity"></param>
        /// <returns></returns>
        float IHealthDamage.FinalValue(object damage, IAdvancedHealth health, out bool triggersImmunity) => FinalValue(damage, (EntityHealth)health, out triggersImmunity);
    }
}
