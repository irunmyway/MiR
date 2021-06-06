using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Foundation
{
    public sealed class KeyBindingUI : AbstractBehaviour
    {
        [SerializeField] TextMeshProUGUI actionLabel;
        [SerializeField] TextMeshProUGUI bindingText;

        InputAction inputAction;
        string bindingId;
        InputActionRebindingExtensions.RebindingOperation rebindOperation;

        static List<KeyBindingUI> allKeyBindingUIs = new List<KeyBindingUI>();

        public void Init(InputAction action, string binding)
        {
            inputAction = action;
            bindingId = binding;
            UpdateUI();
        }

        void UpdateUI()
        {
            string displayString = "";
            if (inputAction != null) {
                var bindingIndex = inputAction.bindings.IndexOf(x => x.id.ToString() == bindingId);
                if (bindingIndex != -1)
                    displayString = inputAction.GetBindingDisplayString(bindingIndex, out var deviceLayoutName, out var controlPath, 0);
            }

            if (actionLabel != null)
                actionLabel.text = inputAction != null ? inputAction.name : "";

            if (bindingText != null)
                bindingText.text = displayString;
        }

        public bool ResolveActionAndBinding(out int bindingIndex)
        {
            bindingIndex = -1;

            if (inputAction == null)
                return false;

            if (string.IsNullOrEmpty(bindingId))
                return false;

            var bindingGuid = new Guid(bindingId);
            bindingIndex = inputAction.bindings.IndexOf(x => x.id == bindingGuid);
            if (bindingIndex == -1) {
                DebugOnly.Error($"Cannot find binding with ID '{bindingId}' on '{inputAction}'");
                return false;
            }

            return true;
        }

        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var bindingIndex))
                return;

            var action = inputAction;
            if (!action.bindings[bindingIndex].isComposite)
                action.RemoveBindingOverride(bindingIndex);
            else {
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }

            UpdateUI();
        }

        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var bindingIndex))
                return;

            var action = inputAction;
            if (!action.bindings[bindingIndex].isComposite)
                PerformInteractiveRebind(action, bindingIndex);
            else {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            rebindOperation?.Cancel();

            action.Disable();

            rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape")
                .OnCancel(op => {
                        action.Enable();
                        UpdateUI();
                        rebindOperation?.Dispose();
                        rebindOperation = null;
                    })
                .OnComplete(op => {
                        action.Enable();

                        if (CheckDuplicateBindings(action, bindingIndex, allCompositeParts)) {
                            action.RemoveBindingOverride(bindingIndex);
                            PerformInteractiveRebind(action, bindingIndex, allCompositeParts);
                            return;
                        }

                        UpdateUI();
                        rebindOperation?.Dispose();
                        rebindOperation = null;

                        if (allCompositeParts) {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            var partName = "";
            if (action.bindings[bindingIndex].isPartOfComposite)
                partName = $"Binding '{action.bindings[bindingIndex].name}'.";

            string waitingText = !string.IsNullOrEmpty(rebindOperation.expectedControlType)
                ? $"Waiting {partName} {rebindOperation.expectedControlType}"
                : $"Waiting {partName} input...";

            if (bindingText != null)
                bindingText.text = waitingText;

            rebindOperation.Start();
        }

        private bool CheckDuplicateBindings(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            InputBinding newBinding = action.bindings[bindingIndex];
            foreach (InputBinding binding in action.actionMap.bindings) {
                if (binding.action == newBinding.action)
                    continue;

                if (binding.effectivePath == newBinding.effectivePath) {
                    DebugOnly.Warn("Duplicate binding found: " + newBinding.effectivePath);
                    return true;
                }
            }

            if (allCompositeParts) {
                for (int i = 1; i < bindingIndex; ++i) {
                    if (action.bindings[i].effectivePath == newBinding.effectivePath) {
                        DebugOnly.Warn("Duplicate binding found: " + newBinding.effectivePath);
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            allKeyBindingUIs.Add(this);
            if (allKeyBindingUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            rebindOperation?.Dispose();
            rebindOperation = null;

            allKeyBindingUIs.Remove(this);
            if (allKeyBindingUIs.Count == 0)
                InputSystem.onActionChange -= OnActionChange;
        }

        static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            var action = obj as InputAction;
            var actionMap = action?.actionMap ?? obj as InputActionMap;
            var actionAsset = (actionMap != null ? actionMap.asset as InputActionAsset : null);

            for (var i = 0; i < allKeyBindingUIs.Count; i++) {
                var ui = allKeyBindingUIs[i];
                var referencedAction = ui.inputAction;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    ui.UpdateUI();
            }
        }
    }
}
