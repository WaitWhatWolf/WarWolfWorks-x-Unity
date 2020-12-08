using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// Scriptable object used by <see cref="ImageAnimator"/> to animate an image.
    /// </summary>
    [CreateAssetMenu(menuName = IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UI + "Image Animation", fileName = "UI_ImgAnim_New")]
    public sealed class ImageAnimation : ScriptableObject
    {
#pragma warning disable 1591
        [Tooltip("The sprites used for the animation. (Order-Sensitive)")]
        public Sprite[] Sprites;
        [Range(1, 180), Tooltip("How many fixed-step frames does the animation need to wait for in order to go to the next sprite.")]
        public int Counter;
        [Tooltip("If true, the animation will start over when it reaches the end, otherwise it will simply stop.")]
        public bool Loops;
#pragma warning restore 1591

        /// <summary>
        /// Creates an animation with a single image.
        /// </summary>
        /// <returns></returns>
        public static ImageAnimation CreateSingle(Sprite single, int counter = 5)
        {
            if (counter < 1)
                throw new IndexOutOfRangeException("Cannot create an animation with a counter less than 1.");

            ImageAnimation toReturn = CreateInstance<ImageAnimation>();
            toReturn.Sprites = new Sprite[] { single };
            toReturn.Counter = counter;

            return toReturn;
        }

        /// <summary>
        /// Creates an animation with a single image.
        /// </summary>
        /// <returns></returns>
        public static ImageAnimation New(IEnumerable<Sprite> sprites, int counter = 5)
        {
            if (counter < 1)
                throw new IndexOutOfRangeException("Cannot create an animation with a counter less than 1.");

            ImageAnimation toReturn = CreateInstance<ImageAnimation>();
            toReturn.Sprites = sprites.ToArray();
            toReturn.Counter = counter;

            return toReturn;
        }
    }
}
