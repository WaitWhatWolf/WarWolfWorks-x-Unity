using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a Start method.
    /// </summary>
    public interface INyuStart
    {
        /// <summary>
        /// Invoked after initialization.
        /// </summary>
        void NyuStart();
    }
}
