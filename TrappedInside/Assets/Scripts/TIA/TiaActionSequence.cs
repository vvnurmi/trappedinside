using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// A sequence of actions that one actor does.
/// </summary>
public class TiaActionSequence
{
    public TiaActor Actor { get; set; }

    public ITiaAction[] Actions { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone => actionIndex >= Actions.Length;

    private int actionIndex;

    public void Start(ITiaActionContext context)
    {
        TiaDebug.Log($"Starting {DebugName}");
        context.SetActionSequence(this);
        Actor.Initialize(context.TiaRoot);
        TiaDebug.Log($"Resolved '{Actor.GameObjectName}' into '{context.Actor.GameObject.GetFullName()}' for {DebugName}");

        actionIndex = 0;
        if (actionIndex < Actions.Length)
        {
            TiaDebug.Log($"Starting {Actions[actionIndex].DebugName} of type {Actions[actionIndex].GetType()} for {context.Actor.GameObject.GetFullName()}");
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
                TiaDebug.Log($"Starting {Actions[actionIndex].DebugName} of type {Actions[actionIndex].GetType()} for {context.Actor.GameObject.GetFullName()}");
                Actions[actionIndex].Start(context);
            }
        }
    }
}
