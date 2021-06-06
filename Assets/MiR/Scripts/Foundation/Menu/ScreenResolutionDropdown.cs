using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Foundation
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ScreenResolutionDropdown : AbstractBehaviour
    {
        [SerializeField] FullScreenToggle fullScreenToggle;
        TMP_Dropdown dropdown;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();

            var strings = new List<string>();
            int current = 0;

            int index = 0;
            foreach (var resolution in Screen.resolutions) {
                strings.Add($"{resolution.width} x {resolution.height} ({resolution.refreshRate} Hz)");
                if (resolution.width == Screen.currentResolution.width
                        && resolution.height == Screen.currentResolution.height
                        && resolution.refreshRate == Screen.currentResolution.refreshRate)
                    current = index;
                ++index;
            }

            dropdown.AddOptions(strings);
            dropdown.value = current;
        }

        public void Apply()
        {
            var resolution = Screen.resolutions[dropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, fullScreenToggle.IsOn, resolution.refreshRate);
        }
    }
}
