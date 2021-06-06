using UnityEngine;

namespace Foundation
{
    public sealed class ReparentOnAwake : MonoBehaviour
    {
        public Transform NewParent;
        public bool KeepTransform;

        void Awake()
        {
            transform.SetParent(NewParent, KeepTransform);
        }
    }
}
