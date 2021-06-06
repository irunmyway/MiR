using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class PlayerFollow : AbstractBehaviour, IOnLateUpdate
    {
        public int Player;
        public Vector3 Offset;

        [Inject] ISceneState state = null;
        [Inject] IPlayerManager playerManager = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnLateUpdate);
        }

        void IOnLateUpdate.Do(float timeDelta)
        {
            IPlayer player = playerManager.GetPlayer(Player);
            if (player != null)
                transform.position = player.Position + Offset;
        }
    }
}
