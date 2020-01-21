using WarWolfWorks.EntitiesSystem;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used by <see cref="Entity"/>'s EntityComponent management. When using this,
    /// make sure not to use original MonoBehaviour methods and to add it into
    /// an <see cref="Entity"/> through <see cref="Entity.InternalAddComponent(IEntityComponent)"/>
    /// when the component is created/instantiated.
    /// </summary>
    public interface IEntityComponent : IEntity, ICoroutinable, IAwake, IStart, IEnableDisable, IUpdate, IFixedUpdate, IDestroy
    {
        
    }
}
