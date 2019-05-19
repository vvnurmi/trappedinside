using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject mike;
    private new Camera camera;

    public float leftBound = 0;
    public float rightBound = 1000;
    public float lowerBound = -100;
    public float upperBound = 0;

    private float cameraWidth;
    private float cameraHeight;

    private Vector3 BoundlessPosition => new Vector3(mike.transform.position.x, mike.transform.position.y, transform.position.z);

    private Vector3 BoundedPosition => new Vector3(
        x: GetBoundedValue(BoundlessPosition.x, leftBound + cameraWidth, rightBound - cameraWidth),
        y: GetBoundedValue(BoundlessPosition.y, lowerBound + cameraHeight, upperBound - cameraHeight),
        z: -1);

    private float GetBoundedValue(float value, float min, float max)
    {
        return value < min ? min : Mathf.Min(value, max);
    }

    void Start()
    {
        camera = GetComponent<Camera>();
        var bottomLeftCorner = camera.ViewportToWorldPoint(new Vector3(0f, 0f));
        cameraWidth = transform.position.x - bottomLeftCorner.x;
        cameraHeight = transform.position.y - bottomLeftCorner.y;
    }

    void LateUpdate()
    {
        transform.position = BoundedPosition;
    }
}
