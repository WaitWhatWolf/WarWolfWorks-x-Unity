using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarWolfWorks.Enums;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Security;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.UI.MenusSystem.SlickMenu
{
    /// <summary>
    /// Used with <see cref="SlickMenu{TParent, TCell, TBorder}"/> to group up and manipulate each "cell" or simply menu option.
    /// </summary>
    /// <typeparam name="TParent">The type of the menu itself.</typeparam>
    /// <typeparam name="TCell">The type of the cells serviced.</typeparam>
    /// <typeparam name="TBorder">The type of the border used.</typeparam>
    public abstract class SlickCell<TParent, TCell, TBorder> : IParentable<TParent>, IIndexable, IRefreshable 
        where TParent : SlickMenu<TParent, TCell, TBorder>
        where TCell : SlickCell<TParent, TCell, TBorder>
        where TBorder : SlickBorder
    {
        /// <summary>
        /// The current selection of this cell.
        /// </summary>
        public SlickSelectionType Selection;

        /// <summary>
        /// The parent of this cell.
        /// </summary>
        public TParent Parent { get; }
        /// <summary>
        /// Pointer to the <see cref="Background"/> <see cref="GameObject"/>.
        /// </summary>
        public GameObject Core { get; }
        /// <summary>
        /// Pointer to the <see cref="Background"/> <see cref="RectTransform"/>.
        /// </summary>
        public RectTransform Rect { get; }

        /// <summary>
        /// The <see cref="SlickBorder"/> added to the <see cref="Core"/> when this <see cref="SlickCell{TParent, TCell, TBorder}"/>'s constructor is called.
        /// Note: The border is not instantiated and remains null if <see cref="BorderFlags"/> is equal to <see cref="SlickBorderFlags.None"/>.
        /// </summary>
        public TBorder Border { get; }

        /// <summary>
        /// The event trigger which handles mouse navigation.
        /// </summary>
        public EventTrigger EventHandler { get; }

        /// <summary>
        /// The background of this cell.
        /// </summary>
        public Image Background { get; }
        
        /// <summary>
        /// The index of this cell within it's parent.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Flags applied to the slick border of this cell.
        /// </summary>
        public virtual SlickBorderFlags BorderFlags => SlickBorderFlags.All;

        /// <summary>
        /// Refreshes this cell to be up-to-date with back-end info. (Called by the parent)
        /// </summary>
        public virtual void Refresh()
        {
            Selection = GetSelectionType();
            Background.color = GetSelectionColor(UI_SLICK_BACK_TRANSPARENCY);
        }

        /// <summary>
        /// Standard constructor of the slick cell.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="eventHandler"></param>
        /// <param name="background"></param>
        public SlickCell(TParent parent, int index, EventTrigger eventHandler, Image background)
        {
            if (eventHandler == null)
                throw new SlickMenuException(70, this, "EventHandler given is null.");

            if (background == null)
                throw new SlickMenuException(70, this, "Background given is null.");

            Parent = parent;
            Index = index;

            EventHandler = eventHandler;

            Background = background;

            Core = Background.gameObject;
            Rect = Background.rectTransform;

            if (BorderFlags != SlickBorderFlags.None)
            {
                Border = Core.AddComponent<TBorder>();
                Border.Flags = BorderFlags;
                Border.Color = ThemeColor;
                Border.Init();
            }
        }

        /// <summary>
        /// The color theme if this cell; Points to the parent's ThemeColor by default.
        /// </summary>
        protected virtual Color ThemeColor => Parent.ThemeColor;

        #region Utility
        /// <summary>
        /// Returns the <see cref="SlickSelectionType"/> enum based on index values given.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <param name="hoverIndex">The current hover index of the parent.</param>
        /// <param name="selectIndex">The current select index of the parent.</param>
        /// <returns></returns>
        public static SlickSelectionType GetSelectionType(int index, int hoverIndex, int selectIndex)
        {
            if (index == hoverIndex && index == selectIndex)
                return SlickSelectionType.SelectedHovered;
            else if (index == selectIndex)
                return SlickSelectionType.Selected;
            else if (index == hoverIndex)
                return SlickSelectionType.Hovered;

            return SlickSelectionType.Unselected;
        }

        /// <summary>
        /// Returns the <see cref="SlickSelectionType"/> enum based on index values of this cell and it's parent;
        /// Uses the static <see cref="GetSelectionType(int, int, int)"/> as base.
        /// </summary>
        /// <returns></returns>
        public SlickSelectionType GetSelectionType() => GetSelectionType(Index, Parent.Index, Parent.SelectionIndex);

        /// <summary>
        /// Gets the appropriate color based on a menu's selection.
        /// </summary>
        /// <param name="color">The base color.</param>
        /// <param name="alpha">The transparency of the color.</param>
        /// <returns></returns>
        public Color GetSelectionColor(Color color, float alpha)
        {
            Color toReturn = Selection switch
            {
                SlickSelectionType.Unselected => Hooks.Colors.MiddleMan(color, Hooks.Colors.Black, 0.75f),
                SlickSelectionType.Hovered => Hooks.Colors.MiddleMan(color, Hooks.Colors.Black, 0.35f),
                SlickSelectionType.Selected => Hooks.Colors.MiddleMan(color, Hooks.Colors.Black, 0.15f),
                _ => color
            };

            return new Color(toReturn.r, toReturn.g, toReturn.b, alpha);
        }

        /// <summary>
        /// Gets the appropriate color based on a menu's selection. Color used is <see cref="ThemeColor"/>.
        /// </summary>
        /// <param name="alpha">The transparency of the color.</param>
        /// <returns></returns>
        public Color GetSelectionColor(float alpha)
            => GetSelectionColor(ThemeColor, alpha);

        /// <summary>
        /// Gets the appropriate color based on a menu's selection. Color used is <see cref="ThemeColor"/>, Alpha used is <see cref="UI_SLICK_BACK_TRANSPARENCY"/>.
        /// </summary>
        /// <returns></returns>
        public Color GetSelectionColor()
            => GetSelectionColor(ThemeColor, UI_SLICK_BACK_TRANSPARENCY);
        #endregion
    }
}
