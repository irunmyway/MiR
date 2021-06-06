namespace Foundation
{
    interface ISceneStateInternal
    {
        void InternalBecomeTopmost();
        void InternalResignTopmost();
        void InternalActivate();
        void InternalDeactivate();
        void InternalSetSortingOrder(int order);
    }
}
