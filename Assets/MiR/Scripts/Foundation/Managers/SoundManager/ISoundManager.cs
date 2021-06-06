using UnityEngine;

namespace Foundation
{
    public interface ISoundManager
    {
        ISoundChannel Sfx { get; }
        ISoundChannel Music { get; }

        ISoundChannel GetChannel(string name);

        void PlayMusic(AudioClip clip, float volume = 1.0f);
        void StopMusic();
    }
}
