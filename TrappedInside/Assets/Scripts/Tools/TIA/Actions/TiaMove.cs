using System;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Moves actor along a curve over a given time
/// </summary>
[Serializable]
public class TiaMove : ITiaAction
{
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

    public bool IsDone { get; private set; }

    public BezierCurve Curve =>
        curve ?? throw new InvalidOperationException($"{nameof(TiaMove)} has no curve, maybe {nameof(Start)} wasn't called?");

    private BezierCurve curve;
    private bool isFlipped;
    private float startTime;

    public void Start(ITiaActionContext context)
    {
        IsDone = false;
        startTime = Time.time;

        var curveObject = context.TiaRoot.FindChildByName(CurveName);
        Debug.Assert(curveObject != null);
        if (curveObject != null)
        {
            curve = curveObject.GetComponent<BezierCurve>();
            Debug.Assert(curve != null);
        }
    }

    public void Update(ITiaActionContext context)
    {
        RepositionOnCurve(context.Actor.GameObject);

        IsDone = Time.time >= startTime + DurationSeconds;
    }

    public void Finish(ITiaActionContext context)
    {
    }

    private void RepositionOnCurve(GameObject obj)
    {
        float flightTime = Time.time - startTime;
        float curveParam = Mathf.InverseLerp(0, DurationSeconds, flightTime);
        var pathPosition = Curve.GetPointAt(curveParam);
        var oldPosition = obj.transform.position;

        obj.transform.SetPositionAndRotation(pathPosition, obj.transform.rotation);

        if (FlipLeft)
        {
            var keepEitherFlipState = Mathf.Abs(oldPosition.x - pathPosition.x) < 1e-3f;
            var shouldBeFlipped = LooksLeftInitially != oldPosition.x > pathPosition.x;
            if (!keepEitherFlipState && isFlipped != shouldBeFlipped)
            {
                var spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in spriteRenderers)
                    renderer.flipX = !renderer.flipX;
                isFlipped = shouldBeFlipped;
            }
        }
    }
}
