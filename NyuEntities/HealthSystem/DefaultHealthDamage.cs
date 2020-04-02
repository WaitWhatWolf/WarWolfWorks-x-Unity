using WarWolfWorks.Interfaces;

namespace WarWolfWorks.NyuEntities.HealthSystem
{
    /// <summary>
    /// Default health damage set on <see cref="NyuHealth"/> when <see cref="NyuHealth.Calculator"/> is null. 
    /// Uses <see cref="float"/> as it's base type.
    /// </summary>
    public class DefaultHealthDamage : HealthDamage
    {
        /// <summary>
        /// Checks if the value given is a float value.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public override bool AcceptableValue(object damage) => damage is float;

        /// <summary>
        /// Returns the value given in float.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="entity"></param>
        /// <param name="triggersImmunity"></param>
        /// <returns></returns>
        public override float FinalValue(object damage, NyuHealth entity, out bool triggersImmunity)
        {
            triggersImmunity = true;
            return (float)damage;
        }
    }
}
