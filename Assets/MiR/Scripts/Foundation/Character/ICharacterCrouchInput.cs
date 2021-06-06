using UnityEngine;
using Zenject;

namespace Foundation
{
    public interface ICharacterCrouchInput
    {
        bool Crouching { get; }
    }
}
