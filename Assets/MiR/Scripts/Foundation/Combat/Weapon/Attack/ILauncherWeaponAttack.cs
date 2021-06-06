using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface ILauncherWeaponAttack : IWeaponAttack
    {
        void BeginLauncherAttack(float damage);
    }
}
