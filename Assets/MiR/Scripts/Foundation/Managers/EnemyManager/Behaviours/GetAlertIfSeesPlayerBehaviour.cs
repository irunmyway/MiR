using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class GetAlertIfSeesPlayerBehaviour : EnemyBehaviour
    {
        public bool AlertAllEnemies;

        [Inject] IEnemy enemy = default;
        [Inject] IEnemyManager enemyManager = default;

        public override bool CheckUpdateAI(float deltaTime)
        {
            if (enemy.SeenPlayer != null) {
                enemy.EnterAlertState();
                if (AlertAllEnemies)
                    enemyManager.AlertAllEnemies();
            }

            return false;
        }
    }
}
