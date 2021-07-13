using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Waits for given time until continuing to the next action.
/// </summary>
[System.Serializable]
public class TiaPause : ITiaAction
{
    [YamlMember(Alias = "Seconds")]
    [field: SerializeField]
    public float DurationSeconds { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

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
