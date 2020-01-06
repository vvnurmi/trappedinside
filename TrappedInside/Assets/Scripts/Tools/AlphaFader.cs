using System;
using UnityEngine;

/// <summary>
/// Helper to do an alpha fade in or out.
/// </summary>
public class AlphaFader
{
    private float startTime;
    private float endTime;
    private float beginAlpha;
    private float endAlpha;
    private Action<float> setAlpha;

    public bool IsDone { get; private set; }

    /// <summary>
    /// Initialize a fade. Call <see cref="Update"/> regularly after this.
    /// </summary>
    public void StartFade(
        float fadeSeconds,
        float beginAlpha,
        float endAlpha,
        Action<float> setAlpha)
    {
        IsDone = false;

        startTime = Time.time;
        endTime = startTime + fadeSeconds;
        this.beginAlpha = beginAlpha;
        this.endAlpha = endAlpha;
        this.setAlpha = setAlpha;

        setAlpha(beginAlpha);
    }

    /// <summary>
    /// To be called regularly to make the fade happen.
    /// </summary>
    public void Update()
    {
        var lerpParam = Mathf.InverseLerp(startTime, endTime, Time.time);
        var alpha = Mathf.Lerp(beginAlpha, endAlpha, lerpParam);
        setAlpha(alpha);

        if (lerpParam == 1.0f)
            IsDone = true;
    }
}
