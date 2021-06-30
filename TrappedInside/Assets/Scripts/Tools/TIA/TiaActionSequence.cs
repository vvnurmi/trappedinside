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
        InitializeActor(context);

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

    private void InitializeActor(ITiaActionContext context)
    {
        if (Actor == null)
        {
            TiaDebug.Log($"No actor set for {DebugName}. This is fine unless an action needs an actor.");
            return;
        }

        var success = Actor.Initialize(context.TiaRoot);
        if (!success)
        {
            TiaDebug.Log($"Aborting {DebugName} because actor '{Actor.GameObjectName}' wasn't found");
            actionIndex = Actions.Length; // IsDone => true
            return;
        }
        TiaDebug.Log($"Resolved '{Actor.GameObjectName}' into '{context.Actor.GameObject.GetFullName()}' for {DebugName}");
    }
}
