using WarWolfWorks.EntitiesSystem;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface used for custom effect when an <see cref="Entity"/> enters immunity.
    /// </summary>
    public interface IImmunityEffect
    {
        /// <summary>
        /// This is invoked when immunity is first triggered.
        /// </summary>
        /// <param name="entity"></param>
        void OnTrigger(Entity entity);
        /// <summary>
        /// This is invoked when immunity ends.
        /// </summary>
        void OnEnd();
    }
}
