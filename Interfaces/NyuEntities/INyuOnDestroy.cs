using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnDestroy method; Invoked when either the
    /// parent entity is destroyed, or when <see cref="Nyu.RemoveNyuComponent{T}"/> is called.
    /// </summary>
    public interface INyuOnDestroy
    {
        /// <summary>
        /// Invoked right before this INyuOnDestroy is about to be removed.
        /// </summary>
        void NyuOnDestroy();
    }
}
