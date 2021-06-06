using System;

namespace Foundation
{
    public sealed class ObserverHandle : IDisposable
    {
        internal int Index;
        internal IObserverList List;

        public void Dispose()
        {
            if (List != null) {
                List.Remove(this);
                DebugOnly.Check(List == null, "Handle was not properly removed from the list.");
            }
        }
    }
}
