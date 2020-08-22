using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public bool isFixed = false;
    public bool verticalFix = true;

    private GameObject player;
    private Camera _camera;

    private Vector3 Position => new Vector3(player.transform.position.x, CameraY, transform.position.z);

    private float CameraY => verticalFix ? transform.position.y : player.transform.position.y;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if(!isFixed)
            _camera.transform.position = Position;
    }
}
