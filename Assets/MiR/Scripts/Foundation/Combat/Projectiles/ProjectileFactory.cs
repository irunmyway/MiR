using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Zenject;

namespace Foundation
{
    public sealed class ProjectileFactory : MonoInstaller
    {
        public AbstractWeapon Weapon;
        public AbstractProjectile Prefab;
        public int PoolSize = 32;

        public override void InstallBindings()
        {
            Container.BindFactory<Transform, AbstractProjectile, AbstractProjectile.Factory>()
                .WithId(Weapon.GetType().Name)
                .FromMonoPoolableMemoryPool<Transform, AbstractProjectile>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform)
                );
        }
    }
}
