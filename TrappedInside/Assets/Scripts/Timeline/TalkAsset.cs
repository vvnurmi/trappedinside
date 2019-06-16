using UnityEngine;
using UnityEngine.Playables;

public class TalkAsset : PlayableAsset
{

    public string speaker = "Speaker";
    public Color speakerColor = new Color(1, 1, 1);
    public string text = "Example text";

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<TalkBehaviour>.Create(graph);
        var talkBehaviour = playable.GetBehaviour();
        talkBehaviour.text = text;
        talkBehaviour.speaker = speaker;
        talkBehaviour.speakerColor = speakerColor;
        return playable;
    }
}
