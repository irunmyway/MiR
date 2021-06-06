using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class TrafficLights : AbstractBehaviour
    {
        public GameObject RedLight;
        public GameObject GreenLight;

        [ReadOnly] bool isGreen;
        public bool IsGreen => isGreen;

        public void SetGreen(bool flag)
        {
            isGreen = flag;
            RedLight.SetActive(!isGreen);
            GreenLight.SetActive(isGreen);
        }
    }
}
