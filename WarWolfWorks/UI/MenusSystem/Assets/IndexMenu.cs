using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// Indexed menu which implements various utility for handling a Index-based menu. Uses <see cref="IndexEvent"/> to make menu choices.
    /// </summary>
    [CompleteNoS]
    public abstract class IndexMenu : Menu, IIndexMenu
    {
        private int ns_MenuIndex;
        /// <summary>
        /// Current index of this menu.
        /// </summary>
        public virtual int MenuIndex
        {
            get => ns_MenuIndex;
            set
            {
                int prev = ns_MenuIndex;
                ns_MenuIndex = InteractibleRange.GetClampedValue(value);
                OnIndexChanged(prev);
                for (int i = 0; i < s_Events.Count; i++)
                    s_Events[i].InternalOnIndexChanged();
            }
        }

        /// <summary>
        /// Returns how many <see cref="IndexEvent"/> are attached to this <see cref="IndexMenu"/>.
        /// </summary>
        /// <returns></returns>
        public int GetIndexEventCount() => s_Events.Count;

        /// <summary>
        /// Limits the <see cref="MenuIndex"/> to this range.
        /// </summary>
        protected virtual IntRange InteractibleRange => new IntRange(0, s_Events.Count);

        /// <summary>
        /// When this method returns true, <see cref="MenuIndex"/> will increase by 1.
        /// </summary>
        /// <returns></returns>
        protected abstract bool IncreasesIndex();
        /// <summary>
        /// When this method returns true, <see cref="MenuIndex"/> will decrease by 1.
        /// </summary>
        /// <returns></returns>
        protected abstract bool DecreasesIndex();
        /// <summary>
        /// When this method returns true, it will invoke <see cref="IndexEvent.OnActivate(object)"/> at the current index; Argument passed is 0.
        /// </summary>
        /// <returns></returns>
        protected abstract bool ActivatesIndexEvent();

        [FormerlySerializedAs("events"), SerializeField]
        private protected List<IndexEvent> s_Events;
        /// <summary>
        /// All events held/used by this menu.
        /// </summary>
        public IEnumerable<IndexEvent> Events => s_Events;

        /// <summary>
        /// Returns the <see cref="IndexEvent"/> at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IndexEvent this[int index] => s_Events[index];

        /// <summary>
        /// Invoked when <see cref="MenuIndex"/> is changed.
        /// </summary>
        /// <param name="previous"></param>
        protected abstract void OnIndexChanged(int previous);

        /// <summary>
        /// Adds a <see cref="IndexEvent"/> to this menu's events.
        /// </summary>
        public void AddIndexEvent(IndexEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            @event.Initiate(this, s_Events.Count);
            s_Events.Add(@event);
        }

        /// <summary>
        /// Adds a <see cref="IndexEvent"/> to this menu's events at the specified index.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="at"></param>
        public void AddIndexEvent(IndexEvent @event, int at)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            if(at > s_Events.Count)
            {
                AdvancedDebug.LogWarningFormat("Cannot add {0} as the index ({1}) given is above the allowed range ({2})", DEBUG_LAYER_WWW_INDEX, nameof(@event), at, s_Events.Count);
                return;
            }

            foreach(IndexEvent ie in s_Events)
            {
                if (ie.IndexInMenu >= at) ie.IndexInMenu++;
            }

            @event.Initiate(this, at);
            s_Events.Insert(at, @event);
        }

        /// <summary>
        /// Removes a <see cref="IndexEvent"/> from this menu's events.
        /// </summary>
        /// <param name="event"></param>
        public void RemoveIndexEvent(IndexEvent @event)
        {
            if(@event == null)
                throw new ArgumentNullException(nameof(@event));

            int @eventIndex = @event.IndexInMenu;
            @event.InternalOnRemove();
            s_Events.Remove(@event);

            foreach (IndexEvent ie in s_Events)
            {
                if (ie.IndexInMenu > eventIndex) ie.IndexInMenu--;
            }
        }

        /// <summary>
        /// Removes a <see cref="IndexEvent"/> from this menu's events.
        /// </summary>
        /// <param name="at"></param>
        public void RemoveIndexEvent(int at)
        {
            if (at < 0 || at > s_Events.Count)
                throw new IndexOutOfRangeException();

            s_Events[at].InternalOnRemove();
            s_Events.RemoveAt(at);

            foreach (IndexEvent ie in s_Events)
            {
                if (ie.IndexInMenu > at) ie.IndexInMenu--;
            }
        }

        /// <summary>
        /// Initiates all events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < s_Events.Count; i++)
                s_Events[i].Initiate(this, i);

            MenuIndex = 0;
        }

        /// <summary>
        /// Handles <see cref="IncreasesIndex"/> and <see cref="DecreasesIndex"/>; When overriding, make sure to include base.Update() if you want this to be implemented by default.
        /// </summary>
        protected virtual void Update()
        {
            if (IsActive)
            {
                if (IncreasesIndex()) MenuIndex++;
                if (DecreasesIndex()) MenuIndex--;
                if (ActivatesIndexEvent()) s_Events[MenuIndex].Activate(0);
            }
        }
    }
}
