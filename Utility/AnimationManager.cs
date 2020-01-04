using System;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    using System.Collections.Generic;
    using WarWolfWorks.Utility;
    
    public sealed class AnimationManager : MonoBehaviour
    {
        [SerializeField]
        private bool DebugAnimationsPlayed;

        /// <summary>
        /// All clips contained by the Animation Manager.
        /// </summary>
        public AnimationCatalog[] Clips;
        
        private Animator animat = null;
        /// <summary>
        /// The Animation component used to play animations.
        /// </summary>
        public Animator Player
        {
            get
            {
                if (!animat)
                {
                    animat = GetComponent<Animator>();
                    if (!animat) animat = Hooks.ObjectParent(gameObject).GetComponent<Animator>();
                }
                return animat;
            }
        }

        private AnimationCatalog LastPlayed;
        public int LastAnimationIndex => LastPlayed.Index;
        public string LastAnimationName => LastPlayed.Name;
        public float AnimationProgress01 => Player.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

        private void Awake()
        {
            PlayAnimation(0);
        }

        public AnimationCatalog CurrentlyPlaying()
        {
            return LastPlayed;
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

        private void PlayAnimation(AnimationCatalog catalog)
        {
            if (!CanPlayAnimation(catalog))
                return;

            Player.speed = 1;
            Player.Play(catalog.Name, 0, 0.0f);
            if(DebugAnimationsPlayed)
            {
                AdvancedDebug.LogFormat("{0}'s AnimationManager log:\n{1} Was the last animation played.\n{2} is the animation currently requested.", 0, gameObject.name, LastAnimationName, catalog.Name);
            }
            LastPlayed = catalog;
        }

        public bool CanPlayAnimation(AnimationCatalog ac)
        {
            return (ac.Repeats || !LastPlayed.Equals(ac) && !ac.Repeats);
        }

        public void StopCurrentAnimation()
        {
            Player.speed = 0;
        }

    }

    [Serializable]
    public struct AnimationCatalog
    {
        [SerializeField]
        private string animationName;
        [SerializeField]
        private int animationIndex;
        [SerializeField]
        private bool animationRepeats;

        public string Name => animationName;
        public int Index => animationIndex;
        public bool Repeats => animationRepeats;

        public AnimationCatalog(string name, int index, bool repeating)
        {
            animationName = name;
            animationIndex = index;
            animationRepeats = repeating;
        }

        public override bool Equals(object obj)
        {
            AnimationCatalog ac = (AnimationCatalog)obj;
            return Name == ac.Name && Index == ac.Index;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}