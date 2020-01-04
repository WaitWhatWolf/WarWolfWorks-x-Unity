using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// Premade <see cref="IndexEvent"/> with basic utilities.
    /// </summary>
    public abstract class BasicIndexEvent : IndexEvent
    {
        /// <summary>
        /// Graphics assigned through the inspector.
        /// </summary>
        public List<MaskableGraphic> Graphics;

        /// <summary>
        /// Colors used with <see cref="OnFocused"/> and <see cref="OnUnfocused"/>.
        /// </summary>
        public Color FocusedColor = Color.yellow, UnfocusedColor = Color.cyan * .75f;

        /// <summary>
        /// Returns true if the mouse is currently inside this <see cref="IndexEvent"/>'s graphic.
        /// </summary>
        public bool MouseIsInside { get; private set; }

        /// <summary>
        /// Returns true if this <see cref="IndexEvent"/> is currently focused by it's parent though <see cref="IndexMenu.MenuIndex"/>.
        /// </summary>
        public bool Focused { get; private set; }

        /// <summary>
        /// Invokes this <see cref="IndexEvent"/>'s activation.
        /// </summary>
        protected override void EventOnPointerClick()
        {
            Activate(0);
        }

        /// <summary>
        /// Sets the index of the parent to this <see cref="IndexEvent"/>'s index; Make sure to include base.EventOnPointerEnter() at the start of the method when overriding
        /// to make this work properly.
        /// </summary>
        protected override void EventOnPointerEnter()
        {
            MouseIsInside = true;
            Parent.MenuIndex = IndexInMenu;
        }

        /// <summary>
        /// Make sure to include base.EventOnPointerExit() at the start of the method when overriding
        /// to make this work properly.
        /// </summary>
        protected override void EventOnPointerExit()
        {
            MouseIsInside = false;
        }

        /// <summary>
        /// Calls <see cref="OnFocused"/> or <see cref="OnUnfocused"/> based on the Parent's <see cref="IndexMenu.MenuIndex"/>.
        /// </summary>
        protected override void OnIndexChanged()
        {
            if (IndexInMenu == Parent.MenuIndex && !Focused)
                OnFocused();
            else if(Focused && IndexInMenu != Parent.MenuIndex) OnUnfocused();
        }

        /// <summary>
        /// Invoked when this <see cref="IndexEvent"/> is first focused.
        /// </summary>
        protected virtual void OnFocused()
        {
            for (int i = 0; i < Graphics.Count; i++)
            {
                Graphics[i].color = FocusedColor;
            }
            Focused = true;
        }

        /// <summary>
        /// Invoked when this <see cref="IndexEvent"/> looses focus.
        /// </summary>
        protected virtual void OnUnfocused()
        {
            for (int i = 0; i < Graphics.Count; i++)
            {
                Graphics[i].color = UnfocusedColor;
            }
            Focused = false;
        }
    }
}
