using WarWolfWorks.EntitiesSystem;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used by <see cref="Entity"/>'s EntityComponent management. When using this,
    /// make sure not to use original MonoBehaviour methods and to add it into
    /// an <see cref="Entity"/> through <see cref="Entity.InternalAddComponent(IEntityComponent)"/>
    /// when the component is created/instantiated.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public interface IEntityComponent : IEntity, ICoroutinable, IAwake, IStart, IOnEnableDisable, IUpdate, IFixedUpdate, IOnDestroy
    {
        
    }
}
