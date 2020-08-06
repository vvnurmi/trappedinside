/// <summary>
/// A sequence of actions that one actor does.
/// </summary>
public class TiaActionSequence
{
    public TiaActor Actor { get; set; }

    public ITiaAction[] Actions { get; set; }

    public bool IsDone => actionIndex >= Actions.Length;

    private int actionIndex;

    public void Start(ITiaActionContext context)
    {
        context.SetActionSequence(this);
        Actor.Initialize(context.TiaRoot);

        actionIndex = 0;
        if (actionIndex < Actions.Length)
            Actions[actionIndex].Start(context);
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
                Actions[actionIndex].Start(context);
        }
    }
}
