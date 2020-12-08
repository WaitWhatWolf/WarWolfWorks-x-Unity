using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.AudioSystem
{
    /// <summary>
    /// Used by <see cref="AudioManager"/> to play a sound.
    /// (Supported interfaces: <see cref="IAwake"/>, <see cref="IStart"/>, <see cref="IUpdate"/>, <see cref="IFixedUpdate"/>, <see cref="IOnDestroy"/>)
    /// </summary>
    public abstract class AudioPlayer : IParentable<AudioManager>
    {
        /// <summary>
        /// The <see cref="AudioManager"/> parent.
        /// </summary>
        public AudioManager Parent { get; internal set; }

        /// <summary>
        /// The index of this <see cref="AudioPlayer"/> in it's <see cref="AudioManager"/> parent.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// The currently played audio clip.
        /// </summary>
        public abstract AudioClip Played { get; }

        /// <summary>
        /// As long as this returns false, <see cref="AudioManager.RemoveAudioPlayer(AudioPlayer)"/> will not remove this <see cref="AudioPlayer"/>.
        /// </summary>
        public abstract bool Disposable { get; }

        internal void QueueRemoved()
            => OnQueuedRemove();

        /// <summary>
        /// Invoked when <see cref="AudioManager.QueueRemoveAudioPlayer(AudioPlayer)"/> was used on this <see cref="AudioPlayer"/>.
        /// </summary>
        protected virtual void OnQueuedRemove() { }
    }
}
