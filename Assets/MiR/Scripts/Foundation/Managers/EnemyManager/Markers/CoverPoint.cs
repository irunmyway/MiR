using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class CoverPoint : NavigationPoint
    {
        public IEnemy Enemy { get; internal set; }
    }
}
