using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace WarWolfWorks.Interfaces.UI.MenusSystem.SlickMenu
{
    /// <summary>
    /// Indicates a slick cell with a front graphic element.
    /// </summary>
    /// <typeparam name="T">The type of graphic used as front.</typeparam>
    /// <typeparam name="TCont">The type of data assigned to <typeparamref name="T"/>.</typeparam>
    public interface IFront<T, TCont> where T : Graphic
    {
        /// <summary>
        /// The front graphic.
        /// </summary>
        T Front { get; }
        /// <summary>
        /// The content assigned to <see cref="Front"/>.
        /// </summary>
        TCont FrontContent { get; }
    }
}
