using UnityEngine;

public static class TiaDebug
{
    /// <summary>
    /// Log a message with TIA tag.
    /// Good for grouping TIA log messages and also to
    /// conditionally exclude TIA logs from compilation.
    /// </summary>
    [System.Diagnostics.Conditional("TIA_DEBUG")]
    public static void Log(string message) =>
        Debug.unityLogger.Log("TIA", message);
}
