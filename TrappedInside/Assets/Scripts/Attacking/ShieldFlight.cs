using UnityEngine;

/// <summary>
/// Moves a thrown shield like a boomerang.
/// </summary>
public class ShieldFlight : MonoBehaviour, ILaunchable
{
    [Tooltip("Flight path to follow.")]
    public BezierCurve flightPath;

    [Tooltip("Time of flight in seconds.")]
    public float flightSeconds;

    // Initialized on wakeup.
    private float flightStart;

    public void SetFlightPath(BezierCurve path)
    {
        flightPath = path;
    }

    #region MonoBehaviour overrides

    private void Awake()
    {
        flightStart = Time.time;

        // Disown the shield object so that it will fly independently
        // of Mike's movement.
        transform.parent = null;
    }

    private void Update()
    {
        float flightTime = Time.time - flightStart;
        float curveParam = flightTime / flightSeconds;
        var position = flightPath.GetPointAt(curveParam);
        transform.SetPositionAndRotation(position, transform.rotation);
    }

    #endregion
}
