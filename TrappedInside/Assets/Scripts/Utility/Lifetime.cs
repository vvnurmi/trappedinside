using UnityEngine;

public static class Lifetime
{
    /// <summary>
    /// Ensures that only one object with <typeparamref name="ObjectType"/>
    /// exists, and the object lives over scene transitions. Returns true if
    /// <paramref name="obj"/> is a duplicate that will die immediately.
    /// </summary>
    /// <example>
    /// <code>
    /// public class ThingBehaviour : MonoBehaviour
    /// {
    ///     private void OnEnable()
    ///     {
    ///         if (KillForUniquePersistence&lt;ThingBehaviour&gt;(gameObject))
    ///             return;
    ///         // Other stuff...
    ///     }
    /// </code>
    /// </example>
    public static bool KillForUniquePersistence<ObjectType>(GameObject obj) where ObjectType : Object
    {
        // Make object not get destroyed when a new scene is loaded.
        Object.DontDestroyOnLoad(obj);

        // If one of us already exists, don't let any more in.
        var others = Object.FindObjectsOfType<ObjectType>();
        Debug.Assert(others.Length >= 1);
        if (others.Length == 1) return false;

        Object.Destroy(obj);
        return true;
    }
}
