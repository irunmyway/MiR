using System.Collections.Generic;

namespace Foundation
{
    public interface ICraftingManager
    {
        List<CraftingRecipe> AllRecipes { get; }
        bool CanProduceItem(CraftingRecipe recipe, IInventoryStorage inventory);
        bool ProduceItem(CraftingRecipe recipe, IInventoryStorage inventory);
    }
}
