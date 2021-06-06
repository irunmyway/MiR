using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterHorizontalLookInput : AbstractBehaviour, IOnUpdate
    {
        public string InputActionName;
        public Transform CharacterTransform;
        public Transform EyesTransform;
        public float RotationSpeed;

        [Inject] IPlayer player = default;
        [Inject] IInputManager inputManager = default;
        [Inject] ISceneState sceneState = default;

        [InjectOptional] ICharacterVehicle vehicle = default;

        float inCarAngle;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            var input = inputManager.InputForPlayer(player.Index);
            var dir = input.Action(InputActionName).Vector2Value;

            if (Mathf.Approximately(dir.x, 0.0f))
                return;

            float angleDelta = dir.x * RotationSpeed * timeDelta;

            if (vehicle != null && vehicle.CurrentVehicle != null)
                inCarAngle += angleDelta;
            else {
                CharacterTransform.localRotation *= Quaternion.AngleAxis(angleDelta, Vector3.up);
                inCarAngle = 0.0f;
            }

            var angles = EyesTransform.localEulerAngles;
            angles.y = inCarAngle;
            EyesTransform.localEulerAngles = angles;
        }
    }
}
