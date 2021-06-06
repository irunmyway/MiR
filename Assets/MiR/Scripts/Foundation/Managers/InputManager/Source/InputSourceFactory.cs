using Zenject;

namespace Foundation
{
    public sealed class InputSourceFactory : MonoInstaller
    {
        public int PoolSize = 1;
        public InputSource Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<InputSource, InputSource.Factory>()
                .FromMonoPoolableMemoryPool<InputSource>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
