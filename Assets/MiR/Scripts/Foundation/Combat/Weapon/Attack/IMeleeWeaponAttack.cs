using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface IMeleeWeaponAttack : IWeaponAttack
    {
        void BeginMeleeAttack(float damage, ICharacterEffectManager attackerEffectManager, AbstractCharacterEffect[] effects);
    }
}
