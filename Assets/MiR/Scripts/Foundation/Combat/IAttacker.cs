using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface IAttacker
    {
        IPlayer Player { get; }
        IEnemy Enemy { get; }
        AbstractCharacterEffect Effect { get; }
    }
}
