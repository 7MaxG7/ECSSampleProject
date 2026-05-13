using CustomTypes;
using UnityEngine;

namespace Infrastructure
{
    public sealed class LogService
    {
        public static void LogDebug(DebugType debugType, string message)
        {
#if UNITY_EDITOR
            switch (debugType)
            {
                case DebugType.Log:
                    Debug.Log(message);
                    break;
                case DebugType.Warning:
                    Debug.LogWarning(message);
                    break;
                case DebugType.Error:
                    Debug.LogError(message);
                    break;
                default:
                    return;
            }
#endif
        }
    }
}