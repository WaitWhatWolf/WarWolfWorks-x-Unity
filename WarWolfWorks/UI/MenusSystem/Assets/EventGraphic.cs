using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// The core class for <see cref="AdvancedIndexEvent"/>. (Inheritable)
    /// Supported interfaces: <see cref="IAwake"/>, <see cref="IUpdate"/>, <see cref="IOnFocus"/>, <see cref="IOnUnfocus"/>.
    /// </summary>
    [CompleteNoS]
    public abstract class EventGraphic : ScriptableObject, IParentable<AdvancedIndexEvent>
    {
        /// <summary>
        /// The affected graphic.
        /// </summary>
        public Graphic AffectedGraphic { get; internal set; }

        /// <summary>
        /// The parent of this <see cref="EventGraphic"/>.
        /// </summary>
        public AdvancedIndexEvent Parent { get; internal set; }
    }
}
