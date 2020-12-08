using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a FixedUpdate method.
    /// </summary>
    public interface INyuFixedUpdate
    {
        /// <summary>
        /// Called every physics frame.
        /// </summary>
        void NyuFixedUpdate();
    }
}
