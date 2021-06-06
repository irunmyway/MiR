using System.Collections.Generic;

namespace Foundation
{
    public interface IInventoryStorage
    {
        ObserverList<IOnInventoryChanged> OnChanged { get; }
        IEnumerable<(AbstractInventoryItem item, int count)> RawItems { get; }
        int CountOf(AbstractInventoryItem item);
        void Add(AbstractInventoryItem item, int amount = 1);
        bool Remove(AbstractInventoryItem item, int amount = 1);
        void Clear();
    }

    public interface IInventoryStorage<T> : IInventoryStorage
        where T : AbstractInventoryItem
    {
        IEnumerable<(T item, int count)> Items { get; }
        int CountOf(T item);
        void Add(T item, int amount = 1);
        bool Remove(T item, int amount = 1);
    }
}
