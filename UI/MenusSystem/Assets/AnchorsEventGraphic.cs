using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.MenusSystem.Assets
{
    /// <summary>
    /// An <see cref="EventGraphic"/> which changes the anchored position of it's graphic.
    /// </summary>
    [CreateAssetMenu(fileName = "EventGraphic_Anchors_", menuName = "WarWolfWorks/UI/AnchorsEventGraphic")]
    public sealed class AnchorsEventGraphic : EventGraphic, IUpdate, IOnFocus, IOnUnfocus, IAwake
    {
        /// <summary>
        /// Anchor position towards which the affected graphic will go to when it's not focused.
        /// </summary>
        [SerializeField]
        private Vector4 s_AnchorsUnfocused;
        /// <summary>
        /// Anchor position towards which the affected graphic will go to when it's focused.
        /// </summary>
        [SerializeField]
        private Vector4 s_AnchorsFocused;

        /// <summary>
        /// Speed at which the anchors transition happens.
        /// </summary>
        [SerializeField, Range(0.01f, 10f)]
        private float s_AnchorsTransitionSpeed = 1f;

        /// <summary>
        /// The anchors towards which the affected graphic goes towards.
        /// </summary>
        public Vector4 DestinationAnchors { get; private set; }

        /// <summary>
        /// Gets the anchor values set.
        /// </summary>
        /// <param name="focused">If true, returns the value of the focused anchors, otherwise returns unfocused value.</param>
        /// <returns></returns>
        public Vector4 GetAnchors(bool focused)
            => focused ? s_AnchorsFocused : s_AnchorsUnfocused;

        /// <summary>
        /// Sets the value of the anchors.
        /// </summary>
        /// <param name="focused">If true, sets the value of the focused anchors, otherwise sets the unfocused anchors.</param>
        /// <param name="value"></param>
        public void SetAnchors(bool focused, Vector4 value)
        {
            if (focused)
                s_AnchorsFocused = value;
            else s_AnchorsUnfocused = value;
        }

        /// <summary>
        /// The transition speed of the event graphic between focused and unfocused anchors.
        /// </summary>
        public float TransitionSpeed
        {
            get => s_AnchorsTransitionSpeed;
            set => s_AnchorsTransitionSpeed = value;
        }

        void IAwake.Awake()
        {
            DestinationAnchors = s_AnchorsUnfocused;
        }

        void IOnFocus.Focus()
        {
            DestinationAnchors = s_AnchorsFocused;
        }

        void IOnUnfocus.Unfocus()
        {
            DestinationAnchors = s_AnchorsUnfocused;
        }

        void IUpdate.Update()
        {
            AffectedGraphic.rectTransform.SetAnchoredUI(
                Vector4.MoveTowards(AffectedGraphic.rectTransform.GetAnchoredPosition(), DestinationAnchors, s_AnchorsTransitionSpeed * Time.deltaTime));
        }
    }
}
