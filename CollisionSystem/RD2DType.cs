using System;
using UnityEngine;

namespace WarWolfWorks.CollisionSystem
{
    /// <summary>
    /// Used by <see cref="RaycastDetector2D"/> to adapt it's raycasting.
    /// </summary>
    [Flags]
    public enum RD2DType
    {
        /// <summary>
        /// If flagged, it will perform a <see cref="Physics2D.RaycastAll(Vector2, Vector2, float, int, float, float)"/>.
        /// </summary>
        raycast = 1,
        /// <summary>
        /// If flagged, it will perform a <see cref="Physics2D.BoxCastAll(Vector2, Vector2, float, Vector2, float, int, float, float)"/>.
        /// </summary>
        boxcast = 2,
        /// <summary>
        /// If flagged, it will perform a <see cref="Physics2D.CircleCastAll(Vector2, float, Vector2, float, int, float, float)"/>.
        /// </summary>
        circlecast = 4,
        /// <summary>
        /// If flagged, it will perform a <see cref="Physics2D.OverlapCircleAll(Vector2, float, int, float, float)"/>.
        /// </summary>
        overlapCircle = 8,
        /// <summary>
        /// If flagged, it will perform a <see cref="Physics2D.OverlapAreaAll(Vector2, Vector2, int, float, float)"/>.
        /// </summary>
        overlapArea = 16,
        /// <summary>
        /// If flagged, it will perform a <see cref="Physics2D.OverlapBoxAll(Vector2, Vector2, float, int, float, float)"/>.
        /// </summary>
        overlapBox = 32
    }
}
