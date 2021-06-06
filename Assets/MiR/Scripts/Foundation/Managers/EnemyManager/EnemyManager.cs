using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class EnemyManager : AbstractService<IEnemyManager>, IEnemyManager, IOnEnemyDidAttackPlayer
    {
        public int SimultaneouslyAttackingEnemies = 1;

        [SerializeField] float dangerousPlayerDistance = 2.0f;
        public float DangerousPlayerDistance => dangerousPlayerDistance;

        public ICollection<IEnemy> AllEnemies => enemies;
        List<IEnemy> enemies = new List<IEnemy>();
        Dictionary<IEnemy, ObserverHandle> enemyHandles = new Dictionary<IEnemy, ObserverHandle>();

        public ICollection<CoverPoint> AllCoverPoints => coverPoints;
        [SerializeField] List<CoverPoint> coverPoints = new List<CoverPoint>();

        HashSet<IEnemy> enemyCanAttack = new HashSet<IEnemy>();
        List<IEnemy> attackCandidatesCache = new List<IEnemy>();

        [Inject] IPlayerManager playerManager = null;

        public void AddEnemy(IEnemy enemy)
        {
            if (enemyCanAttack.Count < SimultaneouslyAttackingEnemies)
                enemyCanAttack.Add(enemy);

            var handle = new ObserverHandle();
            Observe(ref handle, enemy.OnDidAttackPlayer);
            enemyHandles[enemy] = handle;

            enemies.Add(enemy);
        }

        public void RemoveEnemy(IEnemy enemy)
        {
            if (enemyHandles.TryGetValue(enemy, out var handle)) {
                Unobserve(handle);
                enemyHandles.Remove(enemy);
            }

            enemies.Remove(enemy);

            if (enemyCanAttack.Remove(enemy))
                AllowEnemyAttack();
        }

        public void AlertAllEnemies()
        {
            foreach (var enemy in enemies)
                enemy.EnterAlertState();
        }

        public bool EnemyCanAttack(IEnemy enemy)
        {
            return enemyCanAttack.Contains(enemy);
        }

        void AllowEnemyAttack()
        {
            attackCandidatesCache.Clear();
            foreach (var enemy in enemies) {
                if (!enemyCanAttack.Contains(enemy))
                    attackCandidatesCache.Add(enemy);
            }

            if (attackCandidatesCache.Count == 0)
                return;

            int enemyIndex = Random.Range(0, attackCandidatesCache.Count);
            enemyCanAttack.Add(attackCandidatesCache[enemyIndex]);
        }

        void IOnEnemyDidAttackPlayer.Do(IEnemy enemy)
        {
            if (enemyCanAttack.Remove(enemy))
                AllowEnemyAttack();
        }

        bool CanSeePlayer(Vector3 origin, out float distance)
        {
            foreach (var player in playerManager.EnumeratePlayers()) {
                var direction = player.Position - origin;
                distance = direction.magnitude;
                direction /= distance;

                if (Physics.Raycast(origin, direction, out var hitInfo, distance)) {
                    var context = hitInfo.transform.GetComponentInParent<Context>();
                    if (context != null) {
                        var hitPlayer = context.Container.TryResolve<IPlayer>();
                        if (hitPlayer != null)
                            return true;
                    }
                }
            }

            distance = 0.0f;
            return false;
        }

        public CoverPoint AllocCoverPoint(IEnemy enemy)
        {
            CoverPoint nearestCoverPoint = null;
            float nearestDistance = 0.0f;

            foreach (var obj in coverPoints) {
                if (obj.Enemy != null)
                    continue;

                Vector3 origin = obj.transform.position;
                origin.y += 0.5f; // FIXME: hardcoded
                if (CanSeePlayer(origin, out float distance) && distance < DangerousPlayerDistance)
                    continue;

                float sqrDistance = (obj.transform.position - enemy.Position).sqrMagnitude;
                if (nearestCoverPoint == null || sqrDistance < nearestDistance) {
                    nearestCoverPoint = obj;
                    nearestDistance = sqrDistance;
                }
            }

            if (nearestCoverPoint != null)
                nearestCoverPoint.Enemy = enemy;

            return nearestCoverPoint;
        }

        public void ReleaseCoverPoint(IEnemy enemy, CoverPoint coverPoint)
        {
            DebugOnly.Check(coverPoint.Enemy == enemy, "attempted to release cover point not owned by this enemy.");
            coverPoint.Enemy = null;
        }
    }
}
