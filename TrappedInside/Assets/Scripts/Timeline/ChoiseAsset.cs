using UnityEngine;
using UnityEngine.Playables;

public class ChoiseAsset : PlayableAsset
{
    public string speaker = "Speaker";
    public Color speakerColor = new Color(1, 1, 1);
    public string text = "Example text";
    public string leftChoise = "Yes";
    public string rightChoise = "No";

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<ChoiseBehaviour>.Create(graph);
        var choiseBehaviour = playable.GetBehaviour();
        choiseBehaviour.text = text;
        choiseBehaviour.speaker = speaker;
        choiseBehaviour.speakerColor = speakerColor;
        choiseBehaviour.leftChoise = leftChoise;
        choiseBehaviour.rightChoise = rightChoise;
        return playable;
    }
}