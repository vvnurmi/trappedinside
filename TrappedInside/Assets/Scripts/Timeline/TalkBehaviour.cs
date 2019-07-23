using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

// A behaviour that is attached to a playable
public class TalkBehaviour : PlayableBehaviour
{
    // Modified during gameplay.
    private int charsToShow;
    private float startTime;
    public int charsPerSecond = 10;
    public string speaker = string.Empty;
    public string text = string.Empty;
    public Color speakerColor = new Color(1, 1, 1);
    private bool dialogAcked = false;

    private bool IsDoneTyping => charsToShow == text.Length;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        GameObject dialogBox = playerData as GameObject;
        var textComponents = dialogBox.GetComponentsInChildren<Text>();

        var speakerComponent = textComponents[0];
        speakerComponent.color = speakerColor;
        var textComponent = textComponents[1];

        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * charsPerSecond),
            min: charsToShow,
            max: text.Length);

        speakerComponent.text = speaker;
        textComponent.text = text.Substring(0, charsToShow);

        // If something more was typed, make noise and react to text end.
        if (oldCharsToShow < charsToShow)
        {
            var lastCharIsSpace = textComponent.text.Length == 0 ||
                char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
            if (!lastCharIsSpace)
                dialogBox.GetComponent<AudioSource>().Play();
        }

        var jumpReleased = Input.GetButtonUp("Jump");

        if (IsDoneTyping)
        {
            if(dialogAcked)
            {
                SetSpeed(playable, 5);
            }
            else
            {
                SetSpeed(playable, 0);
                dialogAcked = jumpReleased;
            }
        }
    }

    private void SetSpeed(Playable playable, double value) => playable.GetGraph().GetRootPlayable(0).SetSpeed(value);

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        startTime = Time.time;
        SetSpeed(playable, 1);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        SetSpeed(playable, 1);
    }
}
