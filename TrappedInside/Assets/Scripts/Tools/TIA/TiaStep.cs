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

    public bool IsDone(ITiaActionContext context)
        => Sequences.All(seq => seq.IsDone(context));

    public void Start(ITiaActionContext context)
    {
        TiaDebug.Log($"Starting " + DebugName);
        foreach (var sequence in Sequences)
            sequence.Start(context);
    }

    public void Update(ITiaActionContext context)
    {
        foreach (var sequence in Sequences)
            sequence.Update(context);
    }
}
