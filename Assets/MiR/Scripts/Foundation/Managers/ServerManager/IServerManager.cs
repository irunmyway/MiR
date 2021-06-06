using UnityEngine;

namespace Foundation
{
    public interface IServerManager
    {
        ObserverList<IOnServerError> OnServerError { get; }
    }
}
