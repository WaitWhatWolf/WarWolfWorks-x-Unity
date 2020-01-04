using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// <see cref="ScrollMenuPercent"/> which inherits from <see cref="IndexMenu"/>.
    /// </summary>
    public abstract class IndexMenuScrollPercent : IndexMenu, IScrollablePercentMenu
    {
        [SerializeField]
        private Vector3 minPosition, maxPosition;

        /// <summary>
        /// UI Rect moved.
        /// </summary>
        [SerializeField]
        protected RectTransform scrollHolder;

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
        public RectTransform ScrollHolder => scrollHolder;

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

        /// <summary>
        /// Rect min and max anchores stored in a vector4. Sorted as follows: anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y.
        /// </summary>
        public Vector4 OriginalRectSize { get; private set; }

        /// <summary>
        /// Returns current rect size.
        /// </summary>
        public Vector4 CurrentRectSize => scrollHolder != null ? scrollHolder.GetAnchoredPosition() : Vector4.zero;

        private Vector4 GetFinalAnchor()
        {
            Vector2 percPos = PercentPosition;
            return new Vector4(percPos.x + OriginalRectSize.x,
                percPos.y + OriginalRectSize.y,
                percPos.x + OriginalRectSize.z,
                percPos.y + OriginalRectSize.w
            );
        }

        private Vector2 PercentPosition => Hooks.Vectors.MiddleMan(minPosition, maxPosition, Percent);

        /// <summary>
        /// Gets the <see cref="scrollHolder"/>'s size.
        /// </summary>
        protected virtual void Start()
        {
            OriginalRectSize = CurrentRectSize;
        }
    }
}
