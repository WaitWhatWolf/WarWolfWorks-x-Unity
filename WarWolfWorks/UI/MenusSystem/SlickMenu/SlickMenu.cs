using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WarWolfWorks.Enums;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.SlickMenu
{
    /// <summary>
    /// The base class of the slick UI system; TParent is the parent type, TCell is the cell type.
    /// </summary>
    /// <typeparam name="TParent">The type of the menu itself.</typeparam>
    /// <typeparam name="TCell">The type of the cells serviced.</typeparam>
    /// <typeparam name="TBorder">The type of the border used.</typeparam>
    public abstract class SlickMenu<TParent, TCell, TBorder> : Menu, IIndexable, IRefreshable 
        where TParent : SlickMenu<TParent, TCell, TBorder>
        where TCell : SlickCell<TParent, TCell, TBorder>
        where TBorder : SlickBorder
	{
        /// <summary>
        /// The color theme of this menu.
        /// </summary>
        public virtual Color ThemeColor { get; } = Hooks.Colors.Tangelo;

		/// <summary>
		/// Navigation type of this menu; Used to make the menu visual-only or interactible.
		/// </summary>
		public virtual SlickNavigationType NavigationType { get; } = SlickNavigationType.None;

        /// <summary>
        /// Updates the UI to be up-to-date with back-end info for all it's cells.
        /// </summary>
        public virtual void Refresh()
        {
            foreach (TCell cell in GetRefreshCells())
            {
                cell.Refresh();
            }
        }

        /// <summary>
        /// The currently hovered cell index.
        /// </summary>
        public int Index
        {
            get => pr_Index;
            set
            {
                value = CellsRange.GetClampedValue(value);
                if (pr_Index == value)
                    return;

                pr_Index = value;
                Refresh();
            }
        }

        /// <summary>
        /// The currently selected cell index.
        /// </summary>
        public int SelectionIndex
        {
            get => pr_SelectionIndex;
            set
            {
                value = CellsRange.GetClampedValue(value);
                if (pr_SelectionIndex == value)
                    return;

                pr_SelectionIndex = value;
                Refresh();
            }
        }

        /// <summary>
        /// Called by the core when a navigation has been used.
        /// </summary>
        /// <param name="direction"></param>
        protected abstract void OnNavigationUse(Direction direction);

        /// <summary>
        /// Called by the core when a cell at <see cref="SelectionIndex"/> has been used.
        /// </summary>
        /// <param name="type">What type of selection was made. (0 = Left Mouse, 1 = Right Mouse, 2 = Mouse Middle, etc...)</param>
        protected abstract void OnSelectionAccepted(int type);

        /// <summary>
        /// Used by the core to determine which cells to use navigation on.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TCell> GetNavigationCells();
        /// <summary>
        /// Used by the core to determine which cells to refresh with a <see cref="Refresh"/> call.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TCell> GetRefreshCells();

        /// <summary>
        /// Clamps <see cref="Index"/> and <see cref="SelectionIndex"/> to this range.
        /// </summary>
        protected abstract IntRange CellsRange { get; }

        /// <summary>
        /// Builds the UI using <typeparamref name="TCell"/>. Called by core during <see cref="Start"/>.
        /// </summary>
        protected abstract IEnumerable<TCell> BuildUI();

        /// <summary>
        /// Destroys all UI elements initiated by <see cref="BuildUI"/>.
        /// </summary>
        protected abstract void DestroyUI();

        /// <summary>
        /// Creates a cell.
        /// </summary>
        /// <param name="index">Index to be assigned to the cell.</param>
        /// <param name="name">Name to be used for the cell.</param>
        /// <param name="args">Additional arguments.</param>
        /// <returns></returns>
        protected abstract TCell CreateCell(int index, LanguageString name, params object[] args);

        /// <summary>
		/// Creates a holder that is the child of the menu's holder.
		/// </summary>
		/// <param name="name">The name of the holder GameObject.</param>
		/// <returns></returns>
		protected virtual RectTransform CreateHolder(string name, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f, RectTransform holder = null)
        {
            GameObject obj_Holder = new GameObject(name);
            if (holder == null)
                holder = Holder;
            obj_Holder.transform.SetParent(holder);
            obj_Holder.transform.localScale = Vector3.one;
            RectTransform toReturn = obj_Holder.AddComponent<RectTransform>();
            toReturn.SetAnchoredUI(xMin, yMin, xMax, yMax);
            return toReturn;
        }

        /// <summary>
        /// Assigned on cell event triggers; Sets index to the cell's index by default.
        /// </summary>
        /// <param name="cell"></param>
        protected virtual void Event_Cell_OnPointerEnter(TCell cell)
            => Index = cell.Index;

        /// <summary>
        /// Assigned on cell event triggers. Sets index to -1 by default.
        /// </summary>
        /// <param name="cell"></param>
        protected virtual void Event_Cell_OnPointerExit(TCell cell)
            => Index = -1;

        /// <summary><inheritdoc/></summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Calls <see cref="BuildUI"/> and assigns their events; Make sure to include "base.Start();" when overriding.
        /// </summary>
        protected virtual void Start()
        {
            foreach (TCell cell in BuildUI())
            {
                Hooks.AddEventTriggerListener(cell.EventHandler, EventTriggerType.PointerEnter, (data) => Event_Cell_OnPointerEnter(cell));
                Hooks.AddEventTriggerListener(cell.EventHandler, EventTriggerType.PointerExit, (data) => Event_Cell_OnPointerExit(cell));
            }

            Refresh();
        }

        /// <summary>
        /// Handles navigation; Override without "base.Update();" if <see cref="NavigationType"/> is set to None.
        /// </summary>
        protected virtual void Update()
        {
            switch (NavigationType)
            {
                default: HandleNavigation(); break;
                case SlickNavigationType.None: break;
            }
        }

        /// <summary>
        /// Returns true if the menu is navigating up.
        /// </summary>
        protected abstract bool NavigatesUp();
        /// <summary>
        /// Returns true if the menu is navigating down.
        /// </summary>
        protected abstract bool NavigatesDown();
        /// <summary>
        /// Returns true if the menu is navigating left.
        /// </summary>
        protected abstract bool NavigatesLeft();
        /// <summary>
        /// Returns true if the menu is navigating right.
        /// </summary>
        protected abstract bool NavigatesRight();

        /// <summary>
        /// Returns true if the current navigation of this menu is accepted, or interacted with. (Should be based off of <see cref="NavigationType"/>)
        /// </summary>
        /// <param name="type">Returns the type based on the mouse button pressed, where 0 = Left Mouse, 1 = Right Mouse, 2 = Middle Mouse, etc...</param>
        /// <returns></returns>
        protected abstract bool NavigateAccepts(out int type);

        private void HandleNavigation()
        {
            if (NavigatesLeft())
                OnNavigationUse(Direction.Left);
            if (NavigatesRight())
                OnNavigationUse(Direction.Right);

            if (NavigatesUp())
                OnNavigationUse(Direction.Up);
            if (NavigatesDown())
                OnNavigationUse(Direction.Down);

            if (NavigateAccepts(out int type))
                OnSelectionAccepted(type);
        }

#pragma warning disable 1591
        protected int pr_Index = -1;
        protected int pr_SelectionIndex = -1;
#pragma warning restore 1591
    }
}
