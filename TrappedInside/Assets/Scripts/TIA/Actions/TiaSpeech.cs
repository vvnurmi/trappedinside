using System;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Types text in a speech bubble on top of actor.
/// </summary>
public class TiaSpeech : ITiaAction, ITiaActionNew
{
    /// <summary>
    /// TextMesh Pro rich text to display in the speech bubble.
    /// </summary>
    [YamlMember(Alias = "Text")]
    public string TmpRichText { get; set; }

    /// <summary>
    /// Multiplier to the general typing speed.
    /// Defaults to 1.
    /// </summary>
    [YamlMember(Alias = "Speed")]
    public float TypingSpeedMultiplier { get; set; }

    /// <summary>
    /// If true then prompt player after text, otherwise just wait for a while.
    /// Defaults to true.
    /// </summary>
    [YamlMember(Alias = "Modal")]
    public bool IsModal { get; set; }

    public bool IsDone => throw new NotImplementedException();

    public TiaSpeech()
    {
        TypingSpeedMultiplier = 1;
        IsModal = true;
    }

    public void Start(GameObject tiaRoot)
    {
        throw new NotImplementedException();
    }

    public void Update(TiaActor actor)
    {
        throw new NotImplementedException();
    }

    public void Start(ITiaActionContext context)
    {
        throw new NotImplementedException();
    }

    public void Update(ITiaActionContext context)
    {
        throw new NotImplementedException();
    }
}
