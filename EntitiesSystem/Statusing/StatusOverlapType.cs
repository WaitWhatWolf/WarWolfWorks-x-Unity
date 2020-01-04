using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Statusing
{
    /// <summary>
    /// Determines how the <see cref="IStatus"/> is handled when a <see cref="IStatus"/> of the same type was found inside a <see cref="IStatus"/>.
    /// </summary>
    public enum StatusOverlapType
    {
        /// <summary>
        /// Ignores this <see cref="IStatus"/> and doesn't add it.
        /// </summary>
        ignore,
        /// <summary>
        /// Adds it regardless.
        /// </summary>
        add,
        /// <summary>
        /// Replaces the current one with this one.
        /// </summary>
        replace,
    }
}
