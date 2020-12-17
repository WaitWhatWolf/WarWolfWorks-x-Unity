using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.AudioSystem
{
    /// <summary>
    /// Core class to handle audio.
    /// </summary>
    [CompleteNoS]
    public sealed class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private int s_PoolSize;

        [SerializeField]
        private AudioMixerGroup s_DefaultMixerGroup;

        private List<AudioSource> ns_AudioSources;
        private List<AudioPlayer> ns_AudioPlayers;

        private int PreviousIndex;

        /// <summary>
        /// The default <see cref="AudioMixerGroup"/> that will be assigned to new <see cref="AudioSource"/> components.
        /// </summary>
        public AudioMixerGroup DefaultAudioMixerGroup
        {
            get => s_DefaultMixerGroup;
            set
            {
                foreach (AudioSource source in ns_AudioSources)
                    source.outputAudioMixerGroup = value;
                s_DefaultMixerGroup = value;
            }
        }

        private void Awake()
        {
            ns_AudioSources = new List<AudioSource>(s_PoolSize);
            ns_AudioPlayers = new List<AudioPlayer>(s_PoolSize);

            if (s_PoolSize != 0)
                Init(s_PoolSize);
        }

        /// <summary>
        /// Returns the <see cref="AudioSource"/> component on the specified index.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public AudioSource GetAudioSourceAtIndex(int of)
            => ns_AudioSources[of];

        /// <summary>
        /// Returns the <see cref="AudioPlayer"/> element on the specified index.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public AudioPlayer GetAudioPlayerAtIndex(int of)
            => ns_AudioPlayers[of];

        /// <summary>
        /// Initiates the <see cref="AudioManager"/>.
        /// </summary>
        /// <param name="poolSize"></param>
        public void Init(int poolSize)
        {
            if (poolSize < V_AUDIOMANAGER_MIN_POOLSIZE)
                throw new System.Exception(string.Format("Cannot initiate a pool with a poolsize of less than {0}.", V_AUDIOMANAGER_MIN_POOLSIZE));

            if (ns_AudioSources.Count == poolSize)
                throw new System.Exception("Cannot re-initiate an audio manager with the same pool size.");

            for (int i = ns_AudioSources.Count - 1; i > 0; i--)
            {
                Destroy(ns_AudioSources[i]);
            }

            ns_AudioSources.Clear();
            ns_AudioPlayers.Clear();

            for (int i = 0; i < poolSize; i++)
            {
                ns_AudioSources.Add(NewAudioSource());
                ns_AudioPlayers.Add(null);
            }
        }

        private AudioSource NewAudioSource()
        {
            AudioSource used = gameObject.AddComponent<AudioSource>();
            used.outputAudioMixerGroup = s_DefaultMixerGroup;
            return used;
        }

        /// <summary>
        /// Adds an audio player to the list of used <see cref="AudioPlayer"/> elements.
        /// Returns the <see cref="AudioSource"/> used by the given <see cref="AudioPlayer"/> to change it's settings.
        /// </summary>
        /// <param name="audioPlayer"></param>
        public AudioSource AddAudioPlayer(AudioPlayer audioPlayer)
        {
            PreviousIndex = Hooks.Enumeration.GetEmptyIndex(ns_AudioPlayers);
            if(PreviousIndex == -1)
            {
                PreviousIndex = ns_AudioPlayers.Count;
                ns_AudioPlayers.Add(audioPlayer);
                ns_AudioSources.Add(NewAudioSource());
            }

            audioPlayer.Index = PreviousIndex;
            audioPlayer.Parent = this;
            ns_AudioPlayers[PreviousIndex] = audioPlayer;

            if (ns_AudioPlayers[PreviousIndex] is IAwake awake)
                awake.Awake();

            ns_AudioSources[PreviousIndex].clip = ns_AudioPlayers[PreviousIndex].Played;
            ns_AudioSources[PreviousIndex].Play();

            if (ns_AudioPlayers[PreviousIndex] is IStart start)
                start.Start();

            return ns_AudioSources[PreviousIndex];
        }

        /// <summary>
        /// Attempts to remove a <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="audioPlayer"></param>
        /// <returns></returns>
        public bool RemoveAudioPlayer(AudioPlayer audioPlayer)
        {
            if (audioPlayer.Disposable)
            {
                if (ns_AudioPlayers[audioPlayer.Index] != null)
                {
                    ns_AudioSources[audioPlayer.Index].Stop();
                    ns_AudioSources[audioPlayer.Index].clip = null;
                    if (ns_AudioPlayers[audioPlayer.Index] is IOnDestroy destroy)
                        destroy.OnDestroy();
                    ns_AudioPlayers[audioPlayer.Index] = null;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to remove a <see cref="AudioPlayer"/> ignoring it's <see cref="AudioPlayer.Disposable"/>.
        /// </summary>
        /// <param name="audioPlayer"></param>
        /// <returns></returns>
        public bool ForceRemoveAudioPlayer(AudioPlayer audioPlayer)
        {
            if (ns_AudioPlayers[audioPlayer.Index] != null)
            {
                ns_AudioSources[audioPlayer.Index].Stop();
                ns_AudioSources[audioPlayer.Index].clip = null;
                if (ns_AudioPlayers[audioPlayer.Index] is IOnDestroy destroy)
                    destroy.OnDestroy();
                ns_AudioPlayers[audioPlayer.Index] = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Queues the removal of a <see cref="AudioPlayer"/>; It will remove the specified <see cref="AudioPlayer"/> as soon as it's <see cref="AudioPlayer.Disposable"/>
        /// returns true.
        /// </summary>
        /// <param name="audioPlayer"></param>
        public void QueueRemoveAudioPlayer(AudioPlayer audioPlayer)
        {
            if (!ns_AudioPlayers.Contains(audioPlayer))
            {
                AdvancedDebug.LogWarning("Couldn't queue an AudioPlayer for removal as it is not contained in the list of active audio players.", DEBUG_LAYER_WWW_INDEX);
                return;
            }

            audioPlayer.QueueRemoved();
            AVT_RemoveAudioPlayer(audioPlayer);
        }

        private async void AVT_RemoveAudioPlayer(AudioPlayer player)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (player.Disposable)
                    {
                        ns_AudioPlayers[player.Index] = null;
                        Threading.ThreadingUtilities.QueueOnMainThread(() => 
                        {
                            ns_AudioSources[player.Index].Stop();
                            ns_AudioSources[player.Index].clip = null;
                            if (ns_AudioPlayers[player.Index] is IOnDestroy destroy)
                                destroy.OnDestroy();
                        });
                        break;
                    }
                };
            });
        }

        private void Update()
        {
            for(int i = 0; i < ns_AudioPlayers.Count; i++)
            {
                if (ns_AudioPlayers[i] == null)
                    continue;

                if (ns_AudioPlayers[i] is IUpdate update)
                    update.Update();

                if (ns_AudioPlayers[i].Played != ns_AudioSources[i].clip)
                {
                    ns_AudioSources[i].clip = ns_AudioPlayers[i].Played;
                    ns_AudioSources[i].Play();
                }
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < ns_AudioPlayers.Count; i++)
            {
                if (ns_AudioPlayers[i] == null)
                    continue;

                if (ns_AudioPlayers[i] is IFixedUpdate fixedUpdate)
                    fixedUpdate.FixedUpdate();
            }
        }
    }
}
