using Foundation;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName="OTUS/Weapon/Ranged")]
    public class RangedWeapon : AbstractWeapon
    {
        public AbstractInventoryItem AmmoItem;
        public RangedWeaponParameters Parameters;
        public float Damage;

        public override bool CanShoot(IInventoryStorage inventory)
        {
            if (AmmoItem == null)
                return true;

            if (inventory == null)
                return false;

            return inventory.CountOf(AmmoItem) > 0;
        }

        public override bool PrepareShoot(ICharacterEffectManager attackerEffectManager, IInventoryStorage inventory, IWeaponAttack attack)
        {
            if (AmmoItem != null && (inventory == null || !inventory.Remove(AmmoItem, 1)))
                return false;

            if (attack is IRangedWeaponAttack rangedAttack)
                rangedAttack.BeginRangedAttack(Parameters, Damage);
            else
                DebugOnly.Error("Using ranged weapon with wrong attack.");

            return true;
        }
    }
}
