using Cinemachine;
using UnityEngine;
using Zenject;
using TMPro;

namespace Foundation
{
    public sealed class LevelUpSound : AbstractBehaviour, IOnLevelReached
    {
        public int PlayerIndex;
        public AudioClip sound;

        [Inject] ISoundManager soundManager = default;
        [Inject] IExperienceManager experienceManager = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(experienceManager.OnLevelReached);
        }

        void IOnLevelReached.Do(int player, int level)
        {
            if (PlayerIndex == player)
                soundManager.Sfx.Play(sound);
        }
    }
}
