using System;
using UnityEngine;
using UnityEngine.Serialization;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A utility method which allows for indexing of animations and playback of them.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UTILITY + nameof(AnimationManager))]
    public sealed class AnimationManager : MonoBehaviour
    {
        /// <summary>
        /// If true, it will debug all animations played in the unity console using <see cref="AdvancedDebug"/>.
        /// </summary>
        public bool DebugAnimationsPlayed;

        /// <summary>
        /// All clips contained by the Animation Manager.
        /// </summary>
        public AnimationCatalog[] Clips;
        
        /// <summary>
        /// The Animation component used to play animations.
        /// </summary>
        public Animator Player { get; private set; }

        /// <summary>
        /// Returns the index of the currently played animation.
        /// </summary>
        public int CurrentIndex => pv_Current.Index;
        /// <summary>
        /// Returns the name of the currently played animation.
        /// </summary>
        public string CurrentName => pv_Current.Name;
        /// <summary>
        /// Returns the progress of the currently played animation in percent, 0 being the start, 1 being the end.
        /// </summary>
        public float CurrentProgress01 => Player.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

        private void Awake()
        {
            Player = GetComponent<Animator>();
            if (Player is null)
                Player = GetComponentInParent<Animator>();
            if (Player is null)
                throw new Exception("Cannot use AnimationManager when the object or it's parent doesn't have an Animator component.");

            PlayAnimation(0);
        }

        /// <summary>
        /// Returns the currently played animation.
        /// </summary>
        /// <returns></returns>
        public AnimationCatalog CurrentlyPlaying()
        {
            return pv_Current;
        }

        /// <summary>
        /// Plays animation based on index.
        /// </summary>
        /// <param name="toPlay"></param>
        public void PlayAnimation(int toPlay)
        {
            foreach(AnimationCatalog ac in Clips)
            {
                if (ac.Index == toPlay)
                {
                    PlayAnimation(ac);
                    break;
                }
            }
        }

        /// <summary>
        /// Plays animation based on name.
        /// </summary>
        /// <param name="toPlay"></param>
        public void PlayAnimation(string toPlay)
        {
            foreach (AnimationCatalog ac in Clips)
            {
                if (ac.Name == toPlay) // 
                {
                    PlayAnimation(ac);
                    break;
                }
            }
        }

        /// <summary>
        /// Returns true if the given animation can be played.
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public bool CanPlayAnimation(AnimationCatalog ac)
        {
            return (ac.Repeats || !pv_Current.Equals(ac) && !ac.Repeats);
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void StopCurrentAnimation()
        {
            Player.speed = 0;
        }

        /// <summary>
        /// Resumes the current animation.
        /// </summary>
        public void ResumeCurrentAnimation()
        {
            Player.speed = 1f;
        }

        private void PlayAnimation(AnimationCatalog catalog)
        {
            if (!CanPlayAnimation(catalog))
                return;

            Player.speed = 1;
            Player.Play(catalog.Name, 0, 0.0f);
            if (DebugAnimationsPlayed)
            {
                AdvancedDebug.LogFormat("{0}'s AnimationManager log:\n{1} Was the last animation played.\n{2} is the animation currently requested.", 0, gameObject.name, CurrentName, catalog.Name);
            }
            pv_Current = catalog;
        }

        private AnimationCatalog pv_Current = new AnimationCatalog(string.Empty, 0, false);
    }

    /// <summary>
    /// A group of variables used for <see cref="AnimationManager"/>.
    /// </summary>
    [Serializable]
    public record AnimationCatalog
    {
        /// <summary>
        /// Name of the animation inside the <see cref="Animator"/> component.
        /// </summary>
        [Tooltip("Name of the animation inside the Animator component."), FormerlySerializedAs("animationName")]
        public string Name;
        /// <summary>
        /// Index assigned to this animation.
        /// </summary>
        [Tooltip("Index assigned to this animation."), FormerlySerializedAs("animationIndex")]
        public int Index;
        /// <summary>
        /// If true, the same animation can be called to play even if it is currently playing.
        /// </summary>
        [Tooltip("If true, the same animation can be called to play even if it is currently playing."), FormerlySerializedAs("animationRepeats")]
        public bool Repeats;

        /// <summary>
        /// Creates an <see cref="AnimationCatalog"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="repeating"></param>
        public AnimationCatalog(string name, int index, bool repeating)
        {
            Name = name;
            Index = index;
            Repeats = repeating;
        }
    }
}