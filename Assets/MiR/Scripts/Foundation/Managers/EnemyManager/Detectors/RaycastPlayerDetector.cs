using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class RaycastPlayerDetector : AbstractService<IPlayerDetector>, IPlayerDetector
    {
        public float DetectionDistance;

        [Inject] IPlayerManager playerManager = default;
        List<IPlayer> cachedPlayerList;

        public IPlayer FindTargetPlayer()
        {
            Vector3 origin = transform.position;
            playerManager.GetPlayersSortedByDistanceNonAlloc(origin, ref cachedPlayerList);

            foreach (var player in cachedPlayerList) {
                Vector3 target = player.Position;
                target.y = origin.y;

                Vector3 direction = target - origin;

                float distanceToPlayer = direction.magnitude;
                if (distanceToPlayer > DetectionDistance)
                    continue;

                direction /= distanceToPlayer;
                var (hits, hitCount) = PhysicsUtility.CastRay(origin, direction, DetectionDistance);

                float playerDistance = 0.0f;
                float closestHit = float.PositiveInfinity;
                bool hitPlayer = false;
                for (int i = 0; i < hitCount; i++) {
                    float distance = hits[i].distance;
                    if (distance < closestHit) {
                        closestHit = distance;
                        if (hitPlayer && closestHit < playerDistance) {
                            hitPlayer = false;
                            break;
                        }
                    }

                    var context = hits[i].transform.GetComponentInParent<Context>();
                    if (context != null) {
                        var playerInstance = context.Container.TryResolve<IPlayer>();
                        if (playerInstance == player) {
                            if (distance > closestHit) {
                                hitPlayer = false;
                                break;
                            }
                            playerDistance = distance;
                            hitPlayer = true;
                        }
                    }
                }

                if (hitPlayer)
                    return player;
            }

            return null;
        }
    }
}
