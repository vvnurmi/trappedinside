using System;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Plays all action sequences simultaneously.
/// </summary>
[Serializable]
public class TiaStep
{
    [field: SerializeField]
    public TiaActionSequence[] Sequences { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone => Sequences.All(seq => seq.IsDone);

    public void Start(ITiaActionContext context)
    {
        TiaDebug.Log($"Starting " + DebugName);
        var subcontexts = Sequences
            .Select(sequence => context.CloneEmpty())
            .ToArray();
        context.Set(this, subcontexts);
        DoForSequences(context, (sequence, subcontext) => sequence.Start(subcontext));
    }

    public void Update(ITiaActionContext context)
    {
        DoForSequences(context, (sequence, subcontext) => sequence.Update(subcontext));
    }

    private void DoForSequences(
        ITiaActionContext context,
        Action<TiaActionSequence, ITiaActionContext> fun)
    {
        var (success, subcontexts) = context.Get<ITiaActionContext[]>(this);
        Debug.Assert(success && subcontexts.Length == Sequences.Length);

        for (int i = 0; i < Sequences.Length; i++)
            fun(Sequences[i], subcontexts[i]);
    }
}
