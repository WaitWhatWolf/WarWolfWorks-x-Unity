using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// A utility component which animates an image component.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class ImageAnimator : MonoBehaviour
    {
        #region Unity Serialized
        [SerializeField, Tooltip("This animation will be used for the ImageAnimator.")]
        private ImageAnimation s_StartingAnimation;
        #endregion

#pragma warning disable 1591
        public Image GetAnimatedImage() => pv_AnimatedImage;
        public ImageAnimation GetCurrentAnimation() => pv_Animation;
#pragma warning restore 1591
        /// <summary>
        /// The amount of frames waited in total for the current animation.
        /// </summary>
        public int GetCurrentCount() => pv_CurrentCount;
        /// <summary>
        /// The currently displayed sprite index in the animation.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentFrame() => pv_CurrentFrame;

        /// <summary>
        /// Sets the animation and returns true if it was successfully set.
        /// </summary>
        /// <param name="animation"></param>
        /// <returns></returns>
        public bool SetAnimation(ImageAnimation animation)
        {
            if (animation == pv_Animation || animation == null || animation.Counter < 1)
                return false;

            CheckImage();

            pv_Animation = animation;
            pv_AllowUpdate = true;
            Reset();

            return true;
        }

        /// <summary>
        /// Removes the current animation.
        /// </summary>
        /// <param name="replace">Replaces the currently displayed image with this one.</param>
        /// <returns></returns>
        public bool RemoveAnimation(Sprite replace = null)
        {
            if (pv_Animation == null)
                return false;

            Reset();
            pv_Animation = null;
            pv_AnimatedImage.sprite = replace;
            pv_AllowUpdate = false;

            return true;
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause() => pv_Paused = true;
        /// <summary>
        /// Resumes the animation if it is paused.
        /// </summary>
        public void Resume() => pv_Paused = false;
        /// <summary>
        /// Resets the current animation to start from the beginning.
        /// </summary>
        public void Reset()
        {
            pv_IntendedFrame = pv_CurrentCount = pv_CurrentFrame = 0;
            CurrentFrame = 0;
        }
        /// <summary>
        /// Resets the current animation and pauses it.
        /// </summary>
        public void Stop()
        {
            Reset();
            pv_Paused = true;
        }

        private void Awake()
        {
            CheckImage();

            SetAnimation(s_StartingAnimation);
        }

        private void FixedUpdate()
        {
            if (pv_Paused || !pv_AllowUpdate || pv_Animation.Sprites.Length < 2)
                return;

            pv_CurrentCount++;

            if(pv_CurrentCount % pv_Animation.Counter == 0)
            {
                pv_IntendedFrame = pv_CurrentCount / pv_Animation.Counter;
                CurrentFrame = pv_IntendedFrame;

                if (pv_IntendedFrame >= pv_Animation.Sprites.Length)
                {
                    if (pv_Animation.Loops)
                    {
                        Reset();
                    }
                    else pv_Paused = true;
                }
            }
        }

        private int CurrentFrame
        {
            get => pv_CurrentFrame;
            set
            {
                if (pv_AllowUpdate && value < pv_Animation.Sprites.Length)
                {
                    pv_CurrentFrame = value;
                    pv_AnimatedImage.sprite = pv_Animation.Sprites[value];
                }
            }
        }

        private void CheckImage()
        {
            if (!pv_GotImage)
            {
                pv_AnimatedImage = GetComponent<Image>();
                pv_GotImage = true;
            }
        }

        private ImageAnimation pv_Animation;
        private Image pv_AnimatedImage;
        private int pv_CurrentCount;
        private int pv_CurrentFrame;
        private int pv_IntendedFrame;
        private bool pv_Paused;
        private bool pv_AllowUpdate;
        private bool pv_GotImage;
    }
}
