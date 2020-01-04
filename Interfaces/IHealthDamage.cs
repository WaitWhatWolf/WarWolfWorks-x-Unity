using WarWolfWorks.EntitiesSystem;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface used to calcucate health for a Health system.
    /// </summary>
    public interface IHealthDamage
    {
        /// <summary>
        /// Value that will be used to remove health with.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="entity"></param>
        /// <param name="triggerImmunity"></param>
        /// <returns></returns>
        float FinalValue(object damage, IAdvancedHealth entity, out bool triggerImmunity);

        /// <summary>
        /// Determines if the value type passed is correct.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        bool AcceptableValue(object damage);
    }
}
