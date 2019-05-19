using UnityEngine;

public class ScalingParallax : MonoBehaviour
{
    [Tooltip("How much to scale the object. Affects also parallax speed. Scale of 1.0 means do nothing.")]
    public float scale = 1.0f;

    [Tooltip("The camera for which to do the parallax scroll.")]
    public GameObject parallaxCamera;

    // Set about once, probably in Start().
    private Vector3 originalScale;

    // Modified during gameplay.
    private Vector3 oldCameraPosition;

    private void Awake()
    {
        oldCameraPosition = parallaxCamera.transform.position;
        originalScale = transform.localScale;
        transform.localScale = new Vector3(
            originalScale.x * scale,
            originalScale.y,
            originalScale.z);
    }

    private void LateUpdate()
    {
        var newCameraPosition = parallaxCamera.transform.position;
        var parallaxMoveX = -(newCameraPosition - oldCameraPosition).x * (scale - 1.0f);
        var totalMove = Vector3.right * parallaxMoveX;
        gameObject.transform.position += totalMove;
        oldCameraPosition = newCameraPosition;
    }
}
