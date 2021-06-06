using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterJumpInput : AbstractService<ICharacterJumpInput>, ICharacterJumpInput, IOnUpdate
    {
        public string InputActionName;

        public ObserverList<IOnCharacterJump> OnCharacterJump { get; } = new ObserverList<IOnCharacterJump>();

        [Inject] IPlayer player = default;
        [Inject] IInputManager inputManager = default;
        [Inject] ISceneState sceneState = default;

        [InjectOptional] ICharacterVehicle vehicle = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            if (vehicle != null && vehicle.CurrentVehicle != null)
                return;

            var input = inputManager.InputForPlayer(player.Index);
            if (input.Action(InputActionName).Triggered) {
                

                foreach (var observer in OnCharacterJump.Enumerate())
                    observer.Do();
            }
        }
    }
}
