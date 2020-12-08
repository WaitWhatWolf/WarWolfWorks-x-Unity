using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on an <see cref="NyuComponent"/> or sub-component to indicate that it has an OnEnable method.
    /// </summary>
    public interface INyuOnEnable
    {
        /// <summary>
        /// Invoked when the parent entity is enabled.
        /// </summary>
        void NyuOnEnable();
    }
}
