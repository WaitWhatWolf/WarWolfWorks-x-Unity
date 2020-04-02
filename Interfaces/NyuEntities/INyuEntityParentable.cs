using System;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Interface used for parent-like behaviour for entities and their components.
    /// </summary>
    public interface INyuEntityParentable
    {
        /// <summary>
        /// The entity's parent.
        /// </summary>
        Nyu NyuParent { get; set; }

        /// <summary>
        /// Invokes when the Parent is set. T1 is child, T2 is Parent.
        /// </summary>
        event Action<Nyu, Nyu> OnNyuParentSet;
    }
}
