using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// Menu which scroll a <see cref="RectTransform"/> based on a percentage.
    /// </summary>
    public abstract class ScrollMenuPercent : Menu, IScrollablePercentMenu
    {
        [SerializeField]
        private Vector2 minPosition;
        //[Tooltip("Position at which this menu will go to when Percent is at 1; MinPosition is the starting position.")]
        [SerializeField]
        private Vector2 maxPosition;

        /// <summary>
        /// Pointer to minimal Position assigned in the inspector.
        /// </summary>
        public Vector3 MinPosition { get { return minPosition; } set { minPosition = value; } }
        /// <summary>
        /// Pointer minimal Position assigned in the inspector.
        /// </summary>
        public Vector3 MaxPosition { get { return maxPosition; } set { maxPosition = value; } }

        /// <summary>
        /// UI Rect moved.
        /// </summary>
        [FormerlySerializedAs("movedUI")]
        [SerializeField]
        protected RectTransform scrollHolder;
        /// <summary>
        /// <see cref="RectTransform"/> which is moved based on <see cref="Percent"/>.
        /// </summary>
        public RectTransform ScrollHolder => scrollHolder;

        /// <summary>
        /// Rect min and max anchores stored in a vector4. Sorted as follows: anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y.
        /// </summary>
        public Vector4 OriginalRectSize { get; private set; }

        /// <summary>
        /// Returns current rect size.
        /// </summary>
        public Vector4 CurrentRectSize => scrollHolder != null ? scrollHolder.GetAnchoredPosition() : Vector4.zero;

        private float percent;
        private readonly FloatRange percentRange = new FloatRange(0f, 1f);
        /// <summary>
        /// The current percentage of this menu. (Clamped 0-1)
        /// </summary>
        public float Percent
        {
            get => percent;
            set
            {
                percent = percentRange.GetClampedValue(value);
                scrollHolder.SetAnchoredUI(GetFinalAnchor());
            }
        }

        private Vector2 PercentPosition => Hooks.Vectors.MiddleMan(minPosition, maxPosition, Percent);

        private Vector4 GetFinalAnchor()
        {
            Vector2 percPos = PercentPosition;
            return new Vector4(percPos.x + OriginalRectSize.x,
                percPos.y + OriginalRectSize.y,
                percPos.x + OriginalRectSize.z,
                percPos.y + OriginalRectSize.w
            );
        }

        /// <summary>
        /// Gets the <see cref="scrollHolder"/>'s size.
        /// </summary>
        protected virtual void Start()
        {
            OriginalRectSize = CurrentRectSize;
            //minPosition = new Vector2((OriginalRectSize.x + OriginalRectSize.z) / 2, (OriginalRectSize.y + OriginalRectSize.w) / 2);
        }
    }
}
