using Foundation;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName="OTUS/Weapon/Launcher")]
    public sealed class LauncherWeapon : AbstractWeapon
    {
        public AbstractInventoryItem AmmoItem;
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

            if (attack is ILauncherWeaponAttack launcherAttack)
                launcherAttack.BeginLauncherAttack(Damage);
            else
                DebugOnly.Error("Using launcher weapon with wrong attack.");

            return true;
        }
    }
}
