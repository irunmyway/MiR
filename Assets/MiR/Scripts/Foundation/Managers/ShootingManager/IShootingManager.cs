using UnityEngine;

namespace Foundation
{
    public interface IShootingManager
    {
        void Shoot(Transform source, IAttacker attacker, RangedWeaponParameters parameters, int layerMask, float damage);
    }
}
