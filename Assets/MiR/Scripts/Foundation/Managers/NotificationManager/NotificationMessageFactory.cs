using TMPro;
using Zenject;

namespace Foundation
{
    public sealed class NotificationMessageFactory : MonoInstaller
    {
        public int PoolSize = 16;
        public TextMeshProUGUI Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<string, NotificationMessage, NotificationMessage.Factory>()
                .FromMonoPoolableMemoryPool<string, NotificationMessage>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
