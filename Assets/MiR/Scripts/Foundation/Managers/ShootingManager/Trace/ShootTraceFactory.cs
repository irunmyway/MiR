using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class ShootTraceFactory : MonoInstaller
    {
        public int PoolSize = 32;
        public ShootTrace Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<Vector3, Vector3, ShootTrace, ShootTrace.Factory>()
                .FromMonoPoolableMemoryPool<Vector3, Vector3, ShootTrace>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform)
                );
        }
    }
}
