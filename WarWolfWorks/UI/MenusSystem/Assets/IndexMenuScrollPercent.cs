using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// <see cref="ScrollMenuPercent"/> which inherits from <see cref="IndexMenu"/>.
    /// </summary>
    public abstract class IndexMenuScrollPercent : IndexMenu, IScrollablePercentMenu
    {
        [FormerlySerializedAs("minPosition"), SerializeField]
        private Vector3 s_MinPosition;
        [FormerlySerializedAs("maxPosition"), SerializeField]
        private Vector3 s_MaxPosition;

        /// <summary>
        /// UI Rect moved.
        /// </summary>
        [FormerlySerializedAs("scrollHolder"), SerializeField]
        protected RectTransform s_ScrollHolder;

        /// <summary>
        /// Pointer to minimal Position assigned in the inspector.
        /// </summary>
        public Vector3 MinPosition { get { return s_MinPosition; } set { s_MinPosition = value; } }
        /// <summary>
        /// Pointer minimal Position assigned in the inspector.
        /// </summary>
        public Vector3 MaxPosition { get { return s_MaxPosition; } set { s_MaxPosition = value; } }

        /// <summary>
        /// UI Rect moved.
        /// </summary>
        public RectTransform ScrollHolder => s_ScrollHolder;

        private float ns_Percent;
        private readonly FloatRange ns_PercentRange = new FloatRange(0f, 1f);
        /// <summary>
        /// The current percentage of this menu. (Clamped 0-1)
        /// </summary>
        public float Percent
        {
            get => ns_Percent;
            set
            {
                ns_Percent = ns_PercentRange.GetClampedValue(value);
                s_ScrollHolder.SetAnchoredUI(GetFinalAnchor());
            }
        }

        /// <summary>
        /// Rect min and max anchores stored in a vector4. Sorted as follows: anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y.
        /// </summary>
        public Vector4 OriginalRectSize { get; private set; }

        /// <summary>
        /// Returns current rect size.
        /// </summary>
        public Vector4 CurrentRectSize => s_ScrollHolder != null ? s_ScrollHolder.GetAnchoredPosition() : Vector4.zero;

        private Vector4 GetFinalAnchor()
        {
            Vector2 percPos = PercentPosition;
            return new Vector4(percPos.x + OriginalRectSize.x,
                percPos.y + OriginalRectSize.y,
                percPos.x + OriginalRectSize.z,
                percPos.y + OriginalRectSize.w
            );
        }

        private Vector2 PercentPosition => Hooks.MathF.MiddleMan(s_MinPosition, s_MaxPosition, Percent);

        /// <summary>
        /// Gets the <see cref="s_ScrollHolder"/>'s size.
        /// </summary>
        protected virtual void Start()
        {
            OriginalRectSize = CurrentRectSize;
        }
    }
}
