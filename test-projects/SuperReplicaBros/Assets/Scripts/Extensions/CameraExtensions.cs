using UnityEngine;

public static class CameraExtensions
{
    public static Vector2 GetWorldSize(this Camera camera)
    {
        var cameraWorldMin = camera.ViewportToWorldPoint(Vector3.zero);
        var cameraWorldMax = camera.ViewportToWorldPoint(Vector3.one);
        var cameraWorldSize = cameraWorldMax - cameraWorldMin;
        return new Vector2(cameraWorldSize.x, cameraWorldSize.y);
    }
}
