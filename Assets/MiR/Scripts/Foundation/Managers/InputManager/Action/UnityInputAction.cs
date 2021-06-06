using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Foundation
{
    public sealed class UnityInputAction : IInputAction, IDisposable
    {
        InputAction action;

        public bool Active { get; private set; }
        public bool Triggered => (action != null ? action.triggered : false);
        public Vector2 Vector2Value => (action != null ? action.ReadValue<Vector2>() : Vector2.zero);

        public UnityInputAction(InputAction action)
        {
            this.action = action;

            if (action != null) {
                action.started += OnStarted;
                action.canceled += OnCanceled;
            }
        }

        public void Dispose()
        {
            Active = false;

            if (action != null) {
                action.started -= OnStarted;
                action.canceled -= OnCanceled;
                action = null;
            }
        }

        void OnStarted(InputAction.CallbackContext ctx)
        {
            Active = true;
        }

        void OnCanceled(InputAction.CallbackContext ctx)
        {
            Active = false;
        }
    }
}
