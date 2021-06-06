using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace Foundation
{
    public sealed class PopStateReceiver : MonoBehaviour, INotificationReceiver
    {
        [Inject] ISceneStateManager manager = default;

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            var popStateMarker = notification as PopStateMarker;
            if (popStateMarker == null)
                return;

            manager.Pop(manager.CurrentState);
        }
    }
}
