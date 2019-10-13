using UnityEngine;

/// <summary>
/// Moves a thrown shield like a boomerang.
/// </summary>
public class ShieldFlight : MonoBehaviour, ILaunchable
{
    [Tooltip("Flight path to follow.")]
    public BezierCurve flightPath;

    [Tooltip("Time of flight in seconds.")]
    public float flightSeconds = 2;

    [Tooltip("How many seconds of the end of the flight is reserved for returning home.")]
    public float homingSeconds = 1;

    // Modified at run-time.
    private float flightStart;
    private GameObject homingTarget;

    public void SetFlightPath(BezierCurve path, GameObject home)
    {
        flightPath = path;
        flightStart = Time.time;
        homingTarget = home;
    }

    #region MonoBehaviour overrides

    private void Awake()
    {
        // Disown the shield object so that it will fly independently
        // of Mike's movement.
        transform.parent = null;
    }

    private void Update()
    {
        float flightTime = Time.time - flightStart;
        float curveParam = flightTime / flightSeconds;
        float homingTime = flightTime - (flightSeconds - homingSeconds);
        float homingFactor = Mathf.Clamp(homingTime / homingSeconds, 0, 1);

        var pathPosition = flightPath.GetPointAt(curveParam);
        var homePosition = homingTarget.transform.position;
        var position = Vector3.Lerp(pathPosition, homePosition, homingFactor);

        transform.SetPositionAndRotation(position, transform.rotation);
    }

    #endregion
}
