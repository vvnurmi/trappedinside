using UnityEngine;

public static class TiaDebug
{
    [System.Diagnostics.Conditional("TIA_DEBUG")]
    public static void Log(string message) =>
        Debug.unityLogger.Log("TIA", message);
}
