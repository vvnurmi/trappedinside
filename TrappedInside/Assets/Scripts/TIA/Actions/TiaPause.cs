using UnityEngine;

/// <summary>
/// Waits for given time until continuing to the next action.
/// </summary>
public class TiaPause : ITiaAction
{
    public float DurationSeconds { get; set; }

    public bool IsDone { get; private set; }

    private float finishTime;

    public void Start()
    {
        finishTime = Time.time + DurationSeconds;
    }

    public void Update(TiaActor actor)
    {
        if (IsDone) return;

        IsDone = Time.time >= finishTime;
    }
}
