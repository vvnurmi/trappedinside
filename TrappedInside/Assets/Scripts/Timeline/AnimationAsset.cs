using UnityEngine;
using UnityEngine.Playables;

public enum TIAnimationState
{
    Idle, Talking, Walking, Running, GettingHit, Reaching
}

[System.Serializable]
public class AnimationAsset : PlayableAsset
{
    public TIAnimationState animationState;
    public TIAnimationState endAnimationState = TIAnimationState.Idle;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<AnimationBehaviour>.Create(graph);
        var animationBehaviour = playable.GetBehaviour();
        animationBehaviour.animationState = animationState;
        animationBehaviour.endAnimationState = endAnimationState;
        return playable;
    }
}
