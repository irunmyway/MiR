using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class InventoryRow : AbstractBehaviour, IPoolable<IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<InventoryRow>
        {
        }

        [Inject] InventoryIcon.Factory iconFactory = default;
        Transform originalParent;
        readonly List<InventoryIcon> icons = new List<InventoryIcon>();

        public int Count => icons.Count;
        public IMemoryPool Pool { get; private set; }

        void Awake()
        {
            originalParent = transform.parent;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            Pool = pool;
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
            Clear();
        }

        public void AddItem(AbstractInventoryItem item, int count)
        {
            var icon = iconFactory.Create(item, count);
            icon.transform.SetParent(transform, false);
            icons.Add(icon);
        }

        public void Clear()
        {
            foreach (var icon in icons)
                icon.Pool.Despawn(icon);
            icons.Clear();
        }
    }
}
