using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class PlayerManager : AbstractService<IPlayerManager>, IPlayerManager
    {
        public int NumPlayers { get; private set; }

        public ObserverList<IOnPlayerAdded> OnPlayerAdded { get; } = new ObserverList<IOnPlayerAdded>();
        public ObserverList<IOnPlayerRemoved> OnPlayerRemoved { get; } = new ObserverList<IOnPlayerRemoved>();

        public ObserverList<IOnPlayerDamaged> OnPlayerDamaged { get; } = new ObserverList<IOnPlayerDamaged>();
        public ObserverList<IOnPlayerDied> OnPlayerDied { get; } = new ObserverList<IOnPlayerDied>();
        public ObserverList<IOnPlayerHealed> OnPlayerHealed { get; } = new ObserverList<IOnPlayerHealed>();

        List<IPlayer> players = new List<IPlayer>();

        public void AddPlayer(IPlayer player, out int index, bool reuseSlots)
        {
            if (reuseSlots) {
                for (int i = 0; i < players.Count; i++) {
                    if (players[i] == null) {
                        players[i] = player;
                        ++NumPlayers;
                        index = i;
                        return;
                    }
                }
            }

            index = players.Count;
            players.Add(player);
            ++NumPlayers;

            foreach (var observer in OnPlayerAdded.Enumerate())
                observer.Do(index);
        }

        public void RemovePlayer(IPlayer player)
        {
            int index = players.IndexOf(player);
            if (index >= 0 && players[index] != null) {
                DebugOnly.Check(NumPlayers > 0, "Player counter has been damaged.");
                --NumPlayers;
                players[index] = null;

                foreach (var observer in OnPlayerAdded.Enumerate())
                    observer.Do(index);
            }
        }

        public IPlayer GetPlayer(int index)
        {
            if (index < 0 || index >= players.Count)
                return null;

            return players[index];
        }

        public IPlayer FindClosestPlayer(Vector3 position)
        {
            IPlayer closestPlayer = null;
            float closestDistance = 0.0f;

            foreach (var player in players) {
                if (player == null)
                    continue;

                float distance = (closestPlayer.Position - position).sqrMagnitude;
                if (closestPlayer == null || distance < closestDistance) {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }

            return closestPlayer;
        }

        public void GetPlayersSortedByDistanceNonAlloc(Vector3 position, ref List<IPlayer> outputList)
        {
            if (outputList == null)
                outputList = new List<IPlayer>(players.Count);
            else
                outputList.Clear();

            foreach (var player in players) {
                if (player != null)
                    outputList.Add(player);
            }

            outputList.Sort((a, b) => {
                    float distanceSqrA = (a.Position - position).sqrMagnitude;
                    float distanceSqrB = (b.Position - position).sqrMagnitude;
                    if (distanceSqrA < distanceSqrB)
                        return -1;
                    else if (distanceSqrA > distanceSqrB)
                        return 1;
                    else
                        return 0;
                });
        }

        public IEnumerable<IPlayer> EnumeratePlayers()
        {
            foreach (var player in players) {
                if (player != null)
                    yield return player;
            }
        }
    }
}
