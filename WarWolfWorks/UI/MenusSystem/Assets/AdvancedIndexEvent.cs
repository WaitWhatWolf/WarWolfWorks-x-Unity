using System.Collections.Generic;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// A more advanced version of <see cref="BasicIndexEvent"/>.
    /// </summary>
    public abstract class AdvancedIndexEvent : IndexEvent, IFocusable
    {
        /// <summary>
        /// Event graphics used by this index event.
        /// </summary>
        public List<EventGraphicGroup> EventGraphics = new List<EventGraphicGroup>();

        /// <summary>
        /// Returns true if the mouse is currently inside this <see cref="IndexEvent"/>'s graphic.
        /// </summary>
        public bool MouseIsInside { get; private set; }

        /// <summary>
        /// Returns true if this <see cref="IndexEvent"/> is currently focused by it's parent though <see cref="IndexMenu.MenuIndex"/>.
        /// </summary>
        public bool IsFocused { get; private set; }

        /// <summary>
        /// When overriding, make sure to include "base.Awake();" as it invokes the <see cref="IAwake"/> method of all
        /// event graphics of this <see cref="AdvancedIndexEvent"/>.
        /// </summary>
        protected virtual void Awake()
        {
            for (int i = 0; i < EventGraphics.Count; i++)
            {
                EventGraphics[i] = new EventGraphicGroup(this, EventGraphics[i]);
                
                for(int j = 0; j < EventGraphics[i].Events.Length; j++)
                    if (EventGraphics[i].Events[j] is IAwake iAwake)
                        iAwake.Awake();
            }
        }

        /// <summary>
        /// Invokes this <see cref="IndexEvent"/>'s activation.
        /// </summary>
        protected override void EventOnPointerClick()
        {
            Activate(0);
        }

        /// <summary>
        /// Sets the index of the parent to this <see cref="IndexEvent"/>'s index; Make sure to include "base.EventOnPointerEnter();" at the start of the method when overriding
        /// to make this <see cref="AdvancedIndexEvent"/> work properly.
        /// </summary>
        protected override void EventOnPointerEnter()
        {
            MouseIsInside = true;
            Parent.MenuIndex = IndexInMenu;
        }

        /// <summary>
        /// Make sure to include "base.EventOnPointerExit();" at the start of the method when overriding
        /// to make this <see cref="AdvancedIndexEvent"/> work properly.
        /// </summary>
        protected override void EventOnPointerExit()
        {
            MouseIsInside = false;
        }

        /// <summary>
        /// When overriding, make sure to include "base.Update();" as it calls all <see cref="IUpdate"/> components of <see cref="EventGraphics"/>.
        /// </summary>
        protected virtual void Update()
        {
            for(int i = 0; i < EventGraphics.Count; i++)
            {
                for (int j = 0; j < EventGraphics[i].Events.Length; j++)
                    if (EventGraphics[i].Events[j] is IUpdate iUpdate)
                        iUpdate.Update();
            }
        }

        /// <summary>
        /// Calls OnFocused or OnUnfocused of all event graphics based on the Parent's <see cref="IndexMenu.MenuIndex"/>.
        /// </summary>
        protected override void OnIndexChanged()
        {
            if (IndexInMenu == Parent.MenuIndex && !IsFocused)
            {
                IsFocused = true;

                for (int i = 0; i < EventGraphics.Count; i++)
                {
                    for (int j = 0; j < EventGraphics[i].Events.Length; j++)
                        if (EventGraphics[i].Events[j] is IOnFocus iOnFocus)
                            iOnFocus.Focus();
                }
            }
            else if (IsFocused && IndexInMenu != Parent.MenuIndex)
            {
                IsFocused = false;

                for (int i = 0; i < EventGraphics.Count; i++)
                {
                    for (int j = 0; j < EventGraphics[i].Events.Length; j++)
                        if (EventGraphics[i].Events[j] is IOnUnfocus iOnUnfocus)
                            iOnUnfocus.Unfocus();
                }
            }
        }

        /// <summary>
        /// Invoked when this index event is focused.
        /// </summary>
        protected virtual void OnFocus() { }
        /// <summary>
        /// Invoked when this index event is unfocused.
        /// </summary>
        protected virtual void OnUnfocus() { }
    }
}
