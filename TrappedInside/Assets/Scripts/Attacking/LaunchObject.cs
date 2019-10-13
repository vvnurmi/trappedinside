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
/// Launches <see cref="projectile"/> along <see cref="flightPath"/>.
/// Only one instance can be in flight at a time.
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

    private GameObject liveProjectile;
    private BezierCurve LiveFlightPath => liveFlightPath.Value;
    private bool IsLaunchingRight => launcher.state.collisions.faceDir == 1;

    #region MonoBehaviour overrides

    private void Awake()
    {
        liveProjectile = Instantiate(projectile);
        liveProjectile.SetActive(false);
        liveFlightPath = new Lazy<BezierCurve>(() => Instantiate(flightPath));
    }

    #endregion

    public void OnAttack()
    {
        // Can't launch the projectile if it's flying already.
        if (liveProjectile.activeSelf) return;

        LiveFlightPath.transform.SetPositionAndRotation(transform.position, transform.rotation);

        // Mirror flight path X scale if launching to the left.
        var scaleXSign = IsLaunchingRight ? 1 : -1;
        var flightPathScale = LiveFlightPath.transform.localScale;
        flightPathScale.x = scaleXSign * Math.Abs(flightPathScale.x);
        LiveFlightPath.transform.localScale = flightPathScale;

        liveProjectile.SetActive(true);
        var launchable = liveProjectile.GetComponent<ILaunchable>();
        launchable.SetFlightPath(LiveFlightPath);
    }
}
