using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class TakeCoverBehaviour : EnemyBehaviour
    {
        enum State
        {
            Initial,
            RunningToCover,
            CooldownAfterRun,
            Idle,
        }

        public float CooldownAfterRun = 2.0f;
        public override bool Crouching => state == State.CooldownAfterRun || state == State.Idle;

        [Inject] IEnemy enemy = default;
        [Inject] IEnemyManager enemyManager = default;
        [Inject] ICharacterAgent agent = default;
        CoverPoint coverPoint;
        State state = State.Initial;
        float cooldownTimer;

        public override bool CheckUpdateAI(float deltaTime)
        {
            return enabled && (enemy.IsAlert || state == State.RunningToCover || state == State.CooldownAfterRun);
        }

        public override void ActivateAI()
        {
            FindNewCoverPoint();
        }

        void FindNewCoverPoint()
        {
            coverPoint = enemyManager.AllocCoverPoint(enemy);
            if (coverPoint != null) {
                state = State.RunningToCover;
                var pos = coverPoint.transform.position;
                agent.NavigateTo(new Vector2(pos.x, pos.z));
            }
        }

        void ReleaseCurrentCoverPoint()
        {
            if (coverPoint != null) {
                enemyManager.ReleaseCoverPoint(enemy, coverPoint);
                coverPoint = null;
            }
            state = State.Initial;
            agent.Stop();
        }

        public override void UpdateAI(float deltaTime)
        {
            if (coverPoint == null ||
                (enemy.SeenPlayer != null
                    && Vector3.Distance(enemy.SeenPlayer.Position, coverPoint.transform.position) <= enemyManager.DangerousPlayerDistance)) {

                ReleaseCurrentCoverPoint();
                FindNewCoverPoint();
            }

            switch (state) {
                case State.RunningToCover: {
                    float distanceToCoverPoint = Vector3.Distance(enemy.Position, coverPoint.transform.position);
                    if (distanceToCoverPoint < 0.1f) {
                        state = State.CooldownAfterRun;
                        cooldownTimer = CooldownAfterRun;
                    }
                    break;
                }

                case State.CooldownAfterRun: {
                    cooldownTimer -= deltaTime;
                    if (cooldownTimer <= 0.0f)
                        state = State.Idle;
                    break;
                }
            }
        }

        public override void DeactivateAI()
        {
            ReleaseCurrentCoverPoint();
        }
    }
}
