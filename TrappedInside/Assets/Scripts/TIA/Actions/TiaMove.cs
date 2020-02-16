using UnityEngine;

/// <summary>
/// Moves actor along a curve over a given time
/// </summary>
public class TiaMove : ITiaAction
{
    public BezierCurve Curve { get; set; }
    public float DurationSeconds { get; set; }

    public bool IsDone { get; private set; }

    private float startTime;

    public void Start()
    {
        IsDone = false;
        startTime = Time.time;
    }

    public void Update(TiaActor actor)
    {
        RepositionOnCurve(actor.GameObject);

        IsDone = Time.time >= startTime + DurationSeconds;
    }

    private void RepositionOnCurve(GameObject obj)
    {
        float flightTime = Time.time - startTime;
        float curveParam = Mathf.InverseLerp(0, DurationSeconds, flightTime);
        var pathPosition = Curve.GetPointAt(curveParam);

        obj.transform.SetPositionAndRotation(pathPosition, obj.transform.rotation);
    }
}
