namespace Foundation
{
    public interface ISceneState
    {
        bool IsTopmost { get; }
        bool IsVisible { get; }
        int SortingOrder { get; }

        bool UpdateParentState { get; }

        ObserverList<IOnUpdate> OnUpdate { get; }
        ObserverList<IOnUpdateDuringPause> OnUpdateDuringPause { get; }
        ObserverList<IOnFixedUpdate> OnFixedUpdate { get; }
        ObserverList<IOnLateUpdate> OnLateUpdate { get; }

        ObserverList<IOnStateActivate> OnActivate { get; }
        ObserverList<IOnStateDeactivate> OnDeactivate { get; }

        ObserverList<IOnStateBecomeTopmost> OnBecomeTopmost { get; }
        ObserverList<IOnStateResignTopmost> OnResignTopmost { get; }

        ObserverList<IOnStateSortingOrderChanged> OnSortingOrderChanged { get; }
    }
}
