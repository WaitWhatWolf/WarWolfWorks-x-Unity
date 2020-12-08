using UnityEngine.UI;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// Group used by <see cref="AdvancedIndexEvent.EventGraphics"/>.
    /// </summary>
    [System.Serializable]
    public struct EventGraphicGroup
    {
        /// <summary>
        /// The graphic affected.
        /// </summary>
        public Graphic AffectedGraphic;
        /// <summary>
        /// The event used.
        /// </summary>
        public EventGraphic[] Events;

        /// <summary>
        /// Creates a new <see cref="EventGraphicGroup"/>.
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="eventGraphics"></param>
        public EventGraphicGroup(Graphic graphic, EventGraphic[] eventGraphics)
        {
            AffectedGraphic = graphic;
            Events = eventGraphics;
        }

        internal EventGraphicGroup(AdvancedIndexEvent parent, EventGraphicGroup copy)
        {
            AffectedGraphic = copy.AffectedGraphic;
            Events = Hooks.Enumeration.InstantiateList(copy.Events);
            foreach (EventGraphic eg in Events)
            {
                eg.Parent = parent;
                eg.AffectedGraphic = AffectedGraphic;
            }
        }
    }
}
