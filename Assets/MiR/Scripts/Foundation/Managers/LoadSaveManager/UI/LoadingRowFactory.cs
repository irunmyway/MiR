using Zenject;

namespace Foundation
{
    [FactoryInstaller(typeof(LoadingRow.Factory))]
    public sealed class LoadingRowFactory : MonoInstaller
    {
        public int PoolSize = 8;
        public LoadingRow Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<SaveSlot, LoadingRow, LoadingRow.Factory>()
                .FromMonoPoolableMemoryPool<SaveSlot, LoadingRow>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
