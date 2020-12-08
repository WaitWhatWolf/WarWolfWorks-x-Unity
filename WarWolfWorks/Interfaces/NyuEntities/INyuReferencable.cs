using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Indicates an object that uses a reference to an <see cref="Nyu"/>. (usually owner)
    /// </summary>
    public interface INyuReferencable
    {
        /// <summary>
        /// The owner.
        /// </summary>
        Nyu NyuMain { get; }
    }
}
