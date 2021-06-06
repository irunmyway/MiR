using Zenject;

namespace Foundation
{
    [FactoryInstaller(typeof(DialogRow.Factory))]
    public sealed class DialogRowFactory : MonoInstaller
    {
        public int PoolSize = 8;
        public DialogRow Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<DialogRow, DialogRow.Factory>()
                .FromMonoPoolableMemoryPool<DialogRow>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
