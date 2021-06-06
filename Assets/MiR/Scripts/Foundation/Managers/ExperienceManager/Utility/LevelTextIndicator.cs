using Cinemachine;
using UnityEngine;
using Zenject;
using TMPro;

namespace Foundation
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class LevelTextIndicator : AbstractBehaviour, IOnLevelChanged
    {
        TextMeshProUGUI text;

        public int PlayerIndex;
        public LocalizedString Format;

        [Inject] ILocalizationManager localizationManager = default;
        [Inject] IExperienceManager experienceManager = default;

        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(experienceManager.OnLevelChanged);
            UpdateText();
        }

        void IOnLevelChanged.Do(int player, int level)
        {
            if (PlayerIndex == player)
                UpdateText();
        }

        void UpdateText()
        {
            text.text = string.Format(localizationManager.GetString(Format),
                experienceManager.GetPlayerLevel(PlayerIndex));
        }
    }
}
