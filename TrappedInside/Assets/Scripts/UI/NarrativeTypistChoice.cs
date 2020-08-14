using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// When the text is fully visible, makes the player choose.
/// </summary>
public class NarrativeTypistChoice : NarrativeTypist
{
    [Tooltip("Left choice.")]
    public string leftChoice = "Yes";

    [Tooltip("Right choice.")]
    public string rightChoice = "No";

    [Tooltip("Called when the player confirms his choice.")]
    public UnityEvent onResult;

    // Set about once, probably in Start().
    private Text choiceComponent;
    private string[] textContents;
    private TMPro.TextMeshProUGUI[] textComponents = new TMPro.TextMeshProUGUI[2];

    // Modified during gameplay.
    private int choice;

    public override void StartTyping(NarrativeTypistSetup setup)
    {
        base.StartTyping(setup);

        var textFields = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (var textField in textFields)
        {
            if (textField.gameObject.CompareTag("SpeechLeft"))
            {
                textComponents[0] = textField;
                textField.text = "";
            }
            if (textField.gameObject.CompareTag("SpeechRight"))
            {
                textComponents[1] = textField;
                textField.text = "";
            }
        }
        for (int i = 0; i < textComponents.Length; i++)
            Debug.Assert(textComponents[i] != null, $"{nameof(NarrativeTypistChoice)} couldn't find choice text field {i}");
    }

    #region MonoBehaviour overrides

    override protected void Awake()
    {
        base.Awake();

        choiceComponent = GetComponentsInChildren<Text>().First(text => text.name == "Choice");
        Debug.Assert(choiceComponent != null);
        choiceComponent.enabled = false;

        textContents = new[]
        {
            $"{leftChoice} <<    {rightChoice}",
            $"{leftChoice}    >> {rightChoice}",
        };
    }

    #endregion

    protected override void OnTypingFinished()
    {
        base.OnTypingFinished();
        Choose(0);
        choiceComponent.enabled = true;
    }

    private void Choose(int index)
    {
        Debug.Assert(index >= 0 && index < textContents.Length);
        choice = index;
        choiceComponent.text = textContents[index];
    }

    private void Confirm()
    {
        Debug.Assert(choice >= 0 && choice < textContents.Length);
        // TODO: Pass these as parameters: choice, textContents[choice]
        onResult?.Invoke();
        base.OnTypingAcknowledged();
    }


    override protected void HandleInput(TIInputState inputState)
    {
        if (choiceComponent.enabled)
        {
            if (inputState.uiNavigate.x <= -0.5f)
                Choose(0);
            if (inputState.uiNavigate.x >= 0.5f)
                Choose(1);
            if (inputState.uiSubmitPressed)
                Confirm();
        }

        base.HandleInput(inputState);
    }
}
