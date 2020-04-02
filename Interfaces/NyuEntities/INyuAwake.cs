using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a Awake method, the equivalent of Unity's Awake method.
    /// </summary>
    public interface INyuAwake
    {
        /// <summary>
        /// Invoked when the parent <see cref="Nyu"/> is initiated.
        /// </summary>
        void NyuAwake();
    }
}
