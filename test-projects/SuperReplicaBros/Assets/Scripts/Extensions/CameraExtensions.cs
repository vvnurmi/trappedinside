using UnityEngine;

public static class CameraExtensions
{
    public static Rect GetWorldArea(this Camera camera)
    {
        var cameraWorldMin = camera.ViewportToWorldPoint(Vector3.zero);
        var cameraWorldMax = camera.ViewportToWorldPoint(Vector3.one);
        var cameraWorldSize = cameraWorldMax - cameraWorldMin;
        return new Rect(cameraWorldMin, cameraWorldSize);
    }
}
