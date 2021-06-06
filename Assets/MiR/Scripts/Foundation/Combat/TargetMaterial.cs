using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class TargetMaterial : MonoBehaviour
    {//
        public enum MaterialType
        {
            Blocking,
            Ricochet,
            PassThrough,
        }

        public MaterialType TargetMaterialType;
    }
}
