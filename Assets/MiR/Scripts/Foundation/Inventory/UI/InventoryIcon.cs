using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Image))]
    public sealed class InventoryIcon : AbstractBehaviour, IPoolable<AbstractInventoryItem, int, IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<AbstractInventoryItem, int, InventoryIcon>
        {
        }

        Image image;
        Transform originalParent;

        public IMemoryPool Pool { get; private set; }

        public GameObject Overlay;
        public TextMeshProUGUI Text;

        void Awake()
        {
            image = GetComponent<Image>();
            originalParent = transform.parent;
        }

        public void OnSpawned(AbstractInventoryItem item, int count, IMemoryPool pool)
        {
            Pool = pool;
            image.sprite = item.Icon;

            if (count == 1)
                Overlay.SetActive(false);
            else {
                Text.text = count.ToString();
                Overlay.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
        }
    }
}
