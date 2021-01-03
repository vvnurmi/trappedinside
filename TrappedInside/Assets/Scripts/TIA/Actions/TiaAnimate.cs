using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Makes actor use this animation.
/// </summary>
public class TiaAnimate : ITiaAction
{
    [YamlMember(Alias = "Name")]
    public string AnimationName { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

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

    public void Finish(ITiaActionContext context)
    {
    }
}
