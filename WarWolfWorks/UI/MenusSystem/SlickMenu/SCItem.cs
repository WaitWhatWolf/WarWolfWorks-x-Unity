using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarWolfWorks.Security;

namespace WarWolfWorks.UI.MenusSystem.SlickMenu
{
    /// <summary>
    /// A <see cref="SlickCell{TParent, TCell, TBorder}"/> with an additional image on top, and a text;
    /// Very useful for inventories.
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TCell"></typeparam>
    /// <typeparam name="TBorder"></typeparam>
    public abstract class SCItem<TParent, TCell, TBorder> : SlickCell<TParent, TCell, TBorder>
        where TParent : SlickMenu<TParent, TCell, TBorder>
        where TCell : SCItem<TParent, TCell, TBorder>
        where TBorder : SlickBorder
    {
        /// <summary>
        /// The text of this cell. (Ordered in front of <see cref="Front"/> by default)
        /// </summary>
        public TextMeshProUGUI Text { get; }

        /// <summary>
        /// The front image of this cell.
        /// </summary>
        public Image Front { get; }
        
        /// <summary>
        /// The sprite that is applied to <see cref="Front"/> on refresh.
        /// </summary>
        public abstract Sprite FrontContent { get; }

        /// <summary>
        /// The string that is applied to <see cref="Text"/> on refresh.
        /// </summary>
        public abstract string TextContent { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Refresh()
        {
            Selection = GetSelectionType();
            Background.color = GetSelectionColor();
            Front.sprite = FrontContent;
            Text.text = TextContent;
        }

        /// <summary>
        /// Extension of the base constructor to initiate all other cell info.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="eventHandler"></param>
        /// <param name="background"></param>
        /// <param name="front"></param>
        /// <param name="text"></param>
        public SCItem(TParent parent, int index, EventTrigger eventHandler, Image background, Image front, TextMeshProUGUI text)
            : base(parent, index, eventHandler, background)
        {
            Front = front ? front : throw new WWWException(45, this, "front field given on constructor is null.");
            Front.transform.SetAsLastSibling();
            Text = text ? text : throw new WWWException(45, this, "text field given on constructor is null.");
            Text.transform.SetAsLastSibling();
        }
    }
}
