using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// A sequence of actions that one actor does.
/// </summary>
[System.Serializable]
public class TiaActionSequence
{
    private class Context
    {
        public int actionIndex;
        public GameObject actor;
    }

    [field: SerializeField]
    public string Actor { get; set; }

    [field: SerializeReference]
    public ITiaAction[] Actions { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone(ITiaActionContext context)
    {
        var (success, contextObject) = context.TryGet<Context>(this);
        return !success ? false
            : contextObject.actionIndex >= Actions.Length;
    }

    public void Start(ITiaActionContext context)
    {
        TiaDebug.Log($"Starting {DebugName}");
        var (success, actor) = TryResolveActor(context);
        var contextObject = new Context
        {
            actionIndex = success ? 0 : Actions.Length, // IsDone => true
            actor = actor,
        };
        context.Set(this, contextObject);

        if (contextObject.actionIndex < Actions.Length)
        {
            TiaDebug.Log($"Starting {Actions[contextObject.actionIndex].DebugName}"
                + $" of type {Actions[contextObject.actionIndex].GetType()}"
                + $" for {contextObject.actor.GetFullName()}");
            Actions[contextObject.actionIndex].Start(context, contextObject.actor);
        }
    }

    public void Update(ITiaActionContext context)
    {
        if (IsDone(context)) return;

        var contextObject = context.TryGet<Context>(this).contextObject;
        while (contextObject.actionIndex < Actions.Length)
        {
            if (!Actions[contextObject.actionIndex].IsDone(context))
                Actions[contextObject.actionIndex].Update(context);
            if (!Actions[contextObject.actionIndex].IsDone(context))
                break;

            Actions[contextObject.actionIndex].Finish(context);

            contextObject.actionIndex++;
            if (contextObject.actionIndex < Actions.Length)
            {
                TiaDebug.Log($"Starting {Actions[contextObject.actionIndex].DebugName}"
                    + $" of type {Actions[contextObject.actionIndex].GetType()}"
                    + $" for {contextObject.actor.GetFullName()}");
                Actions[contextObject.actionIndex].Start(context, contextObject.actor);
            }
        }
    }

    private (bool success, GameObject actor) TryResolveActor(ITiaActionContext context)
    {
        if (Actor == null)
        {
            TiaDebug.Log($"No actor set for {DebugName}. This is fine unless an action needs an actor.");
            return (success: true, actor: null);
        }

        var gameObject = context.TiaRoot.FindChildByName(Actor);
        if (gameObject == null)
        {
            TiaDebug.Warning($"Aborting {DebugName} because there's no '{Actor}' under {context.TiaRoot.GetFullName()}");
            return (success: false, actor: null);
        }

        TiaDebug.Log($"Resolved '{Actor}'"
            + $" into '{gameObject.GetFullName()}'"
            + $" for {DebugName}");
        return (success: true, actor: gameObject);
    }
}
