using System;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Types text in a speech bubble on top of actor.
/// 
/// Speech bubbles are expected to have tagged child objects, each with
/// one <see cref="TMPro.TextMeshProUGUI"/> component. The tags mark the
/// role of the component as follows:
/// <list>
///     <item>"SpeechText": The proper text content of the bubble.</item>
///     <item>"SpeechSpeaker": The name of who is uttering the text.</item>
///     <item>"SpeechLeft": The left option for the player.</item>
///     <item>"SpeechRight": The right option for the player.</item>
/// </list>
/// </summary>
public class TiaSpeech : ITiaAction
{
    /// <summary>
    /// TextMesh Pro rich text to display in the speech bubble.
    /// </summary>
    [YamlMember(Alias = "Text")]
    public string TmpRichText { get; set; }

    /// <summary>
    /// Name of the speech bubble game object to display the speech in.
    /// </summary>
    [YamlMember(Alias = "Bubble")]
    public string SpeechBubbleName { get; set; }

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

    private NarrativeTypist narrativeTypist;
    private GameObject speechBubble;

    public bool IsDone => narrativeTypist?.State == NarrativeTypistState.Finished;

    public TiaSpeech()
    {
        TypingSpeedMultiplier = 1;
        IsModal = true;
    }

    public void Start(ITiaActionContext context)
    {
        var bubblePrefab = context.TiaRoot.FindChildByName(SpeechBubbleName);
        Debug.Assert(bubblePrefab != null);
        if (bubblePrefab == null) return;

        speechBubble = UnityEngine.Object.Instantiate(bubblePrefab, context.Actor.GameObject.transform);
        narrativeTypist = speechBubble.GetComponentInChildren<NarrativeTypist>();
        Debug.Assert(narrativeTypist != null, $"Speech bubble has no {nameof(NarrativeTypist)}");
        if (narrativeTypist == null) return;

        var typistSetup = new NarrativeTypistSetup
        {
            fullText = TmpRichText,
            speaker = context.Actor.GameObjectName,
            leftChoice = "Todo Left!!!",
            rightChoice = "Todo Right!!!",
        };
        narrativeTypist.GetComponent<NarrativeTypist>().StartTyping(typistSetup);
    }

    public void Update(ITiaActionContext context)
    {
        // All work is done by narrativeTypist.
    }

    public void Finish(ITiaActionContext context)
    {
        if (speechBubble != null)
            UnityEngine.Object.Destroy(speechBubble);
        speechBubble = null;
    }
}
