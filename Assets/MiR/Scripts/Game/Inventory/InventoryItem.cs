using Foundation;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "OTUS/Inventory Item")]
    public sealed class InventoryItem : AbstractInventoryItem
    {
        public AbstractWeapon Weapon;
    }
}
