using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace WarWolfWorks.Interfaces.UI.MenusSystem.SlickMenu
{
    /// <summary>
    /// Indicates a slick cell with an overlay graphic element.
    /// </summary>
    /// <typeparam name="T">The type of graphic used as overlay.</typeparam>
    /// <typeparam name="TCont">The type of data assigned to <typeparamref name="T"/>.</typeparam>
    public interface IOverlay<T, TCont> where T : Graphic
    {
        /// <summary>
        /// The graphic element.
        /// </summary>
        T Overlay { get; }
        /// <summary>
        /// The content assigned to <see cref="Overlay"/>.
        /// </summary>
        TCont OverlayContent { get; }
    }
}
