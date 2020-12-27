using System;
using System.Globalization;
using UnityEngine;

public struct ShakyTextParams
{
    [Tooltip("?")]
    public float ShakeFoo; // !!!

    public override string ToString()
    {
        FormattableString format = $"x={ShakeFoo}";
        return format.ToString(CultureInfo.InvariantCulture);
    }

    public static ShakyTextParams Default =>
        new ShakyTextParams
        {
            ShakeFoo = 1,
        };
}
