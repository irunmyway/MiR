using UnityEngine;

namespace Foundation
{
    public sealed class DisableOnStart : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(false);
        }
    }
}
