using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Types text in a speech bubble on top of actor.
///
/// Speech bubbles are expected to have tagged child objects, each with
/// one <see cref="TMPro.TextMeshProUGUI"/> component. The role of each
/// component is marked by the tags <see cref="TagText"/>, <see cref="TagSpeaker"/>,
/// <see cref="TagLeft"/>, <see cref="TagRight"/>.
/// </summary>
[System.Serializable]
public class TiaSpeak : ITiaAction
{
    private class Context
    {
        public GameObject actor;
        public NarrativeTypist narrativeTypist;
        public GameObject speechBubble;
        public Task startTask;
        public bool isDoneOverride;
    }

    /// <summary>
    /// Tag on the text field component of a speech bubble game object to denote
    /// the proper text content of the bubble.
    /// </summary>
    public const string TagText = "SpeechText";

    /// <summary>
    /// Tag on the text field component of a speech bubble game object to denote
    /// the name of who is uttering the text.
    /// </summary>
    public const string TagSpeaker = "SpeechSpeaker";

    /// <summary>
    /// Tag on the text field component of a speech bubble game object to denote
    /// the left option for the player.
    /// </summary>
    public const string TagLeft = "SpeechLeft";

    /// <summary>
    /// Tag on the text field component of a speech bubble game object to denote
    /// the right option for the player.
    /// </summary>
    public const string TagRight = "SpeechRight";

    /// <summary>
    /// TextMesh Pro rich text to display in the speech bubble.
    /// </summary>
    [YamlMember(Alias = "Text")]
    [field: SerializeField]
    public string TmpRichText { get; set; }

    /// <summary>
    /// If <see cref="LeftChoice"/> and <see cref="RightChoice"/> are nonempty
    /// then the speech bubble will present the player a choice between the two.
    /// </summary>
    [YamlMember(Alias = "Left")]
    [field: SerializeField]
    public string LeftChoice { get; set; }

    /// <summary>
    /// If <see cref="LeftChoice"/> and <see cref="RightChoice"/> are nonempty
    /// then the speech bubble will present the player a choice between the two.
    /// </summary>
    [YamlMember(Alias = "Right")]
    [field: SerializeField]
    public string RightChoice { get; set; }

    /// <summary>
    /// Name of the speech bubble game object to display the speech in.
    /// </summary>
    [YamlMember(Alias = "Bubble")]
    [field: SerializeField]
    public string SpeechBubbleName { get; set; }

    /// <summary>
    /// Multiplier to the general typing speed.
    /// Defaults to 1.
    /// </summary>
    [YamlMember(Alias = "Speed")]
    [field: SerializeField]
    public float TypingSpeedMultiplier { get; set; }

    /// <summary>
    /// If true then prompt player after text, otherwise just wait for a while.
    /// Defaults to true.
    /// </summary>
    [YamlMember(Alias = "Modal")]
    [field: SerializeField]
    public bool IsModal { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone(ITiaActionContext context)
    {
        var (success, contextObject) = context.TryGet<Context>(this);
        return !success ? false
            : contextObject.isDoneOverride ? true
            : contextObject.narrativeTypist?.State == NarrativeTypistState.Finished;
    }

    public TiaSpeak()
    {
        TypingSpeedMultiplier = 1;
        IsModal = true;
    }

    #region ITiaAction

    public void Start(ITiaActionContext context, GameObject actor)
    {
        var contextObject = new Context { actor = actor };
        context.Set(this, contextObject);

        // Start the asynchronous work to create a speech bubble.
        // Hold on to the task with 'startTask' so that it doesn't get GC'd.
        contextObject.startTask = StartAsync(context, contextObject);
    }

    public void Update(ITiaActionContext context)
    {
        // All work is done by narrativeTypist.
    }

    public void Finish(ITiaActionContext context)
    {
        var contextObject = context.TryGet<Context>(this).contextObject;
        if (contextObject.speechBubble != null)
            Object.Destroy(contextObject.speechBubble);
        contextObject.speechBubble = null;
    }

    #endregion

    private async Task StartAsync(ITiaActionContext context, Context contextObject)
    {
        var bubblePrefab = await TiaTools.FindObject<GameObject>(context, SpeechBubbleName);
        Debug.Assert(bubblePrefab != null, $"{nameof(TiaSpeak)} will skip because it"
            + $" couldn't find speech bubble by name '{SpeechBubbleName}'");
        if (bubblePrefab == null)
        {
            contextObject.isDoneOverride = true;
            return;
        }

        TiaDebug.Log($"Instantiating speech bubble for {DebugName} under '{contextObject.actor.GetFullName()}'");
        contextObject.speechBubble = Object.Instantiate(bubblePrefab, contextObject.actor.transform);
        PositionAbove(who: contextObject.speechBubble, where: contextObject.actor);

        var typistType = string.IsNullOrEmpty(LeftChoice)
            ? typeof(NarrativeTypist)
            : typeof(NarrativeTypistChoice);
        contextObject.narrativeTypist = (NarrativeTypist)contextObject.speechBubble.GetComponent(typistType);
        if (contextObject.narrativeTypist == null)
        {
            Debug.LogWarning($"Speech bubble has no {typistType} component");
            contextObject.isDoneOverride = true;
            return;
        }

        if (string.IsNullOrEmpty(LeftChoice) != string.IsNullOrEmpty(RightChoice))
        {
            Debug.LogWarning($"{nameof(TiaSpeak)} left and right choices must both be"
                + $" empty or nonempty but are '{LeftChoice}' and '{RightChoice}'");
        }
        var typistSetup = new NarrativeTypistSetup
        {
            fullText = TmpRichText,
            speaker = contextObject.actor.name,
            choices = new[] { LeftChoice, RightChoice },
        };
        contextObject.narrativeTypist.StartTyping(typistSetup);
    }

    private static void PositionAbove(GameObject who, GameObject where)
    {
        var renderer = where.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            var middleTopLocal = new Vector3(0, renderer.bounds.extents.y, 0);
            who.transform.localPosition += middleTopLocal;
        }
    }
}
