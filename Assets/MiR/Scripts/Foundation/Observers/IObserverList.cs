using System.Collections;
using System.Collections.Generic;

namespace Foundation
{
    public interface IObserverList
    {
        int ObserverCount { get; }
        void Add(ref ObserverHandle handle, object observer);
        void Remove(ObserverHandle handle);
        IEnumerable Enumerate();
    }

    public interface IObserverList<T> : IObserverList
    {
        void Add(ref ObserverHandle handle, T observer);
        new IEnumerable<T> Enumerate();
    }
}
