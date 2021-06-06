using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterInteractInput : AbstractBehaviour, IOnUpdate
    {
        public string InputActionName;

        ICharacterDialogs activeDialogs;
        VehicleEntrance activeVehicleEntrance;

        [Inject] IPlayer player = default;
        [Inject] IInputManager inputManager = default;
        [Inject] ISceneState sceneState = default;
        [Inject] IDialogUI dialogUI = default;

        [InjectOptional] ICharacterVehicle vehicle = default;

        public void OnTriggerEnter(Collider other)
        {
            var context = other.GetComponentInParent<Context>();
            if (context != null) {
                var dialogs = context.Container.TryResolve<ICharacterDialogs>();
                if (dialogs != null && activeDialogs == null) {
                    activeDialogs = dialogs;
                }
            }

            if (other.TryGetComponent<VehicleEntrance>(out var entrance))
                activeVehicleEntrance = entrance;
        }

        public void OnTriggerExit(Collider other)
        {
            var context = other.GetComponentInParent<Context>();
            if (context != null) {
                var dialogs = context.Container.TryResolve<ICharacterDialogs>();
                if (dialogs == activeDialogs)
                    activeDialogs = null;
            }

            if (other.TryGetComponent<VehicleEntrance>(out var entrance) && entrance == activeVehicleEntrance)
                activeVehicleEntrance = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            if (vehicle != null && vehicle.State != CharacterVehicleState.NotInVehicle)
                return;

            var input = inputManager.InputForPlayer(player.Index);
            bool triggered = input.Action(InputActionName).Triggered;

            if (activeDialogs != null) {
                if (triggered)
                    dialogUI.DisplayDialogs(player, activeDialogs.Portrait, activeDialogs.Dialogs);
            }

            if (activeVehicleEntrance != null) {
                if (triggered)
                    vehicle.TryEnterVehicle(activeVehicleEntrance);
            }
        }
    }
}
