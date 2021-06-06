using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class CraftingManager : AbstractService<ICraftingManager>, ICraftingManager
    {
        [SerializeField] List<CraftingRecipe> allRecipes;
        public List<CraftingRecipe> AllRecipes { get { return allRecipes; } }

        public bool CanProduceItem(CraftingRecipe recipe, IInventoryStorage inventory)
        {
            foreach (var ingredient in recipe.Ingredients) {
                if (inventory.CountOf(ingredient.Item) < ingredient.Count)
                    return false;
            }

            return true;
        }

        public bool ProduceItem(CraftingRecipe recipe, IInventoryStorage inventory)
        {
            if (!CanProduceItem(recipe, inventory))
                return false;

            foreach (var ingredient in recipe.Ingredients) {
                // FIXME: здесь возможна проблема с утратой части предметов
                if (ingredient.Consumable && !inventory.Remove(ingredient.Item, ingredient.Count))
                    return false;
            }

            inventory.Add(recipe.TargetItem, recipe.TargetCount);
            return true;
        }
    }
}
