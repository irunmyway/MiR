namespace Foundation
{
    public sealed class DummyInputSource : IInputSource
    {
        public static readonly DummyInputSource Instance = new DummyInputSource();

        public IInputAction Action(string name)
        {
            return DummyInputAction.Instance;
        }

        public void DisconnectAllActions()
        {
        }
    }
}
