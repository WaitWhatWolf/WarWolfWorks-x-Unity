using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// Indexed menu which implements various utility for handling a Index-based menu. Uses <see cref="IndexEvent"/> to make menu choices.
    /// </summary>
    public abstract class IndexMenu : Menu, IIndexMenu
    {
        private int actIndex;
        /// <summary>
        /// Current index of this menu.
        /// </summary>
        public virtual int MenuIndex
        {
            get => actIndex;
            set
            {
                int prev = actIndex;
                actIndex = InteractibleRange.GetClampedValue(value);
                OnIndexChanged(prev);
                for (int i = 0; i < events.Count; i++)
                    events[i].InternalOnIndexChanged();
            }
        }

        /// <summary>
        /// Returns how many <see cref="IndexEvent"/> are attached to this <see cref="IndexMenu"/>.
        /// </summary>
        /// <returns></returns>
        public int GetIndexEventCount() => events.Count;

        /// <summary>
        /// Limits the <see cref="MenuIndex"/> to this range.
        /// </summary>
        protected virtual IntRange InteractibleRange => new IntRange(0, events.Count);

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

        [SerializeField]
        private protected List<IndexEvent> events;
        /// <summary>
        /// All events held/used by this menu.
        /// </summary>
        public IEnumerable<IndexEvent> Events => events;

        /// <summary>
        /// Returns the <see cref="IndexEvent"/> at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IndexEvent this[int index] => events[index];

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

            @event.Initiate(this, events.Count);
            events.Add(@event);
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

            if(at > events.Count)
            {
                AdvancedDebug.LogWarningFormat("Cannot add {0} as the index ({1}) given is above the allowed range ({2})", AdvancedDebug.WWWInfoLayerIndex, nameof(@event), at, events.Count);
                return;
            }

            foreach(IndexEvent ie in events)
            {
                if (ie.IndexInMenu >= at) ie.IndexInMenu++;
            }

            @event.Initiate(this, at);
            events.Insert(at, @event);
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
            events.Remove(@event);

            foreach (IndexEvent ie in events)
            {
                if (ie.IndexInMenu > eventIndex) ie.IndexInMenu--;
            }
        }

        /// <summary>
        /// Initiates all events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < events.Count; i++)
                events[i].Initiate(this, i);
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
                if (ActivatesIndexEvent()) events[MenuIndex].Activate(0);
            }
        }
    }
}
