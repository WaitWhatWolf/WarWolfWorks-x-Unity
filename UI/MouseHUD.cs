using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// Displays an image right next to the cursor.
    /// </summary>
    public static class MouseHUD
    {
        private static WWWStack<(Sprite sprite, Color color)> MouseSprites { get; set; } = new WWWStack<(Sprite, Color)>();

        private static Image MouseImage;
        private static Thread MouseSetter = new Thread(new ThreadStart(InitiateMouseHUD));

        /// <summary>
        /// Returns the currently displayed Sprite/Color.
        /// </summary>
        public static (Sprite sprite, Color color) CurrentlyQueued
        {
            get
            {
                try
                {
                    return MouseSprites.Peek();
                }
                catch { return default; }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeThread()
        {
            GameObject g = new GameObject("MouseHUD");
            MouseImage = g.AddComponent<Image>();
            MouseImage.raycastTarget = false;
            g.AddComponent<MonoUpdater>().OnUpdate += SetMouseImagePosition;
            if (!Hooks.UtilityCanvas) MouseSetter.Start();
            else InitiateMouseHUD();
        }

        private static void InitiateMouseHUD()
        {
            while (true)
            {
                if (Hooks.UtilityCanvas)
                {
                    MouseImage.rectTransform.SetParent(Hooks.UtilityCanvas.transform);
                    RefreshMouseHUD();
                    break;
                }
            }

            if(MouseSetter.IsAlive) MouseSetter.Abort();
        }

        /// <summary>
        /// Adds the sprite to the queue of images to be displayed.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="color"></param>
        public static void AddToQueue(Sprite sprite, Color color)
        {
            MouseSprites.Push((sprite, color));
            RefreshMouseHUD();
        }

        /// <summary>
        /// Removes the sprite from the queue of images to be displayed.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="color"></param>
        public static void RemoveFromQueue(Sprite sprite, Color color)
        {
            MouseSprites.Remove(t => t.color == color && t.sprite == sprite);
            RefreshMouseHUD();
        }

        private static void SetMouseImagePosition()
        {
            Vector2 mousePos = Hooks.Cursor.MousePosInPercent;
            Resolution r = Screen.currentResolution;
            float xUse = ((float)r.height / r.width) / 7.5f;
            float yUse = ((float)r.width / r.height) / 15f;
            MouseImage.rectTransform.SetAnchoredUI(new Vector4(mousePos.x, mousePos.y - yUse, mousePos.x + xUse, mousePos.y));
        }

        private static void RefreshMouseHUD()
        {
            if (MouseSprites.Count == 0)
            {
                MouseImage.sprite = null;
                MouseImage.color = Color.clear;
            }
            else
            {
                (Sprite sprite, Color color) arg = MouseSprites.Lift();
                MouseImage.sprite = arg.sprite;
                MouseImage.color = arg.color;
            }
        }


    }
}
