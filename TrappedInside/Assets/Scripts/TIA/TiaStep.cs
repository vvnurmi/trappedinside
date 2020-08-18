using System.Linq;
using UnityEngine;
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

    private ITiaActionContext[] sequenceContexts;

    public void Start(ITiaActionContext context)
    {
        TiaDebug.Log($"Starting " + DebugName);
        sequenceContexts = Sequences
            .Select(_ => context.Clone())
            .ToArray();
        Debug.Assert(Sequences.Length == sequenceContexts.Length);
        for (int sequenceIndex = 0; sequenceIndex < Sequences.Length; sequenceIndex++)
            Sequences[sequenceIndex].Start(sequenceContexts[sequenceIndex]);
    }

    public void Update(ITiaActionContext context)
    {
        // FIXME: 'context' is ignored now. A proper solution would be to maintain
        // a separate context for each class.
        Debug.Assert(Sequences.Length == sequenceContexts.Length);
        for (int sequenceIndex = 0; sequenceIndex < Sequences.Length; sequenceIndex++)
            Sequences[sequenceIndex].Update(sequenceContexts[sequenceIndex]);
    }
}
