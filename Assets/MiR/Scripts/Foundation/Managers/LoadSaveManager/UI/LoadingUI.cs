using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

namespace Foundation
{
    public sealed class LoadingUI : AbstractBehaviour, IOnStateActivate, IOnStateDeactivate, IOnLoadSaveSlotsChanged
    {
        public Transform Content;

        readonly List<LoadingRow> rows = new List<LoadingRow>();
        ObserverHandle loadSaveSlotsChangeObserver;

        [Inject] ISceneState state = default;
        [Inject] ILoadSaveManager loadSaveManager = default;
        [Inject] LoadingRow.Factory rowFactory = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnActivate);
            Observe(state.OnDeactivate);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Clear();
        }

        void IOnStateActivate.Do()
        {
            Refresh();
            Observe(ref loadSaveSlotsChangeObserver, loadSaveManager.OnLoadSaveSlotsChanged);
        }

        void IOnStateDeactivate.Do()
        {
            Unobserve(loadSaveSlotsChangeObserver);
            Clear();
        }

        void IOnLoadSaveSlotsChanged.Do()
        {
            Refresh();
        }

        void Refresh()
        {
            Clear();

            foreach (var slot in loadSaveManager.GetSlots()) {
                var row = rowFactory.Create(slot);
                row.transform.SetParent(Content, false);
                rows.Add(row);
            }
        }

        void Clear()
        {
            foreach (var row in rows)
                row.Pool.Despawn(row);
            rows.Clear();
        }
    }
}
