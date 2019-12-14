using System.Text;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    /// <summary>
    /// Returns the name of <paramref name="obj"/> with game object ancestor path included.
    /// May be helpful for debugging.
    /// </summary>
    public static string GetFullName(this MonoBehaviour obj)
    {
        var fullName = new StringBuilder("'", GameObjectExtensions.FullNameExpectedMaxLength);
        obj.GetFullNameStringBuilder(fullName);
        fullName.Append('\'');
        return fullName.ToString();
    }

    /// <summary>
    /// Writes to <paramref name="outFullName"/> the name of <paramref name="obj"/> with
    /// game object ancestor path included. May be helpful for debugging.
    /// </summary>
    public static void GetFullNameStringBuilder(this MonoBehaviour obj, StringBuilder outFullName)
    {
        obj.gameObject.GetFullNameStringBuilder(outFullName);
        outFullName.Append(':');
        outFullName.Append(obj.GetType().Name);
    }
}
