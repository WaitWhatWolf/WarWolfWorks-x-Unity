using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a Update method.
    /// </summary>
    public interface INyuUpdate
    {
        /// <summary>
        /// Invoked every in-game frame.
        /// </summary>
        void NyuUpdate();
    }
}
