using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class ObserverHandleManager
    {
        List<ObserverHandle> handles = new List<ObserverHandle>();

        public ObserverHandle Alloc()
        {
            var handle = new ObserverHandle();
            Add(handle);
            return handle;
        }

        public void Add(ObserverHandle handle)
        {
            handles.Add(handle);
        }

        public ObserverHandle Observe<T>(IObserverList<T> observable, T observer)
        {
            ObserverHandle handle = null;
            Observe(ref handle, observable, observer);
            return handle;
        }

        public void Observe<T>(ref ObserverHandle handle, IObserverList<T> observable, T observer)
        {
            observable.Add(ref handle, observer);
            Add(handle);
        }

        public void Unobserve(ObserverHandle handle)
        {
            if (handle != null) {
                handle.Dispose();
                handles.Remove(handle);
            }
        }

        public void Clear()
        {
            foreach (var handle in handles)
                handle.Dispose();
            handles.Clear();
        }
    }
}
