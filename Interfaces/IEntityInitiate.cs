using WarWolfWorks.EntitiesSystem;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used for basic initiation of an object.
    /// </summary>
    public interface IEntityInitiate : IEntity, IInitiated
    {
        /// <summary>
        /// Initiates it.
        /// </summary>
        void Init(Entity owner);
    }
}
