using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputSource : AbstractBehaviour, IInputSource, IPoolable<IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<InputSource>
        {
        }

        PlayerInput playerInput;
        Dictionary<string, UnityInputAction> actions = new Dictionary<string, UnityInputAction>();

        void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            DisconnectAllActions();
        }

        public IInputAction Action(string name)
        {
            if (actions.TryGetValue(name, out var wrapper))
                return wrapper;

            var action = playerInput.actions.FindAction(name);
            DebugOnly.Check(action != null, $"Unable to find input action \"{name}\".");

            wrapper = new UnityInputAction(action);
            actions[name] = wrapper;

            return wrapper;
        }

        public void DisconnectAllActions()
        {
            foreach (var it in actions)
                it.Value.Dispose();
            actions.Clear();
        }

        public void OnSpawned(IMemoryPool pool)
        {
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            DisconnectAllActions();
        }
    }
}
