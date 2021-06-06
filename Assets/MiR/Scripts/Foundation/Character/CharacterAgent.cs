using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class CharacterAgent : AbstractService<ICharacterAgent>, ICharacterAgent, IOnUpdate
    {
        NavMeshAgent agent;

        [InjectOptional] ICharacterHealth health = default;
        [InjectOptional] ICharacterVehicle vehicle = default;
        [Inject] ISceneState state = default;

        public Transform CharacterTransform;
        public bool UpdatePosition;
        public bool UpdateRotation;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        public void Move(Vector2 dir)
        {
            if (agent != null)
                agent.Move(new Vector3(dir.x, 0.0f, dir.y));
        }

        public void NavigateTo(Vector2 target)
        {
            if (agent != null) {
                agent.destination = new Vector3(target.x, transform.position.y, target.y);
                agent.isStopped = false;
            }
        }

        public void NavigateTo(Transform target)
        {
            if (agent != null) {
                agent.destination = new Vector3(target.position.x, transform.position.y, target.position.z);
                agent.isStopped = false;
            }
        }

        public void Look(Vector2 dir)
        {
            CharacterTransform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0.0f, dir.y));
            transform.localRotation = Quaternion.identity;
        }

        public void Stop()
        {
            if (agent != null)
                agent.isStopped = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            if (health != null && health.IsDead && agent != null) {
                Destroy(agent);
                agent = null;
                return;
            }

            if (vehicle != null) {
                if (vehicle.CurrentVehicle != null && agent.enabled) {
                    agent.enabled = false;
                    return;
                }

                if (vehicle.CurrentVehicle == null && !agent.enabled)
                    agent.enabled = true;
            }

            if (UpdatePosition) {
                CharacterTransform.position = transform.position;
                transform.localPosition = Vector3.zero;
            }

            if (UpdateRotation) {
                CharacterTransform.rotation = transform.rotation;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
