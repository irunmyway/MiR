using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Zenject;

namespace Foundation
{
    public sealed class SoundSourceFactory : MonoInstaller
    {
        public int PoolSize = 32;
        public SoundSource Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<AudioClip, SoundSource, SoundSource.Factory>()
                .FromMonoPoolableMemoryPool<AudioClip, SoundSource>(opts => {
                    opts.WithInitialSize(PoolSize);
                    if (Prefab == null)
                        opts.FromNewComponentOnNewGameObject().UnderTransform(transform);
                    else
                        opts.FromComponentInNewPrefab(Prefab).UnderTransform(transform);
                });
        }
    }
}
