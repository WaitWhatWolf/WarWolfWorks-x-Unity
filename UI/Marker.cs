using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// Used with <see cref="GUIViewport"/> to display an image on screen based on a worldview.
    /// </summary>
    [System.Serializable]
    public class Marker : GUIViewport.GUI
    {
        /// <summary>
        /// <see cref="GUIViewport.GUI.CoreGraphic"/> as <see cref="Image"/>.
        /// </summary>
        public Image ImageGraphic => (Image)CoreGraphic;

        [SerializeField, NoS]
        private Vector2 s_AnchoredSize;
        [SerializeField, NoS]
        private Vector3 s_Offset;
        [SerializeField, NoS]
        private Transform s_Followed;
        [SerializeField, NoS]
        private Sprite s_MarkerSprite;

        /// <summary>
        /// The anchored size of the image.
        /// </summary>
        public override Vector2 AnchoredSize { get => s_AnchoredSize; }

        /// <summary>
        /// The position of the <see cref="Transform"/> followed.
        /// </summary>
        public override Vector3 Position { get => (s_Followed ? s_Followed.position : Vector3.zero) + s_Offset; protected set => s_Followed.position = value; }

        /// <summary>
        /// The sprite used for the marker.
        /// </summary>
        public Sprite MarkerSprite => s_MarkerSprite;

        /// <summary>
        /// The offset of the world position.
        /// </summary>
        public Vector3 Offset => s_Offset;

        /// <summary>
        /// <see cref="FillableMarker"/> 
        /// </summary>
        public override GUIType Type => GUIType.WorldPosToViewport;

        /// <summary>
        /// Destroys the attached <see cref="Image"/> component.
        /// </summary>
        public override void Dispose()
        {
            Object.Destroy(ImageGraphic.gameObject);

            Deinit();
        }

        /// <summary>
        /// Sets the cooldown and starts a coroutine to calculate the countdown.
        /// </summary>
        protected override void OnInit()
        {
            GameObject g = new GameObject("Marker_" + ID);
            Image ugui = g.AddComponent<Image>();
            ugui.sprite = s_MarkerSprite;
            ugui.rectTransform.localScale = Vector3.one;
            ugui.preserveAspect = true;
            ugui.raycastTarget = false;

            CoreGraphic = ugui;
        }

        /// <summary>
        /// Sets the followed transform to a new transform.
        /// </summary>
        /// <param name="to"></param>
        public void SetFollwed(Transform to)
        {
            s_Followed = to;
        }

        /// <summary>
        /// Creates a new <see cref="Marker"/>.
        /// </summary>
        public Marker(Transform followed, Sprite markerSprite, Vector2 anchoredSize, Vector3 offset)
        {
            s_AnchoredSize = anchoredSize;
            s_Followed = followed;
            s_MarkerSprite = markerSprite;
            s_Offset = offset;
        }

        /// <summary>
        /// Creates a copy of another <see cref="Marker"/>; Copies everything except <see cref="ImageGraphic"/>.
        /// </summary>
        /// <param name="copy"></param>
        public Marker(Marker copy)
        {
            s_AnchoredSize = copy.s_AnchoredSize;
            s_Followed = copy.s_Followed;
            s_MarkerSprite = copy.s_MarkerSprite;
            s_Offset = copy.s_Offset;
        }
    }
}
