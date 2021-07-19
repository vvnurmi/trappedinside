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

    /// <summary>
    /// Log a warning with TIA tag.
    /// Good for grouping TIA log messages.
    /// Note: Not conditionally compiled so as not to hide warnings.
    /// </summary>
    public static void Warning(string message) =>
        Debug.unityLogger.LogWarning("TIA", message);
}
