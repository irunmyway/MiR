using System.Collections;
using System.Collections.Generic;

namespace Foundation
{
    public sealed class ObserverList<T> : IObserverList<T>
    {
        struct Item
        {
            public ObserverHandle handle;
            public T observer;
        }

        public int ObserverCount => list.Count;

        List<Item> list = new List<Item>();
        List<Item> cachedList = new List<Item>();

        void IObserverList.Add(ref ObserverHandle handle, object observer)
        {
            if (observer is T typedObserver)
                Add(ref handle, typedObserver);
            else {
                if (observer == null)
                    DebugOnly.Error("Observer is null.");
                else
                    DebugOnly.Error($"Was expecting type {typeof(T)}, got type {observer.GetType().Name}.");
            }
        }

        public void Add(ref ObserverHandle handle, T observer)
        {
            if (handle == null)
                handle = new ObserverHandle();

            DebugOnly.Check(observer != null, "Observer is null.");
            DebugOnly.Check(handle.List == null, "Handle is already in use.");

            handle.Index = list.Count;
            handle.List = this;
            list.Add(new Item{ handle = handle, observer = observer });
        }

        public void Remove(ObserverHandle handle)
        {
            DebugOnly.Check(handle != null, "Handle is null.");
            DebugOnly.Check(handle.List == this, "Handle is not registered with this list.");

            int lastIndex = list.Count - 1;
            if (handle.Index != lastIndex) {
                var replacement = list[lastIndex];
                list[handle.Index] = replacement;
                replacement.handle.Index = handle.Index;
            }

            handle.List = null;
            list.RemoveAt(lastIndex);
        }

        IEnumerable IObserverList.Enumerate()
        {
            return Enumerate();
        }

        public IEnumerable<T> Enumerate()
        {
            cachedList.Clear();
            cachedList.AddRange(list);
            foreach (var item in cachedList)
                yield return item.observer;
        }
    }
}
