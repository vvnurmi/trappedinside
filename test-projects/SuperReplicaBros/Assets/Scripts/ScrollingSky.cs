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
    private Tilemap tilemap;
    private BoundsInt tilemapBounds;
    private Camera cameraComponent;

    private void Start()
    {
        CloneTilemap();
        oldCameraPosition = camera.transform.position;
        cameraComponent = camera.GetComponent<Camera>();
    }

    private void Update()
    {
        ScrollTilemap();
        EnsureTilemapsOnScreen();
    }

    private void CloneTilemap()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
        tilemapBounds = tilemap.GetTightCellBounds();
        gameObjectClone = Instantiate(gameObject, transform.parent);
        // This script controls also the cloned object, so it doesn't need its copy of the script.
        Destroy(gameObjectClone.GetComponent<ScrollingSky>());
        gameObjectClone.transform.position += new Vector3(tilemapBounds.xMax - tilemapBounds.xMin, 0, 0);
    }

    private void ScrollTilemap()
    {
        var windMoveX = Time.deltaTime * windSpeed;
        var newCameraPosition = camera.transform.position;
        var parallaxMoveX = -(newCameraPosition - oldCameraPosition).x * parallaxSpeed;
        var totalMove = Vector3.right * (windMoveX + parallaxMoveX);
        gameObject.transform.position += totalMove;
        gameObjectClone.transform.position += totalMove;

        oldCameraPosition = newCameraPosition;
    }

    private void EnsureTilemapsOnScreen()
    {
        // Note: 'tilemap' belongs to 'gameObject' which is always drawn left of 'gameObjectClone'.
        var cameraArea = cameraComponent.GetWorldArea();
        var tilemapMin = tilemap.CellToWorld(tilemapBounds.min);
        var tilemapMax = tilemap.CellToWorld(tilemapBounds.max);
        var moveDirection =
            cameraArea.xMin < tilemapMin.x ? -1.0f :
            tilemapMax.x < cameraArea.xMin ? +1.0f :
            0.0f;
        var move = moveDirection * Vector3.right * (tilemapMax.x - tilemapMin.x);
        gameObject.transform.position += move;
        gameObjectClone.transform.position += move;
    }
}
