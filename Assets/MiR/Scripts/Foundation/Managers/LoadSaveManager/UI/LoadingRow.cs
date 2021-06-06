using System;
using UnityEngine;
using TMPro;
using Zenject;

namespace Foundation
{
    public sealed class LoadingRow : AbstractBehaviour, IPoolable<SaveSlot, IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<SaveSlot, LoadingRow>
        {
        }

        [Inject] ILoadSaveManager loadSaveManager = default;
        [SerializeField] TextMeshProUGUI nameLabel;
        SaveSlot saveSlot;

        public IMemoryPool Pool { get; private set; }

        public void OnSpawned(SaveSlot slot, IMemoryPool pool)
        {
            Pool = pool;
            nameLabel.text = slot.Name;
            saveSlot = slot;
        }

        public void OnDespawned()
        {
        }

        public void Load()
        {
            loadSaveManager.LoadAsync(saveSlot);
        }

        public void Save()
        {
            // loadSaveManager.SaveOverwrite(saveSlot);
            loadSaveManager.SaveOverwrite(saveSlot, $"Save {SaveGameButton.counter++}"); // FIXME
        }

        public void Delete()
        {
            loadSaveManager.Delete(saveSlot);
        }
    }
}
