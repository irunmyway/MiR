using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    [RequireComponent(typeof(Light))]
    public sealed class NightLight : MonoBehaviour
    {
        public Light Light { get; private set; }

        void Awake()
        {
            Light = GetComponent<Light>();
        }
    }
}
