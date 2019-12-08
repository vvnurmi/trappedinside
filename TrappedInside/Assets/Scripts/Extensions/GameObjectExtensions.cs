using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Returns the name of <paramref name="obj"/> with ancestor path included.
    /// May be helpful for debugging.
    /// </summary>
    public static string GetFullName(this GameObject obj)
    {
        var ancestors = new List<Transform>();
        for (var transform = obj.transform; transform != null; transform = transform.parent)
            ancestors.Add(transform);
        if (ancestors.Count == 0) return "???";

        var fullName = new StringBuilder("'", 128);
        for (int i = ancestors.Count - 1; i > 0; i--)
        {
            fullName.Append(ancestors[i].name);
            fullName.Append('/');
        }
        fullName.Append(ancestors[0].name);
        fullName.Append('\'');
        return fullName.ToString();
    }
}
