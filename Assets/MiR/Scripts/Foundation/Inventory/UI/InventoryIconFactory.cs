using Zenject;

namespace Foundation
{
    public sealed class InventoryIconFactory : MonoInstaller
    {
        public int PoolSize = 64;
        public InventoryIcon Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<AbstractInventoryItem, int, InventoryIcon, InventoryIcon.Factory>()
                .FromMonoPoolableMemoryPool<AbstractInventoryItem, int, InventoryIcon>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
