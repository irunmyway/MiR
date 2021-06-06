using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CalmDownAfterDelayBehaviour : EnemyBehaviour
    {
        public float MinDelay = 2.0f;
        public float MaxDelay = 3.0f;

        [Inject] IEnemy enemy = default;
        float timer = 0.0f;

        public override bool CheckUpdateAI(float deltaTime)
        {
            if (timer > 0.0f) {
                timer -= deltaTime;
                if (timer <= 0.0f)
                    enemy.LeaveAlertState();
                return false;
            }

            if (enemy.IsAlert)
                timer = Random.Range(MinDelay, MaxDelay);

            return false;
        }
    }
}
