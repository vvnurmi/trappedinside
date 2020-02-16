using UnityEngine;

/// <summary>
/// Makes actor use this animation.
/// </summary>
public class TiaAnimation : ITiaAction
{
    public string AnimationName { get; set; }

    public bool IsDone { get; private set; }

    public void Start()
    {
    }

    public void Update(TiaActor actor)
    {
        var animator = actor.GameObject.GetComponent<Animator>();
        animator.Play(AnimationName);
        IsDone = true;
    }
}
