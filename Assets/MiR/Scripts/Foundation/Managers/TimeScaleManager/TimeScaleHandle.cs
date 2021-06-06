namespace Foundation
{
    public sealed class TimeScaleHandle : ITimeScaleHandleInternal
    {
        public bool IsValid { get; private set; }
        public float Scale { get; private set; }

        void ITimeScaleHandleInternal.Init(float scale)
        {
            DebugOnly.Check(!IsValid, "Attempted to use TimeScaleHandle multiple times.");
            Scale = scale;
            IsValid = true;
        }

        void ITimeScaleHandleInternal.Reset()
        {
            DebugOnly.Check(IsValid, "Attempted to reset invalid TimeScaleHandle.");
            Scale = 0.0f;
            IsValid = false;
        }
    }
}
