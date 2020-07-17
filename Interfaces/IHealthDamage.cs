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
        /// Value that will be used to add health.
        /// </summary>
        /// <param name="heal"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        float FinalHeal(object heal, IAdvancedHealth entity);

        /// <summary>
        /// Determines if the value passed is correct for damage.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        bool AcceptableValue(object damage);
        /// <summary>
        /// Determines if the value passed is correct for a heal.
        /// </summary>
        /// <param name="heal"></param>
        /// <returns></returns>
        bool AcceptableHealValue(object heal);
    }
}
