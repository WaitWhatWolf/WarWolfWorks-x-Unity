using UnityEngine;
using UnityEngine.EventSystems;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// Inherit this class to make <see cref="IndexMenu"/> events like play, options, exit, etc...
    /// </summary>
    public abstract class IndexEvent : MonoBehaviour
    {
        /// <summary>
        /// Menu which uses this event.
        /// </summary>
        public IndexMenu Parent { get; internal set; }

        /// <summary>
        /// Index if this event inside it's parent.
        /// </summary>
        public int IndexInMenu { get; internal set; }

        /// <summary>
        /// Unity EventTrigger component attached to this IndexEvent. 
        /// </summary>
        protected EventTrigger GUIEvent { get; private set; }

        internal void Initiate(IndexMenu parent, int index)
        {
            Parent = parent;
            IndexInMenu = index;
            GUIEvent = gameObject.AddComponent<EventTrigger>();
            Hooks.AddEventTriggerListener(GUIEvent, EventTriggerType.PointerClick, e => EventOnPointerClick());
            Hooks.AddEventTriggerListener(GUIEvent, EventTriggerType.PointerEnter, e => EventOnPointerEnter());
            Hooks.AddEventTriggerListener(GUIEvent, EventTriggerType.PointerExit, e => EventOnPointerExit());
        }

        internal void Activate(object arg)
            => OnActivate(arg);

        /// <summary>
        /// Added on this <see cref="GUIEvent"/> PointerClick event.
        /// </summary>
        protected abstract void EventOnPointerClick();
        /// <summary>
        /// Added on this <see cref="GUIEvent"/> PointerEnter event.
        /// </summary>
        protected abstract void EventOnPointerEnter();

        internal void InternalOnIndexChanged() => OnIndexChanged();

        /// <summary>
        /// Added on this <see cref="GUIEvent"/> PointerExit event.
        /// </summary>
        protected abstract void EventOnPointerExit();
        /// <summary>
        /// Invoked when the index of the parent menu is changed.
        /// </summary>
        protected abstract void OnIndexChanged();

        /// <summary>
        /// Invoked when this menu is activated/called through the game.
        /// </summary>
        /// <param name="arg"></param>
        protected abstract void OnActivate(object arg);
        internal void InternalOnRemove() => OnRemove();
        /// <summary>
        /// Invoked when this menu is removed from it's parent.
        /// </summary>
        protected virtual void OnRemove() { }

    }
}
