using UnityEngine;

/// <summary>
/// A sequence of actions that one actor does.
/// </summary>
public class TiaActionSequence
{
    public TiaActor Actor { get; set; }

    public ITiaAction[] Actions { get; set; }

    public bool IsDone => actionIndex >= Actions.Length;

    private int actionIndex;

    public void Start(GameObject tiaRoot)
    {
        Actor.Initialize(tiaRoot);
        actionIndex = 0;
        if (actionIndex < Actions.Length)
            Actions[actionIndex].Start(tiaRoot);
    }

    public void Update(GameObject tiaRoot)
    {
        if (IsDone) return;

        while (actionIndex < Actions.Length)
        {
            if (!Actions[actionIndex].IsDone)
                Actions[actionIndex].Update(Actor);
            if (!Actions[actionIndex].IsDone)
                break;

            actionIndex++;
            if (actionIndex < Actions.Length)
                Actions[actionIndex].Start(tiaRoot);
        }
    }
}
