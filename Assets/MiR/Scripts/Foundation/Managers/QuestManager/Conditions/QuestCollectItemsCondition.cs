using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName = "OTUS/Quest Conditions/Collect Item")]
    public sealed class QuestCollectItemsCondition : QuestCondition
    {
        public AbstractInventoryItem Item;
        public int Count;

        public override bool IsTrue(QuestManager questManager)
        {
            var player = questManager.playerManager.GetPlayer(0);
            return (player.Inventory != null && player.Inventory.RawStorage.CountOf(Item) >= Count);
        }
    }
}
