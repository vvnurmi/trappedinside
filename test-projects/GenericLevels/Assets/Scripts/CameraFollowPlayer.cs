using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    private new Camera camera;
    private Player player;
    private float[] cameraBounds;
    private float originalCameraOrthographicSize;

    private void Start()
    {
        camera = GetComponent<Camera>();
        player = FindObjectOfType<Player>();
        originalCameraOrthographicSize = camera.orthographicSize;
        FindCameraBounds();
    }

    private void FindCameraBounds()
    {
        var cameraBoundObjects = GameObject.FindGameObjectsWithTag("CameraBounds");
        cameraBounds = new float[cameraBoundObjects.Length];
        {
            int i = 0;
            foreach (var obj in cameraBoundObjects)
                cameraBounds[i++] = obj.transform.position.x;
        }
    }

    private void LateUpdate()
    {
        // Follow player in late update so that the player will have moved already.
        // Otherwise the player won't appear stable on the screen.
        FollowPlayer();
        ObeyBounds();
        LimitViewSize();
    }

    /// <summary>
    /// Ensures that the viewport shows at most <see cref="originalCameraOrthographicSize"/> world units
    /// horizontally and vertically.
    /// </summary>
    private void LimitViewSize()
    {
        // Note: Unity itself only limits vertical size by camera.orthographicSize.
        // If the view is very low and wide, you would be able to see very far horizontally.
        if (camera.pixelHeight > camera.pixelWidth)
            camera.orthographicSize = originalCameraOrthographicSize;
        else
            camera.orthographicSize = originalCameraOrthographicSize * camera.pixelHeight / camera.pixelWidth;
    }

    private void FollowPlayer()
    {
        var cameraZ = camera.transform.position.z;
        var playerPos = player.transform.position;
        camera.transform.position = new Vector3(
            playerPos.x,
            playerPos.y,
            cameraZ);
    }

    private void ObeyBounds()
    {
        // Figure out extreme world coordinates of the camera.
        var vertExtent = camera.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;
        var cameraPos = camera.transform.position;
        var cameraLeft = cameraPos.x - horzExtent;
        var cameraRight = cameraPos.x + horzExtent;

        foreach (var boundX in cameraBounds)
        {
            // Limit camera from the right.
            if (cameraRight > boundX && cameraPos.x < boundX)
                cameraPos.x = boundX - horzExtent;

            // Limit camera from the left.
            if (cameraLeft < boundX && cameraPos.x >= boundX)
                cameraPos.x = boundX + horzExtent;
        }

        // Actuate new camera position.
        camera.transform.position = cameraPos;
    }
}
