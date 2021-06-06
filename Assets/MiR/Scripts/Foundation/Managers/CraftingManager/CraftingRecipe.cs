using System;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName = "OTUS/Crafting Recipe")]
    public sealed class CraftingRecipe : ScriptableObject
    {
        [Serializable]
        public sealed class Ingredient
        {
            public AbstractInventoryItem Item;
            public int Count;
            public bool Consumable = true;
        }

        public AbstractInventoryItem TargetItem;
        public int TargetCount = 1;

        public List<Ingredient> Ingredients;
    }
}
