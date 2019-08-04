using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float leftBound = 0;
    public float rightBound = 1000;
    public float lowerBound = -100;
    public float upperBound = 0;

    private GameObject player;
    private new Camera camera;
    private float cameraWidth;
    private float cameraHeight;

    private Vector3 BoundlessPosition => new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

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
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Assert(player != null);
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
