using System;

namespace WarWolfWorks.NyuEntities.YharonSystem
{
    /// <summary>
    /// Determines the behavior of <see cref="Yharon"/> when a yharon of an existing type is added to a <see cref="NyuYharon"/>.
    /// </summary>
    [Flags]
    public enum YharonApplication
    {
        /// <summary>
        /// Ignores the <see cref="Yharon"/> that attempts to be added.
        /// </summary>
        Ignore = 0,
        /// <summary>
        /// Adds it.
        /// </summary>
        Add = 4,
        /// <summary>
        /// Calls the existing <see cref="Yharon"/>'s OnOverride method.
        /// </summary>
        Override = 8,
        /// <summary>
        /// Removes the existing one.
        /// </summary>
        Remove = 16,
    }
}
