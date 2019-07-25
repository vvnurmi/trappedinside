using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

// A behaviour that is attached to a playable
public class TalkBehaviour : PlayableBehaviour
{
    // Modified during gameplay.
    private int charsToShow;
    private float startTime;
    public int charsPerSecond = 20;
    public DialogSettings dialogSettings;
    private bool dialogAcked = false;
    private bool leftChoiseSelected = true;
    private bool IsSelectionEnabled => dialogSettings.LeftChoise.Length > 0 && dialogSettings.RightChoise.Length > 0;


    private bool IsDoneTyping => charsToShow == dialogSettings.Text.Length;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        GameObject dialogBox = playerData as GameObject;
        var textComponents = dialogBox.GetComponentsInChildren<Text>();

        var speakerComponent = textComponents[0];
        speakerComponent.color = dialogSettings.SpeakerColor;
        var textComponent = textComponents[1];

        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * charsPerSecond),
            min: charsToShow,
            max: dialogSettings.Text.Length);

        speakerComponent.text = dialogSettings.SpeakerName;
        textComponent.text = dialogSettings.Text.Substring(0, charsToShow);

        // If something more was typed, make noise and react to text end.
        if (oldCharsToShow < charsToShow)
        {
            var lastCharIsSpace = textComponent.text.Length == 0 ||
                char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
            if (!lastCharIsSpace)
                dialogBox.GetComponent<AudioSource>().Play();
        }

        if (IsDoneTyping)
        {
            var horizontalInput = Input.GetAxis("Horizontal");

            if (horizontalInput < 0)
            {
                leftChoiseSelected = true;
            }
            if (horizontalInput > 0)
            {
                leftChoiseSelected = false;
            }

            if (dialogAcked)
            {
                SetSpeed(playable, 5);
                textComponents[2].text = "";
                textComponents[3].text = "";
            }
            else if (IsSelectionEnabled)
            {
                SetSpeed(playable, 0);
                textComponents[2].text = leftChoiseSelected ? $"[{dialogSettings.LeftChoise}]" : $" {dialogSettings.LeftChoise} ";
                textComponents[3].text = leftChoiseSelected ? $" {dialogSettings.RightChoise} " : $"[{dialogSettings.RightChoise}]";
                dialogAcked = Input.GetButtonUp("Jump");
            }
            else
            {
                SetSpeed(playable, 0);
                dialogAcked = Input.GetButtonUp("Jump");
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
