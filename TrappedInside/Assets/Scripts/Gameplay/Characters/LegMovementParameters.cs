using UnityEngine;

/// <summary>
/// Parameters for movement of a character that walks on legs.
/// </summary>
[CreateAssetMenu(fileName = "LegMovementParameters", menuName = "Character/LegMovementParameters")]
public class LegMovementParameters : ScriptableObject
{
    [Tooltip("Maximum walking speed.")]
    public float maxSpeed = 5;

    [Tooltip("Seconds it takes to reach maximum speed from standstill.")]
    public float maxSpeedReachTime = 1;

    [Tooltip("Seconds it takes to stop to standstill from maximum speed.")]
    public float maxSpeedStopTime = 0.5f;

    [Tooltip("Height of the lowest possible jump.")]
    public float jumpHeightMin = 2;

    [Tooltip("Height of the highest possible jump.")]
    public float jumpHeightMax = 4;

    [Tooltip("Seconds it takes for a jump to reach its apex.")]
    public float jumpApexTime = 1;
}
