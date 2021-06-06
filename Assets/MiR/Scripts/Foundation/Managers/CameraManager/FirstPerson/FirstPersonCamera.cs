using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class FirstPersonCamera : AbstractBehaviour, IFirstPersonCamera
    {
        [Inject] IPlayer player = default;
        [Inject] ICameraManager cameraManager = default;

        public int PlayerIndex => player.Index;
        public GameObject GameObject => gameObject;

        void Awake()
        {
            cameraManager.AddFirstPersonCamera(this);
        }

        void OnDestroy()
        {
            cameraManager.RemoveFirstPersonCamera(this);
        }
    }
}
