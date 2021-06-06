using UnityEngine;
using UnityEngine.InputSystem;

namespace Foundation
{
    public class InputSettings : AbstractBehaviour
    {
        [SerializeField] GameObject keyBindingPrefab;
        [SerializeField] Transform container;
        [SerializeField] InputActionAsset actions;

        void Awake()
        {
            InputActionMap actionMap = actions.FindActionMap("Player");
            foreach (var action in actionMap.actions) {
                /*
                bool isKeyboardOrMouse = false;
                foreach (var control in action.controls) {
                    if (control.device is Keyboard) {
                        isKeyboardOrMouse = true;
                        break;
                    } else if (control.device is Mouse) {
                        isKeyboardOrMouse = true;
                        break;
                    }
                }

                if (!isKeyboardOrMouse)
                    continue;
                */

                foreach (var binding in action.bindings) {
                    if (binding.isPartOfComposite)
                        continue;

                    var bindingUI = Instantiate(keyBindingPrefab, container).GetComponent<KeyBindingUI>();
                    bindingUI.Init(action, binding.id.ToString());
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // var rebinds = PlayerPrefs.GetString("rebinds");
            // if (!string.IsNullOrEmpty(rebinds))
            //     actions.LoadBindingOverridesFromJson(rebinds);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // var rebinds = actions.SaveBindingOverridesAsJson();
            // PlayerPrefs.SetString("rebinds", rebinds);
        }
    }
}
