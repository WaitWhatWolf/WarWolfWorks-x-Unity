using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.Transitioning
{
    /// <summary>
    /// A transition class which makes a solid color appear on the screen with it's alpha based on the progression of the transition.
    /// </summary>
    public sealed class ColorTransition : Transition, IAwake, IUpdate, IOnDestroy
    {
        /// <summary>
        /// The image used for the color display.
        /// </summary>
        public Image Graphic { get; private set; }

        /// <summary>
        /// The color used.
        /// </summary>
        public Color Color { get; private set; }

        void IAwake.Awake()
        {
            Graphic = new GameObject("Color Transition Image").AddComponent<Image>();
            Graphic.color = Color.clear;

            Graphic.rectTransform.SetParent(Parent.GetHolder());
            Graphic.rectTransform.SetAnchoredUI(0, 0, 1, 1);
            Graphic.rectTransform.localScale = Vector3.one;
            Graphic.raycastTarget = false;
        }

        void IOnDestroy.OnDestroy()
        {
            Object.Destroy(Graphic.gameObject);
        }

        void IUpdate.Update()
        {
            Graphic.color = new Color(Color.r, Color.g, Color.b, TransitionProgress);
        }

        /// <summary>
        /// Creates a color transition.
        /// </summary>
        /// <param name="color"></param>
        public ColorTransition(Color color)
        {
            Color = color;
        }

        /// <summary>
        /// Creates a color transition with a black color.
        /// </summary>
        public ColorTransition()
        {
            Color = Color.black;
        }
    }
}
