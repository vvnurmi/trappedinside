using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class AnimationBehaviour : PlayableBehaviour
{
    private Animator animator;
    public TIAnimationState animationState;
    public TIAnimationState endAnimationState;

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
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        animator?.Play(endAnimationState.ToString());
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(animator == null)
        {
            animator = (playerData as GameObject).GetComponent<Animator>();
        }
        animator.Play(animationState.ToString());

    }
}
