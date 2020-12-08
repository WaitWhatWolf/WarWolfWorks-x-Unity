using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnDestroyQueued method similar to <see cref="Nyu.OnDestroyQueued"/>;
    /// Invoked right before <see cref="INyuOnDestroy"/> when <see cref="Nyu.RemoveNyuComponent{T}"/> is called, they are both executed
    /// consecutively however.
    /// </summary>
    public interface INyuOnDestroyQueued
    {
        /// <summary>
        /// Invoked when the parent <see cref="Nyu"/> is queued for destruction.
        /// </summary>
        void NyuOnDestroyQueued();
    }
}
