using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class MeleeWeaponAttack : AbstractWeaponAttack, IAttacker, IMeleeWeaponAttack
    {
        [InjectOptional] IPlayer player = default;
        public IPlayer Player => player;
        [InjectOptional] IEnemy enemy = default;
        public IEnemy Enemy => enemy;
        public AbstractCharacterEffect Effect => null;

        HashSet<ICharacterHealth> damaged = new HashSet<ICharacterHealth>();
        HashSet<ICharacterHealth> affected = new HashSet<ICharacterHealth>();
        AbstractCharacterEffect[] effects;
        ICharacterEffectManager attackerEffectManager;
        bool inAttack;
        float damage;

        public void BeginMeleeAttack(float damage, ICharacterEffectManager attackerEffectManager, AbstractCharacterEffect[] effects)
        {
            DebugOnly.Check(!inAttack, "BeginAttack called twice.");
            inAttack = true;
            this.attackerEffectManager = attackerEffectManager;
            this.damage = damage;
            this.effects = effects;
        }

        public override void EndAttack()
        {
            DebugOnly.Check(inAttack, "EndAttack called without attack.");
            inAttack = false;
            damaged.Clear();
            affected.Clear();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!inAttack)
                return;

            var context = other.GetComponentInParent<Context>();
            if (context != null) {
                var health = context.Container.TryResolve<ICharacterHealth>();
                if (health != null && damaged.Add(health))
                    health.Damage(this, damage);

                if (effects != null && effects.Length > 0) {
                    var victimEffectManager = context.Container.TryResolve<ICharacterEffectManager>();
                    if (victimEffectManager != null && affected.Add(health)) {
                        foreach (var effect in effects)
                            victimEffectManager.AddEffect(this, effect);
                    }
                }
            }
        }
    }
}
