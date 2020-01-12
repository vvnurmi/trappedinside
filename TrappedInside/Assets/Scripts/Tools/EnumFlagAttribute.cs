// From https://wiki.unity3d.com/index.php/EnumFlagPropertyDrawer

using UnityEngine;

/// <summary>
/// Flags an flag enum field as being editable as a bit field instead of
/// a regular single-valued enum.
/// </summary>
public class EnumFlagAttribute : PropertyAttribute
{
    public string enumName;

    public EnumFlagAttribute() { }

    public EnumFlagAttribute(string name)
    {
        enumName = name;
    }
}
