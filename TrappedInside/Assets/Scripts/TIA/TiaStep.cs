using System.Linq;

/// <summary>
/// Plays all actions simultaneously, then waits.
/// </summary>
public class TiaStep
{
    public TiaActionSequence[] Sequences { get; set; }

    public bool IsDone => Sequences.All(seq => seq.IsDone);

    public void Start(ITiaActionContext context)
    {
        foreach (var seq in Sequences)
            seq.Start(context);
    }

    public void Update(ITiaActionContext context)
    {
        foreach (var seq in Sequences)
            seq.Update(context);
    }
}
