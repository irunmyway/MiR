using UnityEngine;

namespace Foundation
{
    public interface ICamera
    {
        GameObject GameObject { get; }
        int PlayerIndex { get; }
    }
}
