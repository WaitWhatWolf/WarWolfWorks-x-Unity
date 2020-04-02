using System;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Statusing
{
    /// <summary>
    /// Class used to give resistance towards <see cref="IStatus"/>es.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public sealed class Resistance
    {
        /// <summary>
        /// Type of <see cref="IStatus"/> which this resistance is affecting.
        /// </summary>
        public Type ResistantTo { get; private set; }

        internal Action<EntityStatusApplier> OnAdded { get; set; }
        internal Action<EntityStatusApplier> OnRemoved { get; set; }

        /// <summary>
        /// Instantiate a new resistance class.
        /// </summary>
        /// <param name="to"></param>
        public Resistance(Type to)
        {
            ResistantTo = to;
        }

        /// <summary>
        /// Instantiate a new resistance class with actions performed when added and removed from a <see cref="EntityStatusApplier"/>. (can be null)
        /// </summary>
        /// <param name="to"></param>
        /// <param name="onadded"></param>
        /// <param name="onremoved"></param>
        public Resistance(Type to, Action<EntityStatusApplier> onadded, Action<EntityStatusApplier> onremoved)
        {
            ResistantTo = to;
            OnAdded = onadded;
            OnRemoved = onremoved;
        }
    }
}
