using System;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Moves actor along a curve over a given time
/// </summary>
public class TiaMove : ITiaAction
{
    [YamlMember(Alias = "Curve")]
    public string CurveName { get; set; }

    [YamlMember(Alias = "Seconds")]
    public float DurationSeconds { get; set; }

    public bool IsDone { get; private set; }

    public BezierCurve Curve =>
        curve ?? throw new InvalidOperationException($"{nameof(TiaMove)} has no curve, maybe {nameof(Start)} wasn't called?");

    private BezierCurve curve;

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

        obj.transform.SetPositionAndRotation(pathPosition, obj.transform.rotation);
    }
}
