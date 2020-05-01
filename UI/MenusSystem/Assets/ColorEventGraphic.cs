using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// An <see cref="EventGraphic"/> which changes the anchored position of it's graphic.
    /// </summary>
    [CreateAssetMenu(fileName = "EventGraphic_Color_", menuName = "WarWolfWorks/UI/ColorEventGraphic")]
    public sealed class ColorEventGraphic : EventGraphic, IUpdate, IOnFocus, IOnUnfocus, IAwake
    {
        /// <summary>
        /// Color towards which the affected graphic will go to when it's not focused.
        /// </summary>
        [SerializeField]
        private Color s_ColorUnfocused;
        /// <summary>
        /// Color towards which the affected graphic will go to when it's focused.
        /// </summary>
        [SerializeField]
        private Color s_ColorFocused;

        /// <summary>
        /// Speed at which the color transition happens.
        /// </summary>
        [SerializeField, Range(0.01f, 10f)]
        private float s_ColorTransitonSpeed;

        /// <summary>
        /// The color towards thich the affected graphic goes towards.
        /// </summary>
        public Color DestinationColor { get; private set; }

        void IAwake.Awake()
        {
            DestinationColor = s_ColorUnfocused;
        }

        void IOnFocus.Focus()
        {
            DestinationColor = s_ColorFocused;
        }

        void IOnUnfocus.Unfocus()
        {
            DestinationColor = s_ColorUnfocused;
        }

        void IUpdate.Update()
        {
            AffectedGraphic.color = Hooks.Colors.MoveTowards(AffectedGraphic.color, DestinationColor, s_ColorTransitonSpeed * Time.deltaTime);
        }
    }
}
