using System.Linq;
using UnityEngine;

/// <summary>
/// Plays all actions simultaneously, then waits.
/// </summary>
public class TiaStep
{
    public TiaActionSequence[] Sequences { get; set; }

    public bool IsDone => Sequences.All(seq => seq.IsDone);

    public void Start(GameObject tiaRoot)
    {
        foreach (var seq in Sequences)
            seq.Start(tiaRoot);
    }

    public void Update(GameObject tiaRoot)
    {
        foreach (var seq in Sequences)
            seq.Update(tiaRoot);
    }
}
