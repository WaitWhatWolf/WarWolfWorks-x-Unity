using WarWolfWorks.EntitiesSystem;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used for basic initiation of an entity system object.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public interface IEntityInitiate : IEntity, IInitiated
    {
        /// <summary>
        /// Initiates it.
        /// </summary>
        void Init(Entity owner);
    }
}
