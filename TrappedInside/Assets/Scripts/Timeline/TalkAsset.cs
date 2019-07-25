using System;
using UnityEngine;
using UnityEngine.Playables;

public enum Character
{
    Mike, Randolph, Brokenshield, Amazon, Justin, FanDuin
}

public class DialogSettings
{
    public DialogSettings(string name, Color color)
    {
        SpeakerName = name;
        SpeakerColor = color;
    }

    public string SpeakerName { get; }
    public Color SpeakerColor { get; }
}

public static class DialogSettingsCreator
{
    public static DialogSettings Create(Character character)
    {
        switch(character)
        {
            case Character.Randolph:
                return new DialogSettings("Randolph", new Color(0.7f, 0.32f, 0.97f));
            case Character.Mike:
                return new DialogSettings("Mike", new Color(0.39f, 0.35f, 0.99f));
            case Character.Amazon:
                return new DialogSettings("Amazon", new Color(0.94f, 0.52f, 0.18f));
            case Character.Brokenshield:
                return new DialogSettings("Brokenshield", new Color(0.94f, 0.3f, 0.2f));
            case Character.Justin:
                return new DialogSettings("Justin", new Color(0.96f, 0.47f, 0.56f));
            case Character.FanDuin:
                return new DialogSettings("Fan Duin", new Color(0.2f, 0.95f, 0.28f));
            default:
                throw new ArgumentException($"Invalid talk asset settings: {character.ToString()}");
        }
    }
}

public class TalkAsset : PlayableAsset
{
    public Character character;
    public string text = "Example text";

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<TalkBehaviour>.Create(graph);
        var talkBehaviour = playable.GetBehaviour();
        talkBehaviour.text = text;
        talkBehaviour.dialogSettings = DialogSettingsCreator.Create(character);
        return playable;
    }
}
