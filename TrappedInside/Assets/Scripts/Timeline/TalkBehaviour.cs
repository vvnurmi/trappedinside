using System;
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
    public string text = string.Empty;
    private bool dialogueAcked = false;


    private bool IsDoneTyping => charsToShow == text.Length;
    private bool DialogueReady => IsDoneTyping && dialogueAcked;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        GameObject dialogueBox = playerData as GameObject;

        var textComponent = dialogueBox.GetComponentInChildren<Text>();

        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * charsPerSecond),
            min: charsToShow,
            max: text.Length);

        textComponent.text = text.Substring(0, charsToShow);

        // If something more was typed, make noise and react to text end.
        if (oldCharsToShow < charsToShow)
        {
            var lastCharIsSpace = textComponent.text.Length == 0 ||
                char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
            if (!lastCharIsSpace)
                dialogueBox.GetComponent<AudioSource>().Play();
        }

        if(IsDoneTyping)
        {
            if(dialogueAcked)
            {
                playable.GetGraph().GetRootPlayable(0).SetSpeed(1);
            }
            else
            {
                playable.GetGraph().GetRootPlayable(0).SetSpeed(0);
                dialogueAcked = Input.GetKeyUp(KeyCode.Space);
            }
        }
    }

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        Console.WriteLine("OnGraphStart");
        //startTime = Time.time;
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        Console.WriteLine("OnGraphStart");

    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        startTime = Time.time;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        Console.WriteLine("OnGraphStart");

    }
}
