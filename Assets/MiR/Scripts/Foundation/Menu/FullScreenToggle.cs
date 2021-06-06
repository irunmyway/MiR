using UnityEngine;
using UnityEngine.UI;

namespace Foundation
{
    [RequireComponent(typeof(Toggle))]
    public class FullScreenToggle : AbstractBehaviour
    {
        Toggle toggle;
        public bool IsOn => toggle.isOn;

        void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.isOn = Screen.fullScreen;
        }

        public void Apply()
        {
            Screen.fullScreen = toggle.isOn;
        }
    }
}
