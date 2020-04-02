using UnityEngine;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.AudioSystem
{
    /// <summary>
    /// A <see cref="AudioPlayer"/> that has a start, looping middle, and an end which triggers when <see cref="AudioManager.QueueRemoveAudioPlayer(AudioPlayer)"/>
    /// is called on this <see cref="MusicAudioPlayer"/>.
    /// </summary>
    [System.Serializable]
    public sealed class MusicAudioPlayer : AudioPlayer, IUpdate, IStart
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
                    PastStart = true;
                    playedClip = s_LoopClip;
                    ParentSource.loop = true;
                }
            }
        }

        void IStart.Start()
        {
            ParentSource = Parent.GetAudioSourceAtIndex(Index);
            ParentSource.loop = false;

            playedClip = s_StartClip;
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
    }
}
