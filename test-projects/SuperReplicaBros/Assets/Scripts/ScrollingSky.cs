using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class ScrollingSky : MonoBehaviour
{
    [Tooltip("How fast the sky moves on its own.")]
    public float windSpeed = 1.0f;

    private GameObject gameObjectClone;

    private void Start()
    {
        CloneTilemap();
    }

    private void Update()
    {
        var windMoveX = Time.deltaTime * windSpeed;
        var totalMove = Vector3.right * windMoveX;
        gameObject.transform.position += totalMove;
        gameObjectClone.transform.position += totalMove;
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
