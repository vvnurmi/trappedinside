using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    private new Camera camera;
    private Player player;
    private float[] cameraBounds;

    private void Start()
    {
        camera = GetComponent<Camera>();
        player = FindObjectOfType<Player>();

        var cameraBoundRoot = GameObject.FindGameObjectWithTag("CameraBounds");
        var boundNum = cameraBoundRoot.transform.childCount;
        cameraBounds = new float[boundNum];
        for (int i = 0; i < boundNum; i++)
            cameraBounds[i] = cameraBoundRoot.transform.GetChild(i).transform.position.x;
    }

    private void LateUpdate()
    {
        FollowPlayer();
        ObeyBounds();
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
