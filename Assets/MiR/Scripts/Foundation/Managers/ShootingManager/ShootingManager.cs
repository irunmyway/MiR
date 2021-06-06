using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public class ShootingManager : AbstractService<IShootingManager>, IShootingManager
    {
        sealed class DistanceComparer : IComparer<RaycastHit>
        {
            public static readonly DistanceComparer Instance = new DistanceComparer();

            public Vector3 Dst;

            public int Compare(RaycastHit a, RaycastHit b)
            {
                float distA = (Dst - a.point).sqrMagnitude; // FIXME: use hit.distance?
                float distB = (Dst - b.point).sqrMagnitude;

                if (distA < distB)
                    return -1;
                else if (distA > distB)
                    return 1;
                else
                    return 0;
            }
        }

        [Inject] ShootTrace.Factory shootTraceFactory = default;
        RaycastHit[] result = new RaycastHit[1024];

        Vector3 AdjustShoot(Vector3 from, Vector3 forward, Vector3 right, float minAngle, float maxAngle)
        {
            float targetAngle = UnityEngine.Random.Range(minAngle, maxAngle);
            Vector3 dir = Quaternion.AngleAxis(targetAngle, right) * forward;
            dir = Quaternion.AngleAxis(UnityEngine.Random.Range(0.0f, 360.0f), forward) * dir;
            return dir;
        }

        void DoDamage(RaycastHit hitInfo, IAttacker attacker, float damage)
        {
            var context = hitInfo.collider.GetComponentInParent<Context>();
            if (context != null) {
                var health = context.Container.TryResolve<ICharacterHealth>();
                if (health != null)
                    health.Damage(attacker, damage);
            }
        }

        public void Shoot(Transform source, IAttacker attacker, RangedWeaponParameters parameters, int layerMask, float damage)
        {
            Vector3 from = source.position;
            Vector3 forward = AdjustShoot(from, source.forward, source.right, parameters.MinTargetAngle, parameters.MaxTargetAngle);
            Vector3 right = Vector3.Cross(forward, source.up);

            for (int i = 0; i < parameters.Count; i++) {
                var dir = AdjustShoot(from, forward, right, parameters.MinGroupingAngle, parameters.MaxGroupingAngle);
                Vector3 dst;

                Ray ray = new Ray(from, dir);
                if (!Physics.Raycast(ray, out var hitInfo, parameters.MaxDistance, layerMask))
                    dst = from + ray.direction * parameters.MaxDistance;
                else {
                    dst = hitInfo.point;

                    DoDamage(hitInfo, attacker, damage);

                    var materialType = hitInfo.collider.GetComponentInParent<TargetMaterial>();
                    if (materialType != null) {
                        switch (materialType.TargetMaterialType) {
                            case TargetMaterial.MaterialType.Blocking:
                                break;

                            case TargetMaterial.MaterialType.Ricochet: {
                                float passedDistance = (dst - from).magnitude;
                                float distance = (parameters.MaxDistance - passedDistance) * 0.5f;

                                Vector3 ricochetDir = Vector3.Reflect(ray.direction, hitInfo.normal);
                                Vector3 ricochetDst;
                                Ray ricochetRay = new Ray(dst, ricochetDir);
                                if (!Physics.Raycast(ricochetRay, out var ricochetHitInfo, distance, layerMask))
                                    ricochetDst = from + ricochetDir * distance;
                                else {
                                    ricochetDst = ricochetHitInfo.point;
                                    DoDamage(ricochetHitInfo, attacker, damage * 0.5f);
                                }

                                shootTraceFactory.Create(dst, ricochetDst);
                                break;
                            }

                            case TargetMaterial.MaterialType.PassThrough: {
                                float passedDistance = (dst - from).magnitude;
                                float distance = (parameters.MaxDistance - passedDistance) * 0.5f;

                                Vector3 passDst = Vector3.zero;
                                Ray passRay = new Ray(dst, ray.direction);
                                int n = Physics.RaycastNonAlloc(passRay, result, distance, layerMask);

                                DistanceComparer.Instance.Dst = dst;
                                Array.Sort(result, 0, n, DistanceComparer.Instance);

                                bool found = false;
                                for (int j = 0; j < n; j++) {
                                    var it = result[j];
                                    if (it.collider != hitInfo.collider) {
                                        found = true;
                                        passDst = it.point;
                                        DoDamage(it, attacker, damage * 0.5f);
                                    }
                                }

                                if (!found)
                                    passDst = dst + passRay.direction * distance;

                                shootTraceFactory.Create(dst, passDst);
                                break;
                            }
                        }
                    }
                }

                shootTraceFactory.Create(from, dst);
            }
        }
    }
}
