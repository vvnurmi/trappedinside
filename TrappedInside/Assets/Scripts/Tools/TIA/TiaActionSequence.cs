using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// A sequence of actions that one actor does.
/// </summary>
[System.Serializable]
public class TiaActionSequence
{
    [field: SerializeField]
    public string Actor { get; set; }

    [field: SerializeReference]
    public ITiaAction[] Actions { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone => actionIndex >= Actions.Length;

    private int actionIndex;

    public void Start(ITiaActionContext context)
    {
        TiaDebug.Log($"Starting {DebugName}");
        actionIndex = 0;
        StoreActorInContext(context);

        if (actionIndex < Actions.Length)
        {
            TiaDebug.Log($"Starting {Actions[actionIndex].DebugName}"
                + $" of type {Actions[actionIndex].GetType()}"
                + $" for {context.Actor.GetFullName()}");
            Actions[actionIndex].Start(context);
        }
    }

    public void Update(ITiaActionContext context)
    {
        if (IsDone) return;

        context.SetActionSequence(this);
        while (actionIndex < Actions.Length)
        {
            if (!Actions[actionIndex].IsDone)
                Actions[actionIndex].Update(context);
            if (!Actions[actionIndex].IsDone)
                break;

            Actions[actionIndex].Finish(context);

            actionIndex++;
            if (actionIndex < Actions.Length)
            {
                TiaDebug.Log($"Starting {Actions[actionIndex].DebugName}"
                    + $" of type {Actions[actionIndex].GetType()}"
                    + $" for {context.Actor.GetFullName()}");
                Actions[actionIndex].Start(context);
            }
        }
    }

    private void StoreActorInContext(ITiaActionContext context)
    {
        if (Actor == null)
        {
            TiaDebug.Log($"No actor set for {DebugName}. This is fine unless an action needs an actor.");
            return;
        }

        var gameObject = context.TiaRoot.FindChildByName(Actor);
        if (gameObject == null)
        {
            TiaDebug.Warning($"Aborting {DebugName} because there's no '{Actor}' under {context.TiaRoot.GetFullName()}");
            actionIndex = Actions.Length; // IsDone => true
            return;
        }

        context.Actor = gameObject;
        TiaDebug.Log($"Resolved '{Actor}'"
            + $" into '{context.Actor.GetFullName()}'"
            + $" for {DebugName}");
    }
}
