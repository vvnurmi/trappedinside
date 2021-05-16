using UnityEngine;

/// <summary>
/// A <see cref="ISequentialChild"/> that is done after a number of seconds.
/// </summary>
public class TimedSequentialChild : MonoBehaviour, ISequentialChild
{
    public float doneAfterSeconds = 10;

    private float startTime;

    public void Start() => startTime = Time.time;
    public bool IsDone() => Time.time >= startTime + doneAfterSeconds;
}
