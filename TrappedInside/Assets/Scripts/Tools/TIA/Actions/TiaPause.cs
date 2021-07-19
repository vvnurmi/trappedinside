using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Waits for given time until continuing to the next action.
/// </summary>
[System.Serializable]
public class TiaPause : ITiaAction
{
    private class Context
    {
        public float finishTime;
        public bool isDone;
    }

    [YamlMember(Alias = "Seconds")]
    [field: SerializeField]
    public float DurationSeconds { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone(ITiaActionContext context)
    {
        var (success, contextObject) = context.TryGet<Context>(this);
        return !success ? false
            : contextObject.isDone;
    }

    public void Start(ITiaActionContext context, GameObject actor)
    {
        context.Set(this, new Context
        {
            finishTime = Time.time + DurationSeconds,
        });
    }

    public void Update(ITiaActionContext context)
    {
        var contextObject = context.TryGet<Context>(this).contextObject;
        if (contextObject.isDone) return;

        contextObject.isDone = Time.time >= contextObject.finishTime;
    }

    public void Finish(ITiaActionContext context)
    {
    }
}
