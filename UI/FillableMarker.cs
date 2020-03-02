using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// A <see cref="Marker"/> with a fillable image inside of it.
    /// </summary>
    [System.Serializable]
    public class FillableMarker : Marker
    {
        [SerializeField, NoS]
        private Sprite s_FillSprite;

        /// <summary>
        /// Sprite used for the fillable image.
        /// </summary>
        public Sprite FillSprite => s_FillSprite;

        /// <summary>
        /// The fillable image inside of <see cref="Marker.ImageGraphic"/>.
        /// </summary>
        public Image ImageFillGraphic { get; private set; }

        /// <summary>
        /// Creates a copy of another <see cref="FillableMarker"/>.
        /// </summary>
        /// <param name="copy"></param>
        public FillableMarker(FillableMarker copy) : base(copy)
        {
            s_FillSprite = copy.s_FillSprite;
        }

        /// <summary>
        /// Creates a <see cref="FillableMarker"/>.
        /// </summary>
        /// <param name="followed"></param>
        /// <param name="markerSprite"></param>
        /// <param name="fillSprite"></param>
        /// <param name="anchoredSize"></param>
        /// <param name="offset"></param>
        public FillableMarker(Transform followed, Sprite markerSprite, Sprite fillSprite, Vector2 anchoredSize, Vector3 offset) 
            : base(followed, markerSprite, anchoredSize, offset)
        {
            s_FillSprite = fillSprite;
        }

        /// <summary>
        /// Initiates both graphics.
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            ImageFillGraphic = new GameObject("Fill").AddComponent<Image>();
            ImageFillGraphic.transform.SetParent(ImageGraphic.transform);
            ImageFillGraphic.sprite = s_FillSprite;
            ImageFillGraphic.transform.localScale = Vector3.one;
            ImageFillGraphic.rectTransform.SetAnchoredUI(0, 0, 1, 1);
            ImageFillGraphic.raycastTarget = false;

            ImageGraphic.preserveAspect = false;
            ImageFillGraphic.preserveAspect = false;
            ImageFillGraphic.type = Image.Type.Filled;
        }

        /// <summary>
        /// Disposes of both gameobject graphics.
        /// </summary>
        public override void Dispose()
        {
            Object.Destroy(ImageFillGraphic.gameObject);
            base.Dispose();
        }
    }
}
