using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class ExperienceManager : AbstractService<IExperienceManager>, IExperienceManager, IOnPlayerRemoved
    {
        [Serializable]
        public sealed class PerPlayer
        {
            readonly int playerIndex;
            readonly ExperienceManager manager;

            int level = 1;
            int experience;

            public int Level => level;
            public int Experience => experience;

            public PerPlayer(ExperienceManager manager, int playerIndex)
            {
                this.manager = manager;
                this.playerIndex = playerIndex;
            }

            public void Reset()
            {
                if (level != 1) {
                    level = 1;
                    foreach (var it in manager.OnLevelChanged.Enumerate())
                        it.Do(playerIndex, level);
                }

                if (experience != 0) {
                    experience = 0;
                    foreach (var it in manager.OnExperienceChanged.Enumerate())
                        it.Do(playerIndex, experience);
                }
            }

            public void AddExperience(int value)
            {
                experience += value;

                var levels = manager.thresholds.ExperienceLevels;
                if (level <= levels.Length) {
                    bool levelReached = false;
                    while (level <= levels.Length && experience >= levels[level - 1]) {
                        experience -= levels[level - 1];
                        levelReached = true;
                        ++level;
                    }

                    if (levelReached) {
                        foreach (var it in manager.OnLevelChanged.Enumerate())
                            it.Do(playerIndex, level);
                        foreach (var it in manager.OnLevelReached.Enumerate())
                            it.Do(playerIndex, level);
                    }
                }

                foreach (var it in manager.OnExperienceChanged.Enumerate())
                    it.Do(playerIndex, experience);
            }
        }

        public LevelUpThresholds thresholds;

        [Inject] IPlayerManager playerManager = default;

        List<PerPlayer> perPlayer = new List<PerPlayer>();

        public ObserverList<IOnExperienceGained> OnExperienceGained { get; } = new ObserverList<IOnExperienceGained>();
        public ObserverList<IOnExperienceChanged> OnExperienceChanged { get; } = new ObserverList<IOnExperienceChanged>();
        public ObserverList<IOnLevelReached> OnLevelReached { get; } = new ObserverList<IOnLevelReached>();
        public ObserverList<IOnLevelChanged> OnLevelChanged { get; } = new ObserverList<IOnLevelChanged>();

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(playerManager.OnPlayerRemoved);
        }

        public void ResetAllPlayers()
        {
            foreach (var it in perPlayer) {
                if (it != null)
                    it.Reset();
            }

            perPlayer.Clear();
        }

        void IOnPlayerRemoved.Do(int playerIndex)
        {
            if (playerIndex >= 0 && playerIndex < perPlayer.Count)
                perPlayer[playerIndex] = null;
        }

        public void AddExperience(int player, int experience)
        {
            DebugOnly.Check(player >= 0, "Invalid player.");
            DebugOnly.Check(experience > 0, "Invalid experience amount.");

            foreach (var it in OnExperienceGained.Enumerate())
                it.Do(player, experience);

            while (player >= perPlayer.Count)
                perPlayer.Add(null);

            var playerInfo = perPlayer[player];
            if (playerInfo == null) {
                playerInfo = new PerPlayer(this, player);
                perPlayer[player] = playerInfo;
            }

            playerInfo.AddExperience(experience);
        }

        public int GetPlayerLevel(int player)
        {
            DebugOnly.Check(player >= 0, "Invalid player.");
            return (player >= 0 && player < perPlayer.Count ? perPlayer[player].Level : 1);
        }

        public bool IsPlayerMaxLevel(int player)
        {
            int level = GetPlayerLevel(player);
            return (level - 1) >= thresholds.ExperienceLevels.Length;
        }

        public int GetPlayerExperience(int player)
        {
            DebugOnly.Check(player >= 0, "Invalid player.");
            return (player >= 0 && player < perPlayer.Count ? perPlayer[player].Experience : 0);
        }

        public int GetPlayerExperienceForNextLevel(int player)
        {
            int level = GetPlayerLevel(player);

            var levels = thresholds.ExperienceLevels;
            if (level - 1 < levels.Length)
                return levels[level - 1];

            return 0;
        }
    }
}
