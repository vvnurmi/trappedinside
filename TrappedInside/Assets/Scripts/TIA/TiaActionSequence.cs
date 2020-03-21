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
        {
            // TODO !!! This is in transition. Remove the old TiaAction interface in
            // favor of the new interface that uses a context object. !!!
            if (Actions[actionIndex] is ITiaActionNew action)
                action.Start(context);
            else
                Actions[actionIndex].Start(context.TiaRoot);
        }
    }

    public void Update(ITiaActionContext context)
    {
        if (IsDone) return;

        context.SetActionSequence(this);
        while (actionIndex < Actions.Length)
        {
            if (!Actions[actionIndex].IsDone)
            {
                // TODO !!! This is in transition. Remove the old TiaAction interface in
                // favor of the new interface that uses a context object. !!!
                if (Actions[actionIndex] is ITiaActionNew action)
                    action.Update(context);
                else
                    Actions[actionIndex].Update(Actor);
            }
            if (!Actions[actionIndex].IsDone)
                break;

            actionIndex++;
            if (actionIndex < Actions.Length)
            {
                // TODO !!! This is in transition. Remove the old TiaAction interface in
                // favor of the new interface that uses a context object. !!!
                if (Actions[actionIndex] is ITiaActionNew action)
                    action.Start(context);
                else
                    Actions[actionIndex].Start(context.TiaRoot);
            }
        }
    }
}
