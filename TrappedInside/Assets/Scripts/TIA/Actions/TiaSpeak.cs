using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using YamlDotNet.Serialization;

/// <summary>
/// Types text in a speech bubble on top of actor.
///
/// Speech bubbles are expected to have tagged child objects, each with
/// one <see cref="TMPro.TextMeshProUGUI"/> component. The role of each
/// component is marked by the tags <see cref="TagText"/>, <see cref="TagSpeaker"/>,
/// <see cref="TagLeft"/>, <see cref="TagRight"/>.
/// </summary>
public class TiaSpeak : ITiaAction
{
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
    public string TmpRichText { get; set; }

    /// <summary>
    /// If <see cref="LeftChoice"/> and <see cref="RightChoice"/> are nonempty
    /// then the speech bubble will present the player a choice between the two.
    /// </summary>
    [YamlMember(Alias = "Left")]
    public string LeftChoice { get; set; }

    /// <summary>
    /// If <see cref="LeftChoice"/> and <see cref="RightChoice"/> are nonempty
    /// then the speech bubble will present the player a choice between the two.
    /// </summary>
    [YamlMember(Alias = "Right")]
    public string RightChoice { get; set; }

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

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone
        => isDoneOverride
        || narrativeTypist?.State == NarrativeTypistState.Finished;

    private NarrativeTypist narrativeTypist;
    private GameObject speechBubble;
    private Task startTask;
    private bool isDoneOverride;

    public TiaSpeak()
    {
        TypingSpeedMultiplier = 1;
        IsModal = true;
    }

    private async Task<GameObject> FindObject(ITiaActionContext context, string name)
    {
        const string AddressableNamePrefix = "addressable:";
        if (SpeechBubbleName.StartsWith(AddressableNamePrefix))
        {
            var addressableName = SpeechBubbleName.Substring(AddressableNamePrefix.Length);
            var loadTask = Addressables.LoadAssetAsync<GameObject>(addressableName).Task;
            await loadTask;
            if (loadTask.Status != TaskStatus.RanToCompletion)
            {
                Debug.LogWarning($"{nameof(TiaSpeak)} loading addressable '{addressableName}' ended as {loadTask.Status}");
                return null;
            }
            return loadTask.Result;
        }
        return context.TiaRoot.FindChildByName(SpeechBubbleName);
    }

    #region ITiaAction

    public void Start(ITiaActionContext context)
    {
        // Start the asynchronous work to create a speech bubble.
        // Hold on to the task with 'startTask' so that it doesn't get GC'd.
        startTask = StartAsync(context);
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

    #endregion

    private async Task StartAsync(ITiaActionContext context)
    {
        var bubblePrefab = await FindObject(context, SpeechBubbleName);
        Debug.Assert(bubblePrefab != null, $"{nameof(TiaSpeak)} will skip because it"
            + $" couldn't find speech bubble by name '{SpeechBubbleName}'");
        if (bubblePrefab == null)
        {
            isDoneOverride = true;
            return;
        }

        TiaDebug.Log($"Instantiating speech bubble for {DebugName} under '{context.Actor.GameObject.GetFullName()}'");
        speechBubble = Object.Instantiate(bubblePrefab, context.Actor.GameObject.transform);
        PositionAbove(who: speechBubble, where: context.Actor.GameObject);

        var typistType = string.IsNullOrEmpty(LeftChoice)
            ? typeof(NarrativeTypist)
            : typeof(NarrativeTypistChoice);
        narrativeTypist = (NarrativeTypist)speechBubble.GetComponentInChildren(typistType);
        if (narrativeTypist == null)
        {
            Debug.LogWarning($"Speech bubble has no {typistType} component");
            isDoneOverride = true;
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
            speaker = context.Actor.GameObjectName,
            choices = new[] { LeftChoice, RightChoice },
        };
        narrativeTypist.StartTyping(typistSetup);
    }

    private void PositionAbove(GameObject who, GameObject where)
    {
        var renderer = where.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            var middleTopLocal = new Vector3(0, renderer.bounds.extents.y, 0);
            speechBubble.transform.localPosition += middleTopLocal;
        }
    }
}
