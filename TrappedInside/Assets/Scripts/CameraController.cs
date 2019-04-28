using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject mike;

    [Tooltip("Camera area limits in world coordinates.")]
    public Rect cameraAreaLimits;

    [Tooltip("Player moving area in viewport coordinates.")]
    public Rect playerMovingArea;

    private new Camera camera;

    private Vector3 offset;

    private Vector3 BoundlessPosition => mike.transform.position + offset;

    //private Vector3 BoundedPosition => new Vector3(
    //    x: GetBoundedValue(BoundlessPosition.x, cameraAreaLimits.xMin, cameraAreaLimits.xMax),
    //    y: GetBoundedValue(BoundlessPosition.y, cameraAreaLimits.yMin, cameraAreaLimits.yMax),
    //    z: -1);

    private float GetBoundedValue(float value, float min, float max)
    {
        return value < min ? min : Mathf.Min(value, max);
    }

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - mike.transform.position;
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = mike.transform.position + offset;
        //var viewPortLocation = camera.WorldToViewportPoint(mike.transform.position);
        //if (playerMovingArea.Contains(viewPortLocation))
        //{
        //    return;
        //}

        //var viewportOffsetInWorldCoordinates = 
        //    camera.ViewportToWorldPoint(new Vector3(playerMovingArea.width / 2.0f, playerMovingArea.height / 2.0f, 0));

        //transform.position = BoundedPosition - new Vector3(viewportOffsetInWorldCoordinates.x, 0);
    }


}
