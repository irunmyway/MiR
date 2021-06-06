using Foundation;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName="OTUS/Weapon/Melee")]
    public sealed class MeleeWeapon : AbstractWeapon
    {
        public float Damage;
        public AbstractCharacterEffect[] Effects;

        public override bool PrepareShoot(ICharacterEffectManager attackerEffectManager, IInventoryStorage inventory, IWeaponAttack attack)
        {
            if (attack is IMeleeWeaponAttack meleeAttack)
                meleeAttack.BeginMeleeAttack(Damage, attackerEffectManager, Effects);
            else
                DebugOnly.Error("Using melee weapon with wrong attack.");

            return true;
        }
    }
}
