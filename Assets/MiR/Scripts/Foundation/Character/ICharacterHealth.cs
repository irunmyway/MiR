using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface ICharacterHealth
    {
        float Health { get; }
        float MaxHealth { get; }
        bool IsDead { get; }

        ObserverList<IOnCharacterDamaged> OnDamaged { get; }
        ObserverList<IOnCharacterDied> OnDied { get; }
        ObserverList<IOnCharacterHealed> OnHealed { get; }

        void Damage(IAttacker attacker, float damage);
        void Heal(IAttacker attacker, float heal);
    }
}
