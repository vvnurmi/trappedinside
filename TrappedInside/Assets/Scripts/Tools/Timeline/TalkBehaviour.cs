using TMPro;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TalkBehaviour : PlayableBehaviour
{
    // Modified during gameplay.
    private int charsToShow;
    private float startTime;
    public int charsPerSecond = 20;
    public DialogSettings dialogSettings;
    public GameObject gameObject;
    private bool dialogAcked = false;
    private bool leftChoiceSelected = true;
    private bool IsSelectionEnabled => dialogSettings.LeftChoice.Length > 0 && dialogSettings.RightChoice.Length > 0;

    private bool IsDoneTyping => charsToShow == dialogSettings.Text.Length;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        GameObject dialogBox = playerData as GameObject;
        var textComponents = dialogBox.GetComponentsInChildren<TextMeshProUGUI>();

        var speakerComponent = textComponents[0];
        speakerComponent.color = dialogSettings.SpeakerColor;
        var textComponent = textComponents[1];


        if (IsDoneTyping)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            PlayIdleAnimation();

            if (horizontalInput < 0)
            {
                leftChoiceSelected = true;
            }
            if (horizontalInput > 0)
            {
                leftChoiceSelected = false;
            }

            if (dialogAcked)
            {
                SetSpeed(playable, 50);
                textComponents[2].text = "";
                textComponents[3].text = "";
            }
            else if (IsSelectionEnabled)
            {
                SetSpeed(playable, 0);
                textComponents[2].text = leftChoiceSelected ? $"[{dialogSettings.LeftChoice}]" : $" {dialogSettings.LeftChoice} ";
                textComponents[3].text = leftChoiceSelected ? $" {dialogSettings.RightChoice} " : $"[{dialogSettings.RightChoice}]";
                dialogAcked = Input.GetButtonUp("Jump");
            }
            else
            {
                SetSpeed(playable, 0);
                dialogAcked = Input.GetButtonUp("Jump");
            }
        }
        else
        {
            var oldCharsToShow = charsToShow;
            if (Input.GetButtonUp("Jump"))
            {
                charsToShow = dialogSettings.Text.Length;
            }
            else
            {
                charsToShow = Mathf.Clamp(
                    value: Mathf.RoundToInt((Time.time - startTime) * charsPerSecond),
                    min: charsToShow,
                    max: dialogSettings.Text.Length);
            }

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
            PlayTalkingAnimation();
        }

    }

    private void SetSpeed(Playable playable, double value) => playable.GetGraph().GetRootPlayable(0).SetSpeed(value);

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        startTime = Time.time;
        SetSpeed(playable, 1);
        PlayTalkingAnimation();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        SetSpeed(playable, 1);
        PlayIdleAnimation();
    }

    private void PlayTalkingAnimation() => PlayAnimation("Talking");
    private void PlayIdleAnimation() => PlayAnimation("Idle");

    private void PlayAnimation(string animation) => gameObject.GetComponent<Animator>().Play(animation);
}