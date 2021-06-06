using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterItemPickup : AbstractBehaviour
    {
        [Inject] IInventory inventory = default;

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent<ICollectible>(out var collectible))
                collectible.Collect(inventory.RawStorage);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ICollectible>(out var collectible))
                collectible.Collect(inventory.RawStorage);
        }
    }
}
