using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface IRangedWeaponAttack : IWeaponAttack
    {
        void BeginRangedAttack(RangedWeaponParameters parameters, float damage);
    }
}
