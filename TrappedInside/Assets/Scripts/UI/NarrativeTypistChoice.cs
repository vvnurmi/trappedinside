using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// When the text is fully visible, makes the player choose.
/// </summary>
public class NarrativeTypistChoice : NarrativeTypist
{
    [Tooltip("Called when the player confirms his choice.")]
    public UnityEvent onResult;

    // Set about once, probably in Start().
    private NarrativeTypistSetup setup;
    private TMPro.TextMeshProUGUI[] textComponents = new TMPro.TextMeshProUGUI[2];

    // Modified during gameplay.
    private int choice;

    public override void StartTyping(NarrativeTypistSetup narrativeTypistSetup)
    {
        base.StartTyping(narrativeTypistSetup);
        setup = narrativeTypistSetup;
    }

    protected override void OnTypingFinished()
    {
        base.OnTypingFinished();
        Choose(0);
    }

    private void Choose(int index)
    {
        Debug.Assert(index >= 0 && index < textComponents.Length);
        choice = index;
        for (int textIndex = 0; textIndex < textComponents.Length; textIndex++)
        {
            textComponents[textIndex].text = textIndex == choice
                ? "[" + setup.choices[textIndex] + "]"
                : setup.choices[textIndex];
        }
    }

    private void Confirm()
    {
        Debug.Assert(choice >= 0 && choice < textComponents.Length);
        // TODO: Pass these as parameters: choice, setup.choices[choice]
        onResult?.Invoke();
        base.OnTypingAcknowledged();
    }

    private void HandleInput(TIInputState inputState)
    {
        if (inputState.uiNavigate.x <= -0.5f)
            Choose(0);
        if (inputState.uiNavigate.x >= 0.5f)
            Choose(1);
    }

    #region MonoBehaviour overrides

    protected override void Awake()
    {
        base.Awake();
        textComponents[0] = GetComponentsInChildren<TMPro.TextMeshProUGUI>()
            .Single(text => text.gameObject.CompareTag(TiaSpeak.TagLeft));
        textComponents[1] = GetComponentsInChildren<TMPro.TextMeshProUGUI>()
            .Single(text => text.gameObject.CompareTag(TiaSpeak.TagRight));
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (State != NarrativeTypistState.UserPrompt) return;

        var inputState = inputContext.GetStateAndResetEventFlags();
        HandleInput(inputState);
    }

    #endregion

}
