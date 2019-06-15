using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TalkAsset : PlayableAsset
{

    public string text = "Example text";
    public int charsPerSecond = 10;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<TalkBehaviour>.Create(graph);
        var talkBehaviour = playable.GetBehaviour();
        talkBehaviour.charsPerSecond = charsPerSecond;
        talkBehaviour.text = text;
        return playable;
    }
}
