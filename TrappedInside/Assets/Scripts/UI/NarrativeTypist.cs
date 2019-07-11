using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// </summary>
public class NarrativeTypist : MonoBehaviour
{
    // Set about once, probably in Start().
    private NarrativeTypistSettings settings;
    private TalkAnimator talkAnimator;
    private Text textComponent;
    private string fullText;
    private float startTime;

    // Modified during gameplay.
    private int charsToShow;

    public bool IsDoneTyping => charsToShow == fullText.Length;

    #region MonoBehaviour overrides

    virtual protected void Awake()
    {
        settings = GetComponentInParent<NarrativeTypistSettings>();
        talkAnimator = GetComponent<TalkAnimator>();
        Debug.Assert(settings != null,
            $"Expected to find {nameof(NarrativeTypistSettings)} from the parent of {nameof(NarrativeTypist)}");
        textComponent = GetComponentsInChildren<Text>().First(text => text.name == "Text");
        Debug.Assert(textComponent != null);
        fullText = textComponent.text;
    }

    private void OnEnable()
    {
        startTime = Time.time;
    }

    virtual protected void FixedUpdate()
    {
        ReadInput();

        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * settings.charsPerSecond),
            min: charsToShow,
            max: fullText.Length);
        textComponent.text = fullText.Substring(0, charsToShow);

        // If something more was typed, make noise and react to text end.
        if (oldCharsToShow < charsToShow)
        {
            talkAnimator.StartTalkingAnimation();
            var lastCharIsSpace = textComponent.text.Length == 0 ||
                char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
            if (!lastCharIsSpace)
                settings.audioSource.TryPlay(settings.characterSound);
            if (IsDoneTyping)
                OnTypingFinished();
        }
    }

    #endregion

    /// <summary>
    /// Called when typing has finished but has not yet been acknowledged.
    /// </summary>
    virtual protected void OnTypingFinished()
    {
        talkAnimator.StopTalkingAnimation();
        charsToShow = fullText.Length;
    }

    /// <summary>
    /// Called when typing has finished and the player has acknowledged it.
    /// </summary>
    virtual protected void OnTypingAcknowledged()
    {
        gameObject.SetActive(false);
    }

    private void ReadInput()
    {
        var isSubmitDown = Input.GetButtonDown("Submit");
        if (isSubmitDown)
        {
            // First reveal all of the text. If that's already the case
            // then "acknowledge" the dialog box and move on.
            if (!IsDoneTyping)
                OnTypingFinished();
            else
                OnTypingAcknowledged();
        }
    }
}
