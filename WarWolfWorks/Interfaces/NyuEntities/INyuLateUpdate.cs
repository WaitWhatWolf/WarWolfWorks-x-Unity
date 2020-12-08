using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a LateUpdate method.
    /// </summary>
    public interface INyuLateUpdate
    {
        /// <summary>
        /// Invoked right before unity renders a frame on screen.
        /// </summary>
        void NyuLateUpdate();
    }
}
