using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class ReturnWhenNotAlertBehaviour : EnemyBehaviour
    {
        [Inject] IEnemy enemy = default;
        [Inject] ICharacterAgent agent = default;
        Vector3 startingPoint;

        void Start()
        {
            startingPoint = enemy.Position;
        }

        public override bool CheckUpdateAI(float deltaTime)
        {
            return enabled && !enemy.IsAlert;
        }

        public override void UpdateAI(float deltaTime)
        {
            if (!enemy.IsAlert)
                agent.NavigateTo(new Vector2(startingPoint.x, startingPoint.z));
        }

        public override void DeactivateAI()
        {
            agent.Stop();
        }
    }
}
