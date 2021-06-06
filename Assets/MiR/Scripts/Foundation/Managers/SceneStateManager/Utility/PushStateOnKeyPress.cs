using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class PushStateOnKeyPress : AbstractBehaviour, IOnUpdate
    {
        public string InputActionName;
        public SceneState State;

        [Inject] IPlayer player = default;
        [Inject] IInputManager inputManager = default;
        [Inject] ISceneState sceneState = default;
        [Inject] ISceneStateManager sceneStateManager = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            var input = inputManager.InputForPlayer(player.Index);
            if (input.Action(InputActionName).Triggered)
                sceneStateManager.Push(State);
        }
    }
}
