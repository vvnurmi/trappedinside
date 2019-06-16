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
    private bool dialogueAcked = false;

    private bool IsDoneTyping => charsToShow == text.Length;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        GameObject dialogueBox = playerData as GameObject;
        var textComponents = dialogueBox.GetComponentsInChildren<Text>();

        var speakerComponent = textComponents[0];
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
                dialogueBox.GetComponent<AudioSource>().Play();
        }

        var jumpReleased = Input.GetButtonUp("Jump");

        if (IsDoneTyping)
        {
            if(dialogueAcked)
            {
                SetSpeed(playable, 5);
            }
            else
            {
                SetSpeed(playable, 0);
                dialogueAcked = jumpReleased;
            }
        }
        else
        {
            if (jumpReleased)
            {
                SetSpeed(playable, 2);
                charsPerSecond *= 2;
            }
        }
    }

    private void SetSpeed(Playable playable, double value) => playable.GetGraph().GetRootPlayable(0).SetSpeed(value);

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        startTime = Time.time;
        SetSpeed(playable, 1);
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        SetSpeed(playable, 1);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
    }
}
