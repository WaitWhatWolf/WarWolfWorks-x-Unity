﻿using TMPro;
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
    public abstract class SCText<TParent, TCell, TBorder> : SlickCell<TParent, TCell, TBorder>
        where TParent : SlickMenu<TParent, TCell, TBorder>
        where TCell : SCText<TParent, TCell, TBorder>
        where TBorder : SlickBorder
    {
        /// <summary>
        /// The string that is applied to <see cref="Text"/> on refresh.
        /// </summary>
        public abstract string TextContent { get; }
        /// <summary>
        /// The text of this cell.
        /// </summary>
        public TextMeshProUGUI Text { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Refresh()
        {
            Selection = GetSelectionType();
            Background.color = GetSelectionColor();
            Text.text = TextContent;
        }

        /// <summary>
        /// Extension of the base constructor to initiate all other cell info.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="eventHandler"></param>
        /// <param name="background"></param>
        /// <param name="text"></param>
        public SCText(TParent parent, int index, EventTrigger eventHandler, Image background, TextMeshProUGUI text)
            : base(parent, index, eventHandler, background)
        {
            Text = text ? text : throw new WWWException(45, this, "text field given on constructor is null.");
            Text.transform.SetAsLastSibling();
        }
    }
}
