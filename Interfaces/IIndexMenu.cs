using System.Collections.Generic;
using WarWolfWorks.UI.MenusSystem.Assets;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface for implementing a Index-based Menu.
    /// </summary>
    public interface IIndexMenu
    {
        /// <summary>
        /// Current index of the menu.
        /// </summary>
        int MenuIndex { get; set; }

        /// <summary>
        /// All events of this <see cref="IIndexMenu"/>.
        /// </summary>
        IEnumerable<IndexEvent> Events { get; }
    }
}