using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class ThirdPersonCamera : AbstractBehaviour, IThirdPersonCamera
    {
        [Inject] IPlayer player = default;
        [Inject] ICameraManager cameraManager = default;

        public int PlayerIndex => player.Index;
        public GameObject GameObject => gameObject;

        void Awake()
        {
            cameraManager.AddThirdPersonCamera(this);
        }

        void OnDestroy()
        {
            cameraManager.RemoveThirdPersonCamera(this);
        }
    }
}
