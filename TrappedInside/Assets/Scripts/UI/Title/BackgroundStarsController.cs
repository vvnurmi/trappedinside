using UnityEngine;

public class BackgroundStarsController : MonoBehaviour
{
    [Tooltip("How close to each other the stars will be, approximately.")]
    public float density = 1.0f;

    [Tooltip("Templates for the actual instances to create")]
    public GameObject[] prefabs;

    private void Start()
    {
        var camera = FindObjectOfType<Camera>();
        Debug.Assert(camera != null);
        var cameraExtents = new Vector2(
            camera.orthographicSize * camera.aspect,
            camera.orthographicSize);

        // Divide the screen into a grid of 'density' size.
        // Randomize one star to each grid square.
        for (float gridY = -cameraExtents.y; gridY < cameraExtents.y; gridY += density)
            for (float gridX = -cameraExtents.x; gridX < cameraExtents.x; gridX += density)
            {
                var prefab = prefabs[Random.Range(0, prefabs.Length)];
                var pos = new Vector3(
                    gridX + Random.Range(0, density),
                    gridY + Random.Range(0, density),
                    0);
                Instantiate(prefab, pos, Quaternion.identity, transform);
            }
    }
}
