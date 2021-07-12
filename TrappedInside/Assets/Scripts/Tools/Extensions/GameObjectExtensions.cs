using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class GameObjectExtensions
{
    public static int FullNameExpectedMaxLength = 128;

    /// <summary>
    /// Returns the name of <paramref name="obj"/> with ancestor path included.
    /// May be helpful for debugging.
    /// </summary>
    public static string GetFullName(this GameObject obj)
    {
        var fullName = new StringBuilder("'", FullNameExpectedMaxLength);
        obj.GetFullNameStringBuilder(fullName);
        fullName.Append('\'');
        return fullName.ToString();
    }

    /// <summary>
    /// Writes to <paramref name="outFullName"/> the name of <paramref name="obj"/>
    /// with ancestor path included.
    /// </summary>
    public static void GetFullNameStringBuilder(this GameObject obj, StringBuilder outFullName)
    {
        var ancestors = new List<Transform>();
        for (var transform = obj.transform; transform != null; transform = transform.parent)
            ancestors.Add(transform);

        if (ancestors.Count == 0)
        {
            outFullName.Append("???");
            return;
        }

        for (int i = ancestors.Count - 1; i > 0; i--)
        {
            outFullName.Append(ancestors[i].name);
            outFullName.Append('/');
        }
        outFullName.Append(ancestors[0].name);
    }

    /// <summary>
    /// Returns a child (or deeper descendant) of <see cref="obj"/> that has <see cref="name"/>.
    /// Returns null if no such child was found.
    /// </summary>
    public static GameObject FindChildByName(this GameObject obj, string name)
    {
        if (obj.name == name) return obj;

        foreach (Transform child in obj.transform)
        {
            var result = child.gameObject.FindChildByName(name);
            if (result != null)
                return result;
        }

        return null;
    }
}
