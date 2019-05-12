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

    // Modified during gameplay.
    private int choice;

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

    override protected void FixedUpdate()
    {
        base.FixedUpdate();

        if (!choiceComponent.enabled) return;

        ReadInput();
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

    private void ReadInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var isSubmitDown = Input.GetButtonDown("Submit");

        if (horizontal < 0)
            Choose(0);
        if (horizontal > 0)
            Choose(1);
        if (isSubmitDown)
            Confirm();
    }
}
