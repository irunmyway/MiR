using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface IWeaponAttack
    {
        void EndAttack();
        void EndCooldown();
    }
}
