using UnityEngine;
using DG.Tweening;

namespace Foundation
{
    public struct SoundHandle
    {
        internal readonly SoundSource Source;
        internal readonly int HandleID;

        public readonly ISoundChannel Channel;
        public bool IsValid => (Source != null && Source.HandleID == HandleID);
        public bool IsPlaying => (IsValid ? Source.AudioSource.isPlaying : false);
        public AudioClip AudioClip => (IsValid ? Source.AudioSource.clip : null);

        public float Volume { get {
                DebugOnly.Check(IsValid, "Attempted to get volume with an invalid SoundHandle.");
                return Source.AudioSource.volume;
            } set {
                DebugOnly.Check(IsValid, "Attempted to set volume with an invalid SoundHandle.");
                Source.AudioSource.volume = value;
            } }

        public bool Loop { get {
                DebugOnly.Check(IsValid, "Attempted to get looping with an invalid SoundHandle.");
                return Source.AudioSource.loop;
            } set {
                DebugOnly.Check(IsValid, "Attempted to set looping with an invalid SoundHandle.");
                Source.AudioSource.loop = value;
            } }

        internal SoundHandle(ISoundChannel channel, SoundSource source)
        {
            Channel = channel;
            Source = source;
            HandleID = source.HandleID;
        }

        public void Stop()
        {
            Channel.Stop(this);
        }

        public Tweener DOFade(float endValue, float time)
        {
            DebugOnly.Check(IsValid, "Attempted to fade volume with an invalid SoundHandle.");
            return Source.DOFade(endValue, time);
        }

        public Tweener DOFadeToStop(float time)
        {
            DOKill(false);
            return DOFade(0.0f, time).OnComplete(Stop);
        }

        public void DOKill(bool complete)
        {
            DebugOnly.Check(IsValid, "Attempted to kill tweens with an invalid SoundHandle.");
            Source.DOKill(complete);
        }
    }
}
