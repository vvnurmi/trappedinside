using System;
using System.Globalization;
using UnityEngine;

public struct WavyTextParams
{
    [Tooltip("The maximum displacement of a character, in the object's coordinate system.")]
    public float WaveAmplitude;
    [Tooltip("How many times a character waves back and forth in a second.")]
    public float WaveFrequency;
    [Tooltip("How many characters fit in one wave.")]
    public float WaveLength;

    public override string ToString()
    {
        FormattableString format = $"amplitude={WaveAmplitude} frequency={WaveFrequency} length={WaveLength}";
        return format.ToString(CultureInfo.InvariantCulture);
    }

    public static WavyTextParams Default =>
        new WavyTextParams
        {
            WaveAmplitude = 0.01f,
            WaveFrequency = 3,
            WaveLength = 10,
        };
}
