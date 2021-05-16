using System;
using UnityEngine;

/// <summary>
/// Implements horizontal parallax scrolling by scaling objects horizontally
/// and moving them based on reference camera movement.
/// </summary>
public class ScalingParallax : MonoBehaviour
{
    [Serializable]
    public class ObjectScale
    {
        public GameObject obj;
        public float scale = 1.0f;
    }

    [Tooltip("The camera for which to do the parallax scroll.")]
    public GameObject parallaxCamera;

    [Tooltip("How much to scale the object. Affects also parallax speed. Scale of 1.0 means do nothing.")]
    public ObjectScale[] objectScales;

    // Modified during gameplay.
    private Vector3 oldCameraPosition;

    #region MonoBehaviour overrides

    private void Awake()
    {
        oldCameraPosition = parallaxCamera.transform.position;
        foreach (var x in objectScales)
            x.obj.transform.localScale = new Vector3(
                x.obj.transform.localScale.x * x.scale,
                x.obj.transform.localScale.y,
                x.obj.transform.localScale.z);
    }

    private void LateUpdate()
    {
        var newCameraPosition = parallaxCamera.transform.position;
        foreach (var x in objectScales)
            ParallaxScroll(x, oldCameraPosition, newCameraPosition);
        oldCameraPosition = newCameraPosition;
    }

    #endregion

    private static void ParallaxScroll(ObjectScale x, Vector3 oldCameraPosition, Vector3 newCameraPosition)
    {
        var parallaxMoveX = -(newCameraPosition - oldCameraPosition).x * (x.scale - 1.0f);
        var totalMove = Vector3.right * parallaxMoveX;
        x.obj.transform.position += totalMove;
    }
}
