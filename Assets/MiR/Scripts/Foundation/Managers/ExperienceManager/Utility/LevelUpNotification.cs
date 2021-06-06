using Cinemachine;
using UnityEngine;
using Zenject;
using TMPro;

namespace Foundation
{
    public sealed class LevelUpNotification : AbstractBehaviour, IOnLevelReached
    {
        public int PlayerIndex;
        public LocalizedString Message;

        [Inject] ILocalizationManager localizationManager = default;
        [Inject] INotificationManager notificationManager = default;
        [Inject] IExperienceManager experienceManager = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(experienceManager.OnLevelReached);
        }

        void IOnLevelReached.Do(int player, int level)
        {
            if (PlayerIndex == player)
                notificationManager.DisplayMessage(localizationManager.GetString(Message));
        }
    }
}
