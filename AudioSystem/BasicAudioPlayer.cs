using UnityEngine;

namespace WarWolfWorks.AudioSystem
{
    /// <summary>
    /// A simple <see cref="AudioPlayer"/> to play a single sound once, or in a loop.
    /// </summary>
    [System.Serializable]
    public sealed class BasicAudioPlayer : AudioPlayer
    {
        [SerializeField]
        private AudioClip s_Clip;
        [SerializeField]
        private bool s_Loops;

        /// <summary>
        /// Returns the clip set through the <see cref="BasicAudioPlayer"/> constructor or through the inspector.
        /// </summary>
        public override AudioClip Played => s_Clip;

        /// <summary>
        /// Returns false if this <see cref="BasicAudioPlayer"/> was set to loop, otherwise it will be removed if the
        /// clip used finished playing.
        /// </summary>
        public override bool Disposable => s_Loops ? false : Parent.GetAudioSourceAtIndex(Index).time >= Played.length;

        /// <summary>
        /// Creates a new <see cref="BasicAudioPlayer"/>.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="loops"></param>
        public BasicAudioPlayer(AudioClip clip, bool loops)
        {
            s_Clip = clip;
            s_Loops = loops;
        }
    }
}
