using WarWolfWorks.EntitiesSystem;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used to include a <see cref="Entity"/> component name EntityMain.
    /// Used by virtually every class in the WWWLibrary's premade Entity-related components.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public interface IEntity
    {
        /// <summary>
        /// Parent entity of this component.
        /// </summary>
        Entity EntityMain { get; }
    }
}