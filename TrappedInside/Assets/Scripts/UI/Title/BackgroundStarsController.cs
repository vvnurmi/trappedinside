using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls the creation of stars in the background of the title screen.
/// </summary>
public class BackgroundStarsController : MonoBehaviour
{
    [Tooltip("How close to each other the stars will be, approximately.")]
    public float density = 1.0f;

    [Tooltip("How long to wait at least before spawning another star.")]
    public float spawnDelaySecondsMin = 0.8f;

    [Tooltip("How long to wait at most before spawning another star.")]
    public float spawnDelaySecondsMax = 1.2f;

    [Tooltip("Templates for the actual instances to create.")]
    public GameObject[] prefabs;

    private void Start()
    {
        var positions = RandomizePositions().ToArray();
        positions.Shuffle();
        StartCoroutine(SpawnStars(positions));
    }

    private IEnumerable<Vector3> RandomizePositions()
    {
        // Randomization algorithm:
        // 1. Divide the screen into a grid of 'density' size.
        // 2. Randomize one star to each grid square.

        var camera = FindObjectOfType<Camera>();
        Debug.Assert(camera != null);
        var cameraExtents = new Vector2(
            camera.orthographicSize * camera.aspect,
            camera.orthographicSize);

        for (float gridY = -cameraExtents.y; gridY < cameraExtents.y; gridY += density)
            for (float gridX = -cameraExtents.x; gridX < cameraExtents.x; gridX += density)
            {
                var x = gridX + Random.Range(0, density);
                var y = gridY + Random.Range(0, density);
                if (x < cameraExtents.x && y < cameraExtents.y)
                    yield return new Vector3(x, y, 0);
            }
    }

    private IEnumerator SpawnStars(Vector3[] positions)
    {
        foreach (var pos in positions)
        {
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            var star = Instantiate(prefab, pos, Quaternion.identity, transform);

            // Randomize star animation phase and speed a bit.
            var animator = star.GetComponent<Animator>();
            if (animator != null)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                animator.playbackTime = Random.Range(0, stateInfo.length);
                animator.speed = Random.Range(0.8f, 1.2f);
            }

            var spawnInterval = Random.Range(spawnDelaySecondsMin, spawnDelaySecondsMax);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
