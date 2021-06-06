using Cinemachine;
using UnityEngine;
using Zenject;
using TMPro;

namespace Foundation
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class ExperienceTextIndicator : AbstractBehaviour, IOnExperienceChanged
    {
        TextMeshProUGUI text;

        public int PlayerIndex;
        public LocalizedString Format;
        public LocalizedString MaxLevelMessage;

        [Inject] ILocalizationManager localizationManager = default;
        [Inject] IExperienceManager experienceManager = default;

        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(experienceManager.OnExperienceChanged);
            UpdateText();
        }

        void IOnExperienceChanged.Do(int player, int experience)
        {
            if (PlayerIndex == player)
                UpdateText();
        }

        void UpdateText()
        {
            if (experienceManager.IsPlayerMaxLevel(PlayerIndex))
                text.text = localizationManager.GetString(MaxLevelMessage);
            else {
                text.text = string.Format(localizationManager.GetString(Format),
                    experienceManager.GetPlayerExperience(PlayerIndex),
                    experienceManager.GetPlayerExperienceForNextLevel(PlayerIndex));
            }
        }
    }
}
