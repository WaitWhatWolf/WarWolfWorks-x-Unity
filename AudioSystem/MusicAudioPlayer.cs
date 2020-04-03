using UnityEngine;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.AudioSystem
{
    /// <summary>
    /// A <see cref="AudioPlayer"/> that has a start, looping middle, and an end which triggers when <see cref="AudioManager.QueueRemoveAudioPlayer(AudioPlayer)"/>
    /// is called on this <see cref="MusicAudioPlayer"/>.
    /// </summary>
    [System.Serializable]
    public sealed class MusicAudioPlayer : AudioPlayer, IUpdate, IStart, IAwake
    {
        [SerializeField]
        private AudioClip s_StartClip, s_LoopClip, s_EndClip;

        private AudioClip playedClip;
        /// <summary>
        /// Returns the played clip based on start, loop or end clip based on various conditions.
        /// </summary>
        public override AudioClip Played => playedClip;

        private AudioSource ParentSource;

        private bool PastStart;
        private bool PastLoop;

        /// <summary>
        /// Returns true if the end clip has finished, otherwise returns false.
        /// </summary>
        public override bool Disposable => PastLoop ? ParentSource.time >= Played.length : false;

        /// <summary>
        /// Sets the played clip to the end clip.
        /// </summary>
        protected override void OnQueuedRemove()
        {
            playedClip = s_EndClip;
            ParentSource.loop = false;
            PastLoop = true;
        }

        void IUpdate.Update()
        {
            if(!PastStart)
            {
                if(ParentSource.time >= playedClip.length)
                {
                    playedClip = s_LoopClip;
                    ParentSource.loop = true;
                    PastStart = true;
                }
            }
        }

        void IStart.Start()
        {
            ParentSource = Parent.GetAudioSourceAtIndex(Index);
            ParentSource.loop = false;
        }

        void IAwake.Awake()
        {
            PastStart = PastLoop = false;
            playedClip = s_StartClip;

            //if (s_StartClip == s_EndClip)
            //    throw new System.Exception("The start clip and end clip of the MusicAudioPlayer are identical; This is not allowed.");
        }

        /// <summary>
        /// Creates a new <see cref="MusicAudioPlayer"/>.
        /// </summary>
        /// <param name="startClip"></param>
        /// <param name="loopClip"></param>
        /// <param name="endClip"></param>
        public MusicAudioPlayer(AudioClip startClip, AudioClip loopClip, AudioClip endClip)
        {
            s_StartClip = startClip;
            s_LoopClip = loopClip;
            s_EndClip = endClip;
        }

        /// <summary>
        /// Creates a duplicate of another <see cref="MusicAudioPlayer"/>.
        /// </summary>
        /// <param name="original"></param>
        public MusicAudioPlayer(MusicAudioPlayer original)
        {
            s_StartClip = original.s_StartClip;
            s_LoopClip = original.s_LoopClip;
            s_EndClip = original.s_EndClip;
        }
    }
}
