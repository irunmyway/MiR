using Cinemachine;
using UnityEngine;
using Zenject;
using TMPro;

namespace Foundation
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class HealthTextIndicator : AbstractBehaviour,
        IOnPlayerDamaged, IOnPlayerHealed, IOnPlayerAdded, IOnPlayerRemoved
    {
        TextMeshProUGUI text;

        public int PlayerIndex;
        public LocalizedString Format;

        [Inject] ILocalizationManager localizationManager = default;
        [Inject] IPlayerManager playerManager = default;

        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(playerManager.OnPlayerAdded);
            Observe(playerManager.OnPlayerRemoved);
            Observe(playerManager.OnPlayerDamaged);
            Observe(playerManager.OnPlayerHealed);
            UpdateText();
        }

        void IOnPlayerDamaged.Do(int player, IAttacker attacker, float amount, float newHealth)
        {
            if (PlayerIndex == player)
                UpdateText();
        }

        void IOnPlayerHealed.Do(int player, IAttacker attacker, float amount, float newHealth)
        {
            if (PlayerIndex == player)
                UpdateText();
        }

        void IOnPlayerAdded.Do(int player)
        {
            if (PlayerIndex == player)
                UpdateText();
        }

        void IOnPlayerRemoved.Do(int player)
        {
            if (PlayerIndex == player)
                UpdateText();
        }

        void UpdateText()
        {
            var player = playerManager.GetPlayer(PlayerIndex);
            if (player == null) {
                text.text = "";
                return;
            }

            var health = player.Health;
            if (health == null) {
                text.text = "";
                return;
            }

            text.text = string.Format(localizationManager.GetString(Format),
                health.Health, health.MaxHealth);
        }
    }
}
