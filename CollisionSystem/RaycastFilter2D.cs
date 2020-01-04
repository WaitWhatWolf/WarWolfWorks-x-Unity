using System;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.CollisionSystem
{
    /// <summary>
    /// Used by <see cref="RaycastDetector2D"/> for raycasting settings.
    /// </summary>
    [Serializable]
    public struct RaycastFilter2D
    {
        /// <summary>
        /// Layermask used for raycasts.
        /// </summary>
        public LayerMask LayerMask;
        /// <summary>
        /// Min-Max Depth for raycasting.
        /// </summary>
        public FloatRange Depth;
    }
}
