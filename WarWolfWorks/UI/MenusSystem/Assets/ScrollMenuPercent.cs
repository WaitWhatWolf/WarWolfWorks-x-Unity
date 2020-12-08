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
        [FormerlySerializedAs("minPosition"), SerializeField]
        private Vector2 s_MinPosition;
        [FormerlySerializedAs("maxPosition"), SerializeField]
        private Vector2 s_MaxPosition;

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
        [FormerlySerializedAs("scrollHolder"), SerializeField]
        protected RectTransform s_ScrollHolder;
        /// <summary>
        /// <see cref="RectTransform"/> which is moved based on <see cref="Percent"/>.
        /// </summary>
        public RectTransform ScrollHolder => s_ScrollHolder;

        /// <summary>
        /// Rect min and max anchores stored in a vector4. Sorted as follows: anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y.
        /// </summary>
        public Vector4 OriginalRectSize { get; private set; }

        /// <summary>
        /// Returns current rect size.
        /// </summary>
        public Vector4 CurrentRectSize => s_ScrollHolder != null ? s_ScrollHolder.GetAnchoredPosition() : Vector4.zero;

        private float ns_Percent;
        private readonly FloatRange PercentRange = new FloatRange(0f, 1f);
        /// <summary>
        /// The current percentage of this menu. (Clamped 0-1)
        /// </summary>
        public float Percent
        {
            get => ns_Percent;
            set
            {
                ns_Percent = PercentRange.GetClampedValue(value);
                s_ScrollHolder.SetAnchoredUI(GetFinalAnchor());
            }
        }

        private Vector2 PercentPosition => Hooks.MathF.MiddleMan(s_MinPosition, s_MaxPosition, Percent);

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
        /// Gets the <see cref="s_ScrollHolder"/>'s size.
        /// </summary>
        protected virtual void Start()
        {
            OriginalRectSize = CurrentRectSize;
            //minPosition = new Vector2((OriginalRectSize.x + OriginalRectSize.z) / 2, (OriginalRectSize.y + OriginalRectSize.w) / 2);
        }
    }
}
