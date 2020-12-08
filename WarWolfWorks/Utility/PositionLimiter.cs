using UnityEngine;
using System.Collections.Generic;
using static WarWolfWorks.Constants;
using System;
using WarWolfWorks.Debugging;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Limits a transform's position to specified positions or transforms.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UTILITY + nameof(PositionLimiter))]
    public class PositionLimiter : MonoBehaviour
    {
        /// <summary>
        /// Does the limiter only work on 2D axis?
        /// </summary>
        [Tooltip("If true, the Z axis will be ignored.")]
        public bool Is2D;

        /// <summary>
        /// How the <see cref="PositionLimiter"/> limits it's position.
        /// </summary>
        [Flags]
        public enum LimitationType
        {
            /// <summary>Does not limit the position of the transform in any way.</summary>
            Disabled = 0,
            /// <summary>Limits the position based on <see cref="LimitBoundsX"/>.</summary>
            StaticLimitX = 4,
            /// <summary>Limits the position based on <see cref="LimitBoundsY"/>.</summary>
            StaticLimitY = 8,
            /// <summary>Limits the position based on <see cref="LimitBoundsZ"/>.</summary>
            StaticLimitZ = 16,
            /// <summary>Limits the position based on <see cref="Limiters"/>.</summary>
            DynamicLimit = 32,
        }

        /// <summary>
        /// The currently applied limitation type.
        /// </summary>
        [Tooltip("Specifies which Limit type will be used;" +
            "\nDynamic Limit: Limited between Limiters." +
            "\nStatic Limit: Limited between LimitBoundsX, Y and Z." +
            "\nDisabled: Does not limit.")]
        public LimitationType LimitType = LimitationType.DynamicLimit;

        /// <summary>
        /// All limiters used when <see cref="LimitationType"/> is set to <see cref="LimitationType.DynamicLimit"/>.
        /// </summary>
        public List<Transform> Limiters;

        /// <summary>
        /// Limit applied to the respectice coordinate.
        /// </summary>
        public FloatRange LimitBoundsX, LimitBoundsY, LimitBoundsZ;

        /// <summary>
        /// Override this Vector3 to set a specific position to move; points to transform.position by default.
        /// </summary>
        protected virtual Vector3 LimitedPosition { get => transform.position; set => transform.position = value; }

        /// <summary>
        /// Gets the position hypothetically applied based on given limitations.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Vector3 GetFinalPosition(LimitationType type)
        {
            if (type == LimitationType.Disabled)
                return LimitedPosition;
            else if (type == LimitationType.DynamicLimit)
            {
                FloatRange x = new(LimitedPosition.x), y = new(LimitedPosition.y), z = new(LimitedPosition.z);

                foreach (Transform t in Limiters)
                {
                    if (t is not null)
                    {
                        if (t.position.x < x.Min)
                            x = x with { Min = t.position.x };
                        else if (t.position.x > x.Max)
                            x = x with { Max = t.position.x };

                        if (t.position.y < y.Min)
                            y = y with { Min = t.position.y };
                        else if (t.position.y > y.Max)
                            y = y with { Max = t.position.y };

                        if (t.position.z < z.Min)
                            z = z with { Min = t.position.z };
                        else if (t.position.z > z.Max)
                            z = z with { Max = t.position.z };
                    }
                }

                return GetFinalFromRange(x, y, z);
            }
            else
            {
                return GetFinalFromRange(
                    type.HasFlag(LimitationType.StaticLimitX) ? LimitBoundsX : new FloatRange(LimitedPosition.x), 
                    type.HasFlag(LimitationType.StaticLimitY) ? LimitBoundsY : new FloatRange(LimitedPosition.y), 
                    type.HasFlag(LimitationType.StaticLimitZ) ? LimitBoundsZ : new FloatRange(LimitedPosition.z));
            }
        }

        private Vector3 GetFinalFromRange(FloatRange x, FloatRange y, FloatRange z)
        {
            return new Vector3(x.GetClampedValue(LimitedPosition.x), y.GetClampedValue(LimitedPosition.y), z.GetClampedValue(LimitedPosition.z));
        }

        /// <summary>
        /// Applies <see cref="GetFinalPosition(LimitationType)"/> to <see cref="LimitedPosition"/>.
        /// </summary>
        protected virtual void LateUpdate()
        {
            LimitedPosition = GetFinalPosition(LimitType);
        }
    }
}