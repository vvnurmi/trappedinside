using System.Linq;
using YamlDotNet.Serialization;

/// <summary>
/// Plays all actions simultaneously, then waits.
/// </summary>
public class TiaStep
{
    public TiaActionSequence[] Sequences { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone => Sequences.All(seq => seq.IsDone);

    public void Start(ITiaActionContext context)
    {
        TiaDebug.Log($"Starting " + DebugName);
        foreach (var seq in Sequences)
            seq.Start(context);
    }

    public void Update(ITiaActionContext context)
    {
        foreach (var seq in Sequences)
            seq.Update(context);
    }
}
