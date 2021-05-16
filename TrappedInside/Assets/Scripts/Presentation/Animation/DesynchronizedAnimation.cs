using UnityEngine;

/// <summary>
/// Offsets animation phase and randomizes animation speed to take
/// multiple similar animation instances out of sync.
/// </summary>
[RequireComponent(typeof(Animator))]
public class DesynchronizedAnimation : MonoBehaviour
{
    [Tooltip("Minimum animation speed multiplier.")]
    public float speedMultiplierMin = 0.8f;

    [Tooltip("Maximum animation speed multiplier.")]
    public float speedMultiplierMax = 1.2f;

    public void Start()
    {
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            var playbackTime = Random.Range(0, stateInfo.length);
            animator.speed = Random.Range(0.8f, 1.2f);
            animator.Play(stateNameHash: 0, layer: -1, playbackTime);
        }
    }
}
