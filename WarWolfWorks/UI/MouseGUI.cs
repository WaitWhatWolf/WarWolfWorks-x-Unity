using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Internal;
using WarWolfWorks.Threading;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// Displays an image right next to the cursor.
    /// </summary>
    public static class MouseGUI
    {
        [Flags]
        internal enum MouseGraphicDisplayType
        {
            None = 0,
            Sprite = 8,
            Text = 16,
        }
        /// <summary>
        /// Graphic to be displayed in <see cref="MouseGUI"/>.
        /// </summary>
        public struct MouseGraphic
        {
            /// <summary>
            /// Anchored size of this <see cref="MouseGraphic"/>.
            /// </summary>
            public Vector2 AnchoredSize;

            /// <summary>
            /// Sprite of this <see cref="MouseGraphic"/>.
            /// </summary>
            public Sprite MouseSprite;

            /// <summary>
            /// Color of the <see cref="MouseSprite"/>.
            /// </summary>
            public Color SpriteColor;

            /// <summary>
            /// Text of this <see cref="MouseGraphic"/>.
            /// </summary>
            public string MouseText;

            /// <summary>
            /// Color of <see cref="MouseText"/>.
            /// </summary>
            public Color TextColor;

            internal MouseGraphicDisplayType GraphicDisplayType;

            /// <summary>
            /// Creates a <see cref="MouseGraphic"/> which displays both a sprite and text.
            /// </summary>
            /// <param name="anchorSize"></param>
            /// <param name="sprite"></param>
            /// <param name="text"></param>
            /// <param name="spriteColor"></param>
            /// <param name="textColor"></param>
            public MouseGraphic(Vector2 anchorSize, Sprite sprite, string text, Color spriteColor, Color textColor)
            {
                AnchoredSize = anchorSize;
                MouseSprite = sprite;
                MouseText = text;
                SpriteColor = spriteColor;
                TextColor = textColor;
                GraphicDisplayType = MouseGraphicDisplayType.Text | MouseGraphicDisplayType.Sprite;
            }

            /// <summary>
            /// Creates a <see cref="MouseGraphic"/> which displays a sprite.
            /// </summary>
            /// <param name="anchorSize"></param>
            /// <param name="sprite"></param>
            /// <param name="spriteColor"></param>
            public MouseGraphic(Vector2 anchorSize, Sprite sprite, Color spriteColor)
            {
                AnchoredSize = anchorSize;
                MouseSprite = sprite;
                MouseText = string.Empty;
                SpriteColor = spriteColor;
                TextColor = Color.clear;
                GraphicDisplayType = MouseGraphicDisplayType.Sprite;
            }

            /// <summary>
            /// Creates a <see cref="MouseGraphic"/> which displays text.
            /// </summary>
            /// <param name="anchorSize"></param>
            /// <param name="text"></param>
            /// <param name="textColor"></param>
            public MouseGraphic(Vector2 anchorSize, string text, Color textColor)
            {
                AnchoredSize = anchorSize;
                MouseSprite = null;
                MouseText = text;
                SpriteColor = Color.clear;
                TextColor = textColor;
                GraphicDisplayType = MouseGraphicDisplayType.Text;
            }
        }
        private static List<MouseGraphic> MouseSprites { get; set; } = new List<MouseGraphic>();

        private static RectTransform MouseGUIHolder;

        private static Image MouseImage;
        private static TextMeshProUGUI MouseText;
        private static Thread MouseSetter = new Thread(new ThreadStart(() => InitiateMouseHUD(true)));

        /// <summary>
        /// Returns the currently displayed Sprite/Color.
        /// </summary>
        public static MouseGraphic CurrentlyQueued { get; private set; }

        /// <summary>
        /// Returns the image component used for mouse graphics.
        /// </summary>
        /// <returns></returns>
        public static Image GetMouseImage() => MouseImage;

        /// <summary>
        /// Returns the text mesh pro component used for mouse graphics.
        /// </summary>
        /// <returns></returns>
        public static TextMeshProUGUI GetMouseTMP() => MouseText;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#pragma warning disable IDE0051
        private static void InitializeThread()
#pragma warning restore IDE0051
        {
            MouseGUIHolder = new GameObject("MouseGUI").AddComponent<RectTransform>();
            MouseGUIHolder.SetAnchoredUI(0, 0, 1, 1);
            MouseGUIHolder.SetParent(Settings.UtilityCanvas.transform);
            MouseGUIHolder.transform.localScale = Vector3.one;

            GameObject imageGO = new GameObject("MouseGUIImage");
            imageGO.transform.SetParent(MouseGUIHolder);
            MouseImage = imageGO.AddComponent<Image>();
            MouseImage.raycastTarget = false;
            MouseImage.rectTransform.SetAnchoredUI(0, 0, 1, 1);
            MouseImage.transform.localScale = Vector3.one;

            GameObject textGO = new GameObject("MouseText");
            textGO.transform.SetParent(MouseGUIHolder);
            MouseText = textGO.AddComponent<TextMeshProUGUI>();
            MouseText.raycastTarget = false;
            MouseText.alignment = TextAlignmentOptions.TopLeft;
            MouseText.enableAutoSizing = true;
            MouseText.fontSizeMin = 1;
            MouseText.fontSizeMax = 1000;
            MouseText.rectTransform.SetAnchoredUI(0, 0, 1, 1);
            MouseText.transform.localScale = Vector3.one;

            MouseGUIHolder.gameObject.AddComponent<MonoUGUI>().OnGUIevent += SetMouseImagePosition;
            if (!Settings.UtilityCanvas)
            {
                AdvancedDebug.LogFormat("No utility canvas was found; Retrieving {0} to queue task...", DEBUG_LAYER_WWW_INDEX, ThreadingUtilities.Instance);
                MouseSetter.Start();
            }
            else InitiateMouseHUD(false);
        }

        private static void InitiateMouseHUD(bool useThreadingUtil)
        {
            while (Settings.UtilityCanvas == null && MouseGUIHolder.transform.parent == null)
            {
                if (!useThreadingUtil)
                    IMHUDLoop();
                else ThreadingUtilities.QueueOnMainThread(IMHUDLoop);
            }

            if(MouseSetter.IsAlive) MouseSetter.Abort();
        }

        private static void IMHUDLoop()
        {
            if (Settings.UtilityCanvas)
            {
                MouseGUIHolder.transform.SetParent(Settings.UtilityCanvas.transform);
                MouseGUIHolder.transform.localScale = Vector3.one;
                MouseGUIHolder.transform.localPosition = Vector3.zero;
                RefreshMouseHUD();
            }
        }

        /// <summary>
        /// Adds the sprite to the queue of images to be displayed.
        /// </summary>
        /// <param name="graphic"></param>
        public static void AddToQueue(MouseGraphic graphic)
        {
            MouseSprites.Insert(MouseSprites.Count, graphic);
            RefreshMouseHUD();
        }

        /// <summary>
        /// Removes the currently displayed <see cref="MouseGraphic"/> from queue.
        /// </summary>
        public static void RemoveFromQueue()
        {
            if (MouseSprites.Count > 0)
            {
                MouseSprites.RemoveAt(0);
                RefreshMouseHUD();
            }
        }

        /// <summary>
        /// Removes a specific graphic from queue.
        /// </summary>
        /// <param name="graphic"></param>
        public static void RemoveFromQueue(Predicate<MouseGraphic> graphic)
        {
            if(MouseSprites.Remove(MouseSprites.Find(graphic)))
                RefreshMouseHUD();
        }

        private static void SetMouseImagePosition()
        {
            Vector2 mousePos = Hooks.Cursor.MousePosInPercent;
            MouseGUIHolder.SetAnchoredUI(mousePos.x, mousePos.y - CurrentlyQueued.AnchoredSize.y, mousePos.x + CurrentlyQueued.AnchoredSize.x, mousePos.y);
        }

        private static void RefreshMouseHUD()
        {
            if (MouseSprites.Count == 0)
            {
                MouseImage.sprite = null;
                MouseImage.color = Color.clear;
                CurrentlyQueued = default;
            }
            else
            {
                CurrentlyQueued = MouseSprites[0];
                if (CurrentlyQueued.GraphicDisplayType.HasFlag(MouseGraphicDisplayType.Sprite))
                {
                    MouseImage.sprite = CurrentlyQueued.MouseSprite;
                    MouseImage.color = CurrentlyQueued.SpriteColor;
                }
                if (CurrentlyQueued.GraphicDisplayType.HasFlag(MouseGraphicDisplayType.Text))
                {
                    MouseText.text = CurrentlyQueued.MouseText;
                    MouseText.color = CurrentlyQueued.TextColor;
                }
            }
        }


    }
}
