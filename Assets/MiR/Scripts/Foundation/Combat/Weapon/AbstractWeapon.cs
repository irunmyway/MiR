using UnityEngine;

namespace Foundation
{
    public abstract class AbstractWeapon : ScriptableObject
    {
        public AbstractInventoryItem InventoryItem;
        public float AttackCooldownTime;

        public virtual bool CanShoot(IInventoryStorage inventory)
        {
            return true;
        }

        public virtual bool PrepareShoot(ICharacterEffectManager attackerEffectManager, IInventoryStorage inventory, IWeaponAttack attack)
        {
            return true;
        }

        public virtual void EndShoot(IWeaponAttack attack)
        {
            attack.EndAttack();
        }

        public virtual void EndCooldown(IWeaponAttack attack)
        {
            attack.EndCooldown();
        }
    }
}
