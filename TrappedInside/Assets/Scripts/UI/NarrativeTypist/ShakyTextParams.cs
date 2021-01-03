using System;
using System.Globalization;
using UnityEngine;

public struct ShakyTextParams
{
    [Tooltip("The maximum horizontal displacement of a character, in the object's coordinate system.")]
    public float Amplitude;

    public override string ToString()
    {
        FormattableString format = $"amplitude={Amplitude}";
        return format.ToString(CultureInfo.InvariantCulture);
    }

    public static ShakyTextParams Default =>
        new ShakyTextParams
        {
            Amplitude = 0.005f,
        };
}
