using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Statusing
{
    /// <summary>
    /// Determines how the <see cref="IStatus"/> is handled when a <see cref="IStatus"/> of the same type was found inside a <see cref="IStatus"/>.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
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
