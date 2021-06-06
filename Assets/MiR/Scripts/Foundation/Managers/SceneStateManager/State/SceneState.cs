using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Zenject;

namespace Foundation
{
    public sealed class SceneState : AbstractService<ISceneState>, ISceneState, ISceneStateInternal
    {
        [Header("Debug")]
        [ReadOnly] [SerializeField] bool isTopmost;
        [ReadOnly] [SerializeField] bool isVisible;
        [ReadOnly] [SerializeField] int sortingOrder;
        public bool IsTopmost => isTopmost;
        public bool IsVisible => isVisible;
        public int SortingOrder => sortingOrder;

        [Header("Update Settings")]
        [SerializeField] bool updateParentState = false;
        public bool UpdateParentState => updateParentState;

        public ObserverList<IOnUpdate> OnUpdate { get; } = new ObserverList<IOnUpdate>();
        public ObserverList<IOnUpdateDuringPause> OnUpdateDuringPause { get; } = new ObserverList<IOnUpdateDuringPause>();
        public ObserverList<IOnFixedUpdate> OnFixedUpdate { get; } = new ObserverList<IOnFixedUpdate>();
        public ObserverList<IOnLateUpdate> OnLateUpdate { get; } = new ObserverList<IOnLateUpdate>();

        public ObserverList<IOnStateActivate> OnActivate { get; } = new ObserverList<IOnStateActivate>();
        public ObserverList<IOnStateDeactivate> OnDeactivate { get; } = new ObserverList<IOnStateDeactivate>();

        public ObserverList<IOnStateBecomeTopmost> OnBecomeTopmost { get; } = new ObserverList<IOnStateBecomeTopmost>();
        public ObserverList<IOnStateResignTopmost> OnResignTopmost { get; } = new ObserverList<IOnStateResignTopmost>();

        public ObserverList<IOnStateSortingOrderChanged> OnSortingOrderChanged { get; } = new ObserverList<IOnStateSortingOrderChanged>();

        [SerializeField] UnityEvent activate;
        [SerializeField] UnityEvent deactivate;

        void ISceneStateInternal.InternalBecomeTopmost()
        {
            if (!isTopmost) {
                isTopmost = true;
                foreach (var it in OnBecomeTopmost.Enumerate())
                    it.Do();
            }
        }

        void ISceneStateInternal.InternalResignTopmost()
        {
            if (isTopmost) {
                isTopmost = false;
                foreach (var it in OnResignTopmost.Enumerate())
                    it.Do();
            }
        }

        void ISceneStateInternal.InternalActivate()
        {
            if (!isVisible) {
                isVisible = true;
                activate?.Invoke();
                foreach (var it in OnActivate.Enumerate())
                    it.Do();
            }
        }

        void ISceneStateInternal.InternalDeactivate()
        {
            if (isVisible) {
                isVisible = false;
                deactivate?.Invoke();
                foreach (var it in OnDeactivate.Enumerate())
                    it.Do();
            }
        }

        void ISceneStateInternal.InternalSetSortingOrder(int order)
        {
            if (sortingOrder != order) {
                sortingOrder = order;
                foreach (var it in OnSortingOrderChanged.Enumerate())
                    it.Do(order);
            }
        }
    }
}
