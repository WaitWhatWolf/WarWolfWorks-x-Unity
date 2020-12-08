using TMPro;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// A <see cref="FillableMarker"/> with text inside of it.
    /// </summary>
    [System.Serializable]
    public class FillableTextMarker : FillableMarker
    {
        /// <summary>
        /// The text graphic inside of <see cref="GUIViewport.GUI.CoreGraphic"/>.
        /// </summary>
        public TextMeshProUGUI TextGraphic { get; private set; }

        /// <summary>
        /// Creates a copy of another <see cref="FillableMarker"/>.
        /// </summary>
        /// <param name="copy"></param>
        public FillableTextMarker(FillableTextMarker copy) : base(copy)
        {

        }

        /// <summary>
        /// Creates a <see cref="FillableTextMarker"/>.
        /// </summary>
        /// <param name="followed"></param>
        /// <param name="markerSprite"></param>
        /// <param name="fillSprite"></param>
        /// <param name="anchoredSize"></param>
        /// <param name="offset"></param>
        public FillableTextMarker(Transform followed, Sprite markerSprite, Sprite fillSprite, Vector2 anchoredSize, Vector3 offset) 
            : base(followed, markerSprite, fillSprite, anchoredSize, offset)
        {
            
        }

        /// <summary>
        /// Initiates both graphics.
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            TextGraphic = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            TextGraphic.transform.SetParent(ImageGraphic.transform);
            TextGraphic.rectTransform.SetAnchoredUI(0, 0, 1, 1);
            TextGraphic.transform.localScale = Vector3.one;
            TextGraphic.fontSizeMin = 1;
            TextGraphic.fontSizeMax = 1000;
            TextGraphic.enableAutoSizing = true;
            TextGraphic.raycastTarget = false;
        }

        /// <summary>
        /// Disposes of both gameobject graphics.
        /// </summary>
        public override void Dispose()
        {
            Object.Destroy(TextGraphic.gameObject);
            base.Dispose();
        }
    }
}
