using Zenject;

namespace Foundation
{
    [FactoryInstaller(typeof(DialogButton.Factory))]
    public sealed class DialogButtonFactory : MonoInstaller
    {
        public int PoolSize = 8;
        public DialogButton Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<string, DialogButton, DialogButton.Factory>()
                .FromMonoPoolableMemoryPool<string, DialogButton>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
