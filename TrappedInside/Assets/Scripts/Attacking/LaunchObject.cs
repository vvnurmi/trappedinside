using System;
using UnityEngine;

public interface ILaunchable
{
    /// <summary>
    /// Follow a flight path.
    /// </summary>
    void SetFlightPath(BezierCurve path);
}

/// <summary>
/// Launches a clone of <see cref="projectile"/> along <see cref="flightPath"/>.
/// </summary>
public class LaunchObject : MonoBehaviour, IAttack
{
    [Tooltip("Object to clone and launch.")]
    public GameObject projectile;

    [Tooltip("Flight path to follow.")]
    public BezierCurve flightPath;

    [Tooltip("Who is launching the object.")]
    public CharacterController2D launcher;

    /// <summary>
    /// Copy of the flight path in the game world.
    /// </summary>
    private Lazy<BezierCurve> liveFlightPath;

    private BezierCurve LiveFlightPath => liveFlightPath.Value;
    private bool IsLaunchingRight => launcher.state.collisions.faceDir == 1;

    #region MonoBehaviour overrides

    private void Awake()
    {
        liveFlightPath = new Lazy<BezierCurve>(() => Instantiate(flightPath));
    }

    #endregion

    public void OnAttack()
    {
        LiveFlightPath.transform.SetPositionAndRotation(transform.position, transform.rotation);

        // Mirror flight path X scale if launching to the left.
        var scaleXSign = IsLaunchingRight ? 1 : -1;
        var flightPathScale = LiveFlightPath.transform.localScale;
        flightPathScale.x = scaleXSign * Math.Abs(flightPathScale.x);
        LiveFlightPath.transform.localScale = flightPathScale;

        var projectileClone = Instantiate(projectile);
        var launchable = projectileClone.GetComponent<ILaunchable>();
        launchable.SetFlightPath(LiveFlightPath);
    }
}
