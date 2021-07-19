using System;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Moves actor along a curve over a given time
/// </summary>
[Serializable]
public class TiaMove : ITiaAction
{
    private class Context
    {
        public GameObject actor;
        public BezierCurve curve;
        public bool isFlipped;
        public float startTime;
        public bool isDone;
    }

    [YamlMember(Alias = "Curve")]
    [field: SerializeField]
    public string CurveName { get; set; }

    [YamlMember(Alias = "Seconds")]
    [field: SerializeField]
    public float DurationSeconds { get; set; }

    /// <summary>
    /// If true, flip <see cref="SpriteRenderer"/> components when moving left.
    /// </summary>
    [field: SerializeField]
    public bool FlipLeft { get; set; }

    /// <summary>
    /// If true, then <see cref="TiaMove"/> will assume that in their unflipped
    /// state the <see cref="SpriteRenderer"/> components draw the sprite facing
    /// left (as goes the human interpretation). Makes sense only when
    /// <see cref="FlipLeft"/> is true.
    /// </summary>
    [field: SerializeField]
    public bool LooksLeftInitially { get; set; }

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
        Debug.Assert(actor != null);

        var contextObject = new Context();
        context.Set(this, contextObject);
        contextObject.actor = actor;
        contextObject.startTime = Time.time;

        var curveObject = context.TiaRoot.FindChildByName(CurveName);
        Debug.Assert(curveObject != null);
        if (curveObject != null)
        {
            contextObject.curve = curveObject.GetComponent<BezierCurve>();
            Debug.Assert(contextObject.curve != null);
        }
    }

    public void Update(ITiaActionContext context)
    {
        var contextObject = context.TryGet<Context>(this).contextObject;
        RepositionOnCurve(contextObject);
        contextObject.isDone = Time.time >= contextObject.startTime + DurationSeconds;
    }

    public void Finish(ITiaActionContext context)
    {
    }

    private void RepositionOnCurve(Context contextObject)
    {
        if (contextObject.actor == null || contextObject.curve == null) return;

        float flightTime = Time.time - contextObject.startTime;
        float curveParam = Mathf.InverseLerp(0, DurationSeconds, flightTime);
        var pathPosition = contextObject.curve.GetPointAt(curveParam);
        var oldPosition = contextObject.actor.transform.position;
        contextObject.actor.transform.SetPositionAndRotation(pathPosition, contextObject.actor.transform.rotation);

        if (FlipLeft)
        {
            var keepEitherFlipState = Mathf.Abs(oldPosition.x - pathPosition.x) < 1e-3f;
            var shouldBeFlipped = LooksLeftInitially != oldPosition.x > pathPosition.x;
            if (!keepEitherFlipState && contextObject.isFlipped != shouldBeFlipped)
            {
                var spriteRenderers = contextObject.actor.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in spriteRenderers)
                    renderer.flipX = !renderer.flipX;
                contextObject.isFlipped = shouldBeFlipped;
            }
        }
    }
}
