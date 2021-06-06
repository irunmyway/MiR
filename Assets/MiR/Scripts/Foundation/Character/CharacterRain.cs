using UnityEngine;

namespace Foundation
{
    public sealed class CharacterRain : AbstractService<ICharacterRain>, ICharacterRain
    {
        public bool Enabled { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }
    }
}
