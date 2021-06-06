using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface IPlayerManager
    {
        int NumPlayers { get; }

        ObserverList<IOnPlayerAdded> OnPlayerAdded { get; }
        ObserverList<IOnPlayerRemoved> OnPlayerRemoved { get; }

        ObserverList<IOnPlayerDamaged> OnPlayerDamaged { get; }
        ObserverList<IOnPlayerDied> OnPlayerDied { get; }
        ObserverList<IOnPlayerHealed> OnPlayerHealed { get; }

        void AddPlayer(IPlayer player, out int index, bool reuseSlots = true);
        void RemovePlayer(IPlayer player);

        IPlayer GetPlayer(int index);

        IPlayer FindClosestPlayer(Vector3 position);
        void GetPlayersSortedByDistanceNonAlloc(Vector3 position, ref List<IPlayer> outputList);

        IEnumerable<IPlayer> EnumeratePlayers();
    }
}
