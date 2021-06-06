using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterVehicleInput : AbstractService<ICharacterVehicleInput>, IOnUpdate
    {
        public string MovementInputActionName;
        public string InteractInputActionName;

        [Inject] IPlayer player = default;
        [Inject] IInputManager inputManager = default;
        [Inject] ICharacterVehicle vehicle = default;
        [Inject] ISceneState sceneState = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            if (vehicle == null)
                return;

            var input = inputManager.InputForPlayer(player.Index);

            if (vehicle.State == CharacterVehicleState.Driving) {
                var dir = input.Action(MovementInputActionName).Vector2Value;
                vehicle.CurrentVehicle.Forward = dir.y;
                vehicle.CurrentVehicle.Turn = dir.x;
            }

            if (vehicle.State == CharacterVehicleState.Driving || vehicle.State == CharacterVehicleState.Sitting) {
                if (input.Action(InteractInputActionName).Triggered)
                    vehicle.TryExitVehicle();
            }
        }
    }
}
