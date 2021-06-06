using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Button))]
    public sealed class CraftButton : MonoBehaviour
    {
        [Inject] ICraftingManager craftingManager = default;
        [Inject] INotificationManager notificationManager = default;
        [Inject] ILocalizationManager localizationManager = default;

        public LocalizedString SuccessMessage;
        public LocalizedString FailureMessage;

        public AbstractInventory Inventory;
        public CraftingRecipe Recipe;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => {
                    if (craftingManager.ProduceItem(Recipe, Inventory.RawStorage)) {
                        notificationManager.DisplayMessage(
                            string.Format(localizationManager.GetString(SuccessMessage), Recipe.TargetItem.name));
                    } else {
                        notificationManager.DisplayMessage(
                            string.Format(localizationManager.GetString(FailureMessage), Recipe.TargetItem.name));
                    }
                });
        }
    }
}
