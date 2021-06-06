using UnityEngine;
using UnityEngine.Audio;

namespace Foundation
{
    public interface ISoundChannel
    {
        string Name { get; }
        bool Enabled { get; set; }
        float Volume { get; set; }

        SoundHandle Play(AudioClip clip, bool loop = false, bool surviveSceneLoad = false, float volume = 1.0f);
        SoundHandle PlayAt(GameObject gameObject, AudioClip clip, bool loop = false, bool surviveSceneLoad = false, float volume = 1.0f);
        SoundHandle PlayAt(Component component, AudioClip clip, bool loop = false, bool surviveSceneLoad = false, float volume = 1.0f);
        SoundHandle PlayAt(Transform transform, AudioClip clip, bool loop = false, bool surviveSceneLoad = false, float volume = 1.0f);

        void Stop(SoundHandle sound);
        void StopAllSounds(bool includingSurviveSceneLoad = true);
    }
}
