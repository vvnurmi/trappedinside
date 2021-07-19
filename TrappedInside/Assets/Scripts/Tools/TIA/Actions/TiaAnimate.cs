using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Makes actor use this animation.
/// </summary>
[System.Serializable]
public class TiaAnimate : SimpleTiaActionBase, ITiaAction
{
    [YamlMember(Alias = "Name")]
    [field: SerializeField]
    public string AnimationName { get; set; }

    /// <summary>
    /// Returns true if the action is done.
    /// </summary>
    public override bool Update(ITiaActionContext context, GameObject actor)
    {
        var animator = actor?.GetComponent<Animator>();
        animator?.Play(AnimationName);
        return true;
    }
}
