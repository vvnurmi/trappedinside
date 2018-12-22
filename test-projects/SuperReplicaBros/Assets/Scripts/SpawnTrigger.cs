using UnityEngine;

/// <summary>
/// Spawns objects when the player gets close enough.
/// <seealso cref="Spawner"/>
/// </summary>
public class SpawnTrigger : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        var spawner = collision.gameObject.GetComponent<Spawner>();
        if (spawner == null) return;

        spawner.TrySpawn();
    }
}
