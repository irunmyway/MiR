using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterHealth : AbstractService<ICharacterHealth>, ICharacterHealth
    {
        [SerializeField] float health;
        public float Health => health;

        [SerializeField] float maxHealth;
        public float MaxHealth => maxHealth;

        [SerializeField] [ReadOnly] bool isDead;
        public bool IsDead => isDead;

        public ObserverList<IOnCharacterDamaged> OnDamaged { get; } = new ObserverList<IOnCharacterDamaged>();
        public ObserverList<IOnCharacterDied> OnDied { get; } = new ObserverList<IOnCharacterDied>();
        public ObserverList<IOnCharacterHealed> OnHealed { get; } = new ObserverList<IOnCharacterHealed>();

        public void Damage(IAttacker attacker, float damage)
        {
            DebugOnly.Check(damage >= 0.0f, "Damage is negative.");
            health -= damage;

            bool died = false;
            if (health <= 0.0f) {
                health = 0.0f;
                died = true;
            }

            foreach (var it in OnDamaged.Enumerate())
                it.Do(this, attacker, damage, health);

            if (died && !isDead) {
                isDead = true;
                foreach (var it in OnDied.Enumerate())
                    it.Do(this, attacker);
            }
        }

        public void Heal(IAttacker attacker, float heal)
        {
            DebugOnly.Check(heal >= 0.0f, "Heal is negative.");

            health += heal;
            if (health > MaxHealth)
                health = MaxHealth;

            foreach (var it in OnHealed.Enumerate())
                it.Do(this, attacker, heal, health);
        }
    }
}
