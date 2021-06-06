using UnityEngine;

namespace Foundation
{
    public sealed class CharacterRigidbody : AbstractService<ICharacterRigidbody>, ICharacterRigidbody
    {
        Rigidbody rigidBody;

        public bool Enabled { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }
    }
}
