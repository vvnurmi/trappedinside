using System;

/// <summary>
/// Types text in a speech bubble on top of actor.
/// </summary>
public class TiaSpeech : ITiaAction
{
    /// <summary>
    /// TextMesh Pro rich text to display in the speech bubble.
    /// </summary>
    public string tmpRichText { get; set; }
    public float typingSpeedMultiplier { get; set; }

    /// <summary>
    /// If true then prompt player after text, otherwise just wait for a while.
    /// </summary>
    public bool modal;

    public bool IsDone => throw new NotImplementedException();

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Update(TiaActor actor)
    {
        throw new NotImplementedException();
    }
}
