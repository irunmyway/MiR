using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Foundation
{
    public static class DebugOnly
    {
        [Conditional("UNITY_EDITOR")]
        public static void Message(string message)
        {
            Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Warn(string message)
        {
            Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Error(string message)
        {
            Debug.LogError(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Check(bool condition, string message)
        {
            if (!condition)
                Debug.LogError($"Debug check failed: {message}");
        }
    }
}
