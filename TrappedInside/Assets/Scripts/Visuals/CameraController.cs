using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float leftBound = 0;
    public float rightBound = 1000;
    public float lowerBound = -100;
    public float upperBound = 0;

    public bool isFixed = false;
    public bool verticalFix = true;

    private GameObject player;
    private Camera cameraComponent;
    private float cameraWidth;
    private float cameraHeight;

    private Vector3 BoundedPosition => new Vector3(
        x: GetBoundedValue(BoundlessPosition.x, leftBound + cameraWidth, rightBound - cameraWidth),
        y: GetBoundedValue(BoundlessPosition.y, lowerBound + cameraHeight, upperBound - cameraHeight),
        z: -1);

    private Vector3 BoundlessPosition => new Vector3(player.transform.position.x, CameraY, transform.position.z);

    private float CameraY => verticalFix ? transform.position.y : player.transform.position.y;

    private float GetBoundedValue(float value, float min, float max) =>
        value < min ? min : Mathf.Min(value, max);

    void Start()
    {
        if (!isFixed)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Debug.Assert(player != null);
            cameraComponent = GetComponent<Camera>();
            var bottomLeftCorner = cameraComponent.ViewportToWorldPoint(new Vector3(0f, 0f));
            cameraWidth = transform.position.x - bottomLeftCorner.x;
            cameraHeight = transform.position.y - bottomLeftCorner.y;
        }
    }

    void LateUpdate()
    {
        if(!isFixed)
            transform.position = BoundedPosition;
    }
}
