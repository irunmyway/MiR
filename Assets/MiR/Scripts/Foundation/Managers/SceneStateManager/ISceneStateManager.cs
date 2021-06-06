namespace Foundation
{
    public interface ISceneStateManager
    {
        ISceneState CurrentState { get; }
        void Push(ISceneState state);
        void PushTopmost(ISceneState state);
        void Pop(ISceneState state);
    }
}
