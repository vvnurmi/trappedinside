using UnityEngine;

/// <summary>
/// Waits for given time until continuing to the next action.
/// </summary>
public class TiaPause : ITiaAction, ITiaActionNew
{
    [YamlDotNet.Serialization.YamlMember(Alias = "Seconds")]
    public float DurationSeconds { get; set; }

    public bool IsDone { get; private set; }

    private float finishTime;

    public void Start(GameObject tiaRoot)
    {
        throw new System.NotImplementedException();
    }

    public void Update(TiaActor actor)
    {
        throw new System.NotImplementedException();
    }

    public void Start(ITiaActionContext context)
    {
        finishTime = Time.time + DurationSeconds;
    }

    public void Update(ITiaActionContext context)
    {
        if (IsDone) return;

        IsDone = Time.time >= finishTime;
    }
}
