using System;
using UnityEngine;

namespace Foundation
{
    public abstract class AbstractInventory : AbstractService<IInventory>, IInventory
    {
        public abstract IInventoryStorage RawStorage { get; }
    }

    public abstract class AbstractInventory<T> : AbstractInventory
        where T : AbstractInventoryItem
    {
        [Serializable]
        public struct InitialItem
        {
            public T Item;
            public int Count;
        }

        public readonly InventoryStorage<T> Storage = new InventoryStorage<T>();
        public override IInventoryStorage RawStorage => Storage;

        [SerializeField] InitialItem[] initialInventory;

        void Awake()
        {
            if (initialInventory == null)
                return;

            foreach (var it in initialInventory) {
                DebugOnly.Check(it.Count > 0, $"Invalid count {it.Count} in inventory of '{gameObject.name}'.");
                Storage.Add(it.Item, it.Count);
            }
        }
    }
}
