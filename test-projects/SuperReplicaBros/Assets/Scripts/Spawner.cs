using UnityEngine;

/// <summary>
/// Spawns objects when the player gets close enough.
/// <seealso cref="SpawnTrigger"/>
/// </summary>
public class Spawner : MonoBehaviour
{
    [Tooltip("Object to spawn.")]
    public GameObject spawn;

    [Tooltip("How many objects to spawn.")]
    public int count = 1;

    [Tooltip("Seconds to wait before another spawn.")]
    public float spawnDelay = 1.0f;

    private float lastSpawnTime;

    /// <summary>
    /// Perhaps spawns another object. Returns true on success.
    /// </summary>
    public bool TrySpawn()
    {
        if (count <= 0) return false;
        if (lastSpawnTime + spawnDelay > Time.fixedTime) return false;

        count--;
        lastSpawnTime = Time.fixedTime;
        Instantiate(spawn, transform.position, transform.rotation);
        return true;
    }
}
