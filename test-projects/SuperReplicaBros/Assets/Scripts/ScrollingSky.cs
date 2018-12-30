using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class ScrollingSky : MonoBehaviour
{
    [Tooltip("How fast the sky moves on its own.")]
    public float windSpeed = 1.0f;

    [Tooltip("How fast the sky moves against the camera's movement.")]
    public float parallaxSpeed = 0.1f;

    [Tooltip("The camera for which to do the parallax scroll.")]
    new public GameObject camera;

    private Vector3 oldCameraPosition;
    private GameObject gameObjectClone;

    private void Start()
    {
        CloneTilemap();
        oldCameraPosition = camera.transform.position;
    }

    private void Update()
    {
        var windMoveX = Time.deltaTime * windSpeed;
        var newCameraPosition = camera.transform.position;
        var parallaxMoveX = -(newCameraPosition - oldCameraPosition).x * parallaxSpeed;
        var totalMove = Vector3.right * (windMoveX + parallaxMoveX);
        gameObject.transform.position += totalMove;
        gameObjectClone.transform.position += totalMove;

        oldCameraPosition = newCameraPosition;
    }

    private void CloneTilemap()
    {
        var tilemap = gameObject.GetComponent<Tilemap>();
        tilemap.ResizeBounds();
        var bounds = tilemap.GetTightCellBounds();
        gameObjectClone = Instantiate(gameObject, transform.parent);
        // This script controls also the cloned object, so it doesn't need its copy of the script.
        Destroy(gameObjectClone.GetComponent<ScrollingSky>());
        gameObjectClone.transform.position += new Vector3(bounds.xMax - bounds.xMin, 0, 0);
    }
}
