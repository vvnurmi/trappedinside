using UnityEngine;

/// <summary>
/// Waits for given time until continuing to the next action.
/// </summary>
public class TiaPause : ITiaAction
{
    [YamlDotNet.Serialization.YamlMember(Alias = "Seconds")]
    public float DurationSeconds { get; set; }

    public bool IsDone { get; private set; }

    private float finishTime;

    public void Start(ITiaActionContext context)
    {
        finishTime = Time.time + DurationSeconds;
    }

    public void Update(ITiaActionContext context)
    {
        if (IsDone) return;

        IsDone = Time.time >= finishTime;
    }

    public void Finish(ITiaActionContext context)
    {
    }
}
