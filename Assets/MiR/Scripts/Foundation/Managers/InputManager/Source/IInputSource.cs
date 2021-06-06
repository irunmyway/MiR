namespace Foundation
{
    public interface IInputSource
    {
        IInputAction Action(string name);
        void DisconnectAllActions();
    }
}
