using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarWolfWorks.Security;

namespace WarWolfWorks.UI.MenusSystem.SlickMenu
{
    /// <summary>
    /// A slick cell which contains a text field on top.
    /// </summary>
    /// <typeparam name="TParent">The type of the menu itself.</typeparam>
    /// <typeparam name="TCell">The type of the cells serviced.</typeparam>
    /// <typeparam name="TBorder">The type of the border used.</typeparam>
    public abstract class SCFront<TParent, TCell, TBorder> : SlickCell<TParent, TCell, TBorder>
        where TParent : SlickMenu<TParent, TCell, TBorder>
        where TCell : SCFront<TParent, TCell, TBorder>
        where TBorder : SlickBorder
    {
        /// <summary>
        /// The front image of this cell.
        /// </summary>
        public Image Front { get; }

        /// <summary>
        /// The sprite that is applied to <see cref="Front"/> on refresh.
        /// </summary>
        public abstract Sprite FrontContent { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Refresh()
        {
            Selection = GetSelectionType();
            Background.color = GetSelectionColor();
            Front.sprite = FrontContent;
        }

        /// <summary>
        /// Extension of the base constructor to initiate all other cell info.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="eventHandler"></param>
        /// <param name="background"></param>
        /// <param name="front"></param>
        public SCFront(TParent parent, int index, EventTrigger eventHandler, Image background, Image front)
            : base(parent, index, eventHandler, background)
        {
            Front = front ? front : throw new WWWException(45, this, "front field given on constructor is null.");
        }
    }
}
