using UnityEngine;
using Zenject;

namespace Foundation
{
    public interface ICharacterJumpInput
    {
        ObserverList<IOnCharacterJump> OnCharacterJump { get; }
    }
}
