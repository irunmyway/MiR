using UnityEngine;
using System.Linq;
using TMPro;

namespace Foundation
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class GraphicsQualityDropdown : AbstractBehaviour
    {
        TMP_Dropdown dropdown;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            dropdown.AddOptions(QualitySettings.names.ToList());
            dropdown.value = QualitySettings.GetQualityLevel();
        }

        public void Apply()
        {
            QualitySettings.SetQualityLevel(dropdown.value);
        }
    }
}
