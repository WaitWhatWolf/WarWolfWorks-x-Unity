using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/> or sub-component to indicate that it has an OnDisable method.
    /// </summary>
    public interface INyuOnDisable
    {
        /// <summary>
        /// Invoked when the parent entity is disabled.
        /// </summary>
        void NyuOnDisable();
    }
}
