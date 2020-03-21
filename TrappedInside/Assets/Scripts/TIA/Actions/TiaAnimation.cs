using UnityEngine;

/// <summary>
/// Makes actor use this animation.
/// </summary>
public class TiaAnimation : ITiaAction
{
    [YamlDotNet.Serialization.YamlMember(Alias = "Name")]
    public string AnimationName { get; set; }

    public bool IsDone { get; private set; }

    public void Start(ITiaActionContext context)
    {
    }

    public void Update(ITiaActionContext context)
    {
        var animator = context.Actor.GameObject.GetComponent<Animator>();
        animator.Play(AnimationName);
        IsDone = true;
    }
}
