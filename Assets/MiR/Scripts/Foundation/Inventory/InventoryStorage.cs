using UnityEngine;
using System;
using System.Collections.Generic;

namespace Foundation
{
    public sealed class InventoryStorage<T> : IInventoryStorage<T>
        where T : AbstractInventoryItem
    {
        sealed class Comparer : IComparer<T>
        {
            public static readonly Comparer Instance = new Comparer();

            public int Compare(T a, T b)
            {
                if (a.LessThan(b))
                    return -1;
                if (b.LessThan(a))
                    return 1;

                int iidA = a.GetInstanceID();
                int iidB = b.GetInstanceID();
                if (iidA < iidB)
                    return -1;
                if (iidA > iidB)
                    return 1;

                return 0;
            }
        }

        readonly SortedDictionary<T, int> items = new SortedDictionary<T, int>(Comparer.Instance);

        public ObserverList<IOnInventoryChanged> OnChanged { get; } = new ObserverList<IOnInventoryChanged>();
        public int Count => items.Count;

        public IEnumerable<(T item, int count)> Items { get {
                foreach (var it in items)
                    yield return (it.Key, it.Value);
            } }

        public IEnumerable<(AbstractInventoryItem item, int count)> RawItems { get {
                foreach (var it in items)
                    yield return (it.Key, it.Value);
            } }

        public int CountOf(T item)
        {
            items.TryGetValue(item, out int count);
            return count;
        }

        int IInventoryStorage.CountOf(AbstractInventoryItem item)
        {
            if (item is T castItem)
                return CountOf(castItem);
            return 0;
        }

        public void Add(T item, int amount)
        {
            if (amount <= 0) {
                DebugOnly.Error($"Attempted to add {amount} of '{item.Title}' into the inventory.");
                return;
            }

            items.TryGetValue(item, out int count);
            items[item] = count + amount;

            foreach (var it in OnChanged.Enumerate())
                it.Do();
        }

        void IInventoryStorage.Add(AbstractInventoryItem item, int amount)
        {
            Add((T)item, amount);
        }

        public bool Remove(T item, int amount)
        {
            if (amount <= 0) {
                DebugOnly.Error($"Attempted to remove {amount} of '{item.Title}' from the inventory.");
                return false;
            }

            if (!items.TryGetValue(item, out int count) || count < amount)
                return false;

            count -= amount;
            if (count > 0)
                items[item] = count;
            else {
                if (!items.Remove(item))
                    return false;
            }

            foreach (var it in OnChanged.Enumerate())
                it.Do();

            return true;
        }

        bool IInventoryStorage.Remove(AbstractInventoryItem item, int amount)
        {
            if (item is T castItem)
                return Remove(castItem, amount);
            return false;
        }

        public void Clear()
        {
            items.Clear();

            foreach (var it in OnChanged.Enumerate())
                it.Do();
        }
    }
}
