using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class MovingForwardProjectile : AbstractProjectile, IOnFixedUpdate
    {
        [Inject] ISceneState state = default;
        public float Speed = 1.0f;

        Rigidbody physicsBody;

        void Awake()
        {
            physicsBody = GetComponent<Rigidbody>();
            physicsBody.isKinematic = true;
            physicsBody.velocity = Vector3.zero;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnFixedUpdate);
        }

        void IOnFixedUpdate.Do()
        {
            if (!Launched) {
                physicsBody.isKinematic = true;
                physicsBody.velocity = Vector3.zero;
            } else {
                physicsBody.isKinematic = false;
                physicsBody.velocity = Speed * transform.forward;
            }
        }
    }
}
