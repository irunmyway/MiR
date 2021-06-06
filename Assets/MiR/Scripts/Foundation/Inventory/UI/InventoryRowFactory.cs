using Zenject;

namespace Foundation
{
    public sealed class InventoryRowFactory : MonoInstaller
    {
        public int PoolSize = 8;
        public InventoryRow Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<InventoryRow, InventoryRow.Factory>()
                .FromMonoPoolableMemoryPool<InventoryRow>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
