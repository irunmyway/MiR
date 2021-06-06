using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Zenject;

namespace Foundation
{
    public sealed class SoundManager : AbstractService<ISoundManager>, ISoundManager, IOnCurrentSceneUnload
    {
        public float MusicFadeTime = 1.0f;

        public AudioMixer Mixer;
        public AudioListener Listener;
        public SoundChannel[] Channels;

        [Inject] ISceneManager sceneManager = default;
        SoundHandle currentMusic;
        Dictionary<string, ISoundChannel> channelDict;

        public ISoundChannel Sfx { get; private set; }
        public ISoundChannel Music { get; private set; }

        void Awake()
        {
            channelDict = new Dictionary<string, ISoundChannel>();
            foreach (var channel in Channels) {
                DebugOnly.Check(!channelDict.ContainsKey(channel.Name), $"Duplicate channel name: '{channel.Name}'.");
                channelDict[channel.Name] = channel;
                channel.InternalInit(Mixer);
            }

            Sfx = GetChannel("Sfx");
            Music = GetChannel("Music");
        }

        public ISoundChannel GetChannel(string name)
        {
            if (channelDict.TryGetValue(name, out var channel))
                return channel;
            DebugOnly.Error($"Sound channel '{name}' was not found.");
            return null;
        }

        public void PlayMusic(AudioClip clip, float volume)
        {
            if (currentMusic.IsPlaying) {
                if (currentMusic.AudioClip == clip) {
                    currentMusic.Volume = volume;
                    return;
                }

                currentMusic.DOFadeToStop(MusicFadeTime);
            }

            currentMusic = Music.Play(clip, true, true, 0.0f);
            currentMusic.DOKill(false);
            currentMusic.DOFade(1.0f, MusicFadeTime);
        }

        public void StopMusic()
        {
            if (currentMusic.IsPlaying)
                currentMusic.DOFadeToStop(MusicFadeTime);

            currentMusic = new SoundHandle();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneManager.OnCurrentSceneUnload);
        }

        void IOnCurrentSceneUnload.Do()
        {
            foreach (var channel in Channels)
                channel.StopAllSounds(false);
        }

        void Update()
        {
            foreach (var channel in Channels)
                channel.InternalUpdate(Mixer, Listener);
        }
    }
}
