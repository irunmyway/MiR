using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class AttackIfSeesPlayerBehaviour : EnemyBehaviour
    {
        public float Cooldown = 1.0f;
        public float AimingTime = 0.3f;

        [Inject] IEnemy enemy = default;
        float cooldownLeft;

        float aimingTimeLeft;

        public override bool CheckUpdateAI(float deltaTime)
        {
            if (cooldownLeft > 0.0f) {
                cooldownLeft -= deltaTime;
                return false;
            }

            return enabled && enemy.SeenPlayer != null && enemy.CanAttackPlayer(enemy.SeenPlayer);
        }

        public override void UpdateAI(float deltaTime)
        {
            if (enemy.SeenPlayer != null) {
                if (!enemy.CanAttackPlayer(enemy.SeenPlayer))
                    return;

                var dir = (enemy.SeenPlayer.Position - transform.position).normalized;
                if (enemy.Agent != null)
                    enemy.Agent.Look(new Vector2(dir.x, dir.z));

                if (aimingTimeLeft < 0.0f)
                    aimingTimeLeft = AimingTime;
                else {
                    aimingTimeLeft -= deltaTime;
                    if (aimingTimeLeft <= 0.0f) {
                        if (enemy.TryAttackPlayer(enemy.SeenPlayer))
                            cooldownLeft = Cooldown;
                    }
                }
            }
        }
    }
}
