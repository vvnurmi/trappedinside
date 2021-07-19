using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Helper base class for TIA actions that only need to store the actor and
/// the completion flag in their context object.
/// </summary>
public abstract class SimpleTiaActionBase : ITiaAction
{
    private class Context
    {
        public GameObject actor;
        public bool isDone;
    }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone(ITiaActionContext context)
    {
        var (success, contextObject) = context.TryGet<Context>(this);
        return !success ? false
            : contextObject.isDone;
    }

    #region Subclass overridables

    /// <summary>
    /// If overridden by subclass then this base implementation should be called first.
    /// </summary>
    public virtual void Start(ITiaActionContext context, GameObject actor)
    {
        context.Set(this, new Context { actor = actor });
    }

    /// <summary>
    /// Subclass implementation should return true if the action is done.
    /// </summary>
    public abstract bool Update(ITiaActionContext context, GameObject actor);

    public virtual void Finish(ITiaActionContext context, GameObject actor) { }

    #endregion

    /// <summary>
    /// Subclass should override <see cref="Update(GameObject)"/>
    /// for simple access to reading the actor and storing the completion flag.
    /// </summary>
    public void Update(ITiaActionContext context)
    {
        var contextObject = context.TryGet<Context>(this).contextObject;
        contextObject.isDone = Update(context, contextObject.actor);
    }

    /// <summary>
    /// Subclass can override <see cref="Finish(GameObject)"/> instead
    /// for simple access to reading the actor.
    /// </summary>
    public void Finish(ITiaActionContext context)
    {
        var contextObject = context.TryGet<Context>(this).contextObject;
        Finish(context, contextObject.actor);
    }
}
