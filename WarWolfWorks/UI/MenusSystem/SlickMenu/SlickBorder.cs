using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Enums;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.UI.MenusSystem.SlickMenu
{
    /// <summary>
    /// A behavior which sets borders to a rect transform of 1 pixel width/height to make a clean and simple interface.
    /// </summary>
    public class SlickBorder : MonoBehaviour, IRefreshable, IInitiatable, IDeinitiatable
    {
        /// <summary>
        /// Returns all initiated borders in the scene.
        /// </summary>
        public static IReadOnlyList<SlickBorder> Borders => pv_Borders;
        
        /// <summary>
        /// Calls the <see cref="Refresh"/> method of all existing <see cref="SlickBorder"/>s.
        /// </summary>
        public static void RefreshAllBorders()
        {
            foreach (SlickBorder setter in pv_Borders)
                setter.Refresh();
        }

        #region Unity Serialized
        /// <summary>
        /// Flags of this <see cref="SlickBorder"/>; Used on <see cref="Init"/>.
        /// </summary>
        public SlickBorderFlags Flags = SlickBorderFlags.All;

        /// <summary>
        /// The color applied to all border images.
        /// </summary>
        public Color Color = UI_Slick_Color_Default;
        #endregion

        /// <summary>
        /// Initiated state of this <see cref="SlickBorder"/>.
        /// </summary>
        public bool Initiated { get; private set; }

        /// <summary>
        /// The hidden state of this <see cref="SlickBorder"/> modified through <see cref="Hide"/> and <see cref="Show"/>.
        /// </summary>
        public bool Hidden { get; private set; }

        /// <summary>
        /// Initiates the border images.
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            if (Initiated)
                return false;

            pr_Holder = new GameObject(nameof(SlickBorder)).AddComponent<RectTransform>();
            pr_Holder.SetParent(transform);
            pr_Holder.transform.localScale = Vector3.one;
            pr_Holder.SetAnchoredUI(0f, 0f, 1f, 1f);

            List<Image> images = new List<Image>(4);

            if (Flags.HasFlag(SlickBorderFlags.Left))
            {
                SetImage(ref pr_IMG_Left, UI_Slick_Rect_Left, SlickBorderFlags.Left);
                images.Add(pr_IMG_Left);
            }
            if (Flags.HasFlag(SlickBorderFlags.Right))
            {
                SetImage(ref pr_IMG_Right, UI_Slick_Rect_Right, SlickBorderFlags.Right);
                images.Add(pr_IMG_Right);
            }
            if (Flags.HasFlag(SlickBorderFlags.Top))
            {
                SetImage(ref pr_IMG_Top, UI_Slick_Rect_Top, SlickBorderFlags.Top);
                images.Add(pr_IMG_Top);
            }
            if (Flags.HasFlag(SlickBorderFlags.Bot))
            {
                SetImage(ref pr_IMG_Bot, UI_Slick_Rect_Bot, SlickBorderFlags.Bot);
                images.Add(pr_IMG_Bot);
            }

            images.TrimExcess();
            pr_Images = images.ToArray();

            pv_Borders.Add(this);

            Refresh();

            return Initiated = true;
        }

        /// <summary>
        /// Opposite of <see cref="Init"/>, which destroys all initiated border images.
        /// </summary>
        /// <returns></returns>
        public bool Deinit()
        {
            if (!Initiated)
                return false;

            for (int i = 0; i < pr_Images.Length; i++)
            {
                Destroy(pr_Images[i].gameObject);
            }

            Destroy(pr_Holder);

            pr_Images = null;

            pv_Borders.Remove(this);
            Initiated = false;
            return true;
        }

        /// <summary>
        /// Updates the color of all border images.
        /// </summary>
        public virtual void Refresh()
        {
            foreach (Image image in pr_Images)
            {
                image.color = Color;
            }
        }

        /// <summary>
        /// Calls <see cref="Init"/>.
        /// </summary>
        protected virtual void Start()
        {
            Init();
        }

        /// <summary>
        /// Hides the border.
        /// </summary>
        public void Hide()
        {
            if (Hidden)
                return;

            Hidden = true;

            pr_Holder.gameObject.SetActive(!Hidden);
        }

        /// <summary>
        /// Un-hides the border.
        /// </summary>
        public void Show()
        {
            if (!Hidden)
                return;

            Hidden = false;

            pr_Holder.gameObject.SetActive(!Hidden);
        }

        private void SetImage(ref Image image, Vector4 rect, SlickBorderFlags flags)
        {
            image = new GameObject(flags.ToString()).AddComponent<Image>();
            image.rectTransform.SetParent(pr_Holder);
            image.rectTransform.localScale = Vector3.one;
            image.rectTransform.SetAnchoredUI(rect);
            AdaptSlickAnchors(flags, image.rectTransform);

            if (Color == Color.clear)
                image.color = WWWResources.UI_Slick_Color_Default;
            else
                image.color = Color;
        }

        protected Image pr_IMG_Left, pr_IMG_Right, pr_IMG_Top, pr_IMG_Bot;
        /// <summary>
        /// The holder of all slick border images.
        /// </summary>
        protected RectTransform pr_Holder;
        /// <summary>
        /// <see cref="pr_IMG_Left"/>, <see cref="pr_IMG_Top"/>, <see cref="pr_IMG_Right"/> and <see cref="pr_IMG_Bot"/> in an array.
        /// </summary>
        protected Image[] pr_Images;

        private static List<SlickBorder> pv_Borders = new();
    }
}
