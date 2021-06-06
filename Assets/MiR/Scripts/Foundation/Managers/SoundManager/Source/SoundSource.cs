using System;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Foundation
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundSource : MonoBehaviour, IPoolable<AudioClip, IMemoryPool>, IDisposable
    {
        public sealed class Factory : PlaceholderFactory<AudioClip, SoundSource>
        {
        }

        IMemoryPool pool;
        internal AudioSource AudioSource { get; private set; }
        internal int HandleID { get; private set; }
        [ReadOnly] [SerializeField] internal Transform TargetTransform;
        [ReadOnly] [SerializeField] internal bool SurviveSceneLoad;

        void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            AudioSource.playOnAwake = false;

          #if UNITY_EDITOR
            gameObject.name = "<Free>";
          #endif
        }

        public void Dispose()
        {
            pool.Despawn(this);
        }

        public void OnSpawned(AudioClip clip, IMemoryPool pool)
        {
            this.pool = pool;
            AudioSource.clip = clip;

          #if UNITY_EDITOR
            gameObject.name = (clip != null ? clip.name : "<Invalid>");
          #endif
        }

        public void OnDespawned()
        {
            pool = null;

          #if UNITY_EDITOR
            gameObject.name = "<Free>";
          #endif

            AudioSource.DOKill(false);
            if (AudioSource.isPlaying)
                AudioSource.Stop();

            TargetTransform = null;
            AudioSource.clip = null;
            HandleID++;
        }

        public Tweener DOFade(float endValue, float time)
        {
            return DOTween.To(
                    () => AudioSource.volume,
                    (value) => AudioSource.volume = value,
                    endValue,
                    time)
                .SetOptions(false)
                .SetTarget(this);
        }
    }
}
