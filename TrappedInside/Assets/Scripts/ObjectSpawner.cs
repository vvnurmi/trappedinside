using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Tooltip("Object spawn interval in seconds.")]
    public float spawnInterval = 2.0f;

    [Tooltip("The type of an object to be spawned.")]
    public GameObject objectType;

    [Tooltip("Offset from the center where objects are spawned.")]
    public Vector3 spawnOffset = Vector3.zero;

    private float previousSpawnTime = float.MaxValue;

    void Start()
    {
        Debug.Assert(objectType != null, "Object spawner requires a prefab object.");
    }

    private void FixedUpdate()
    {
        if (Time.time - previousSpawnTime > spawnInterval)
        {
            Instantiate(objectType, gameObject.transform.position + spawnOffset, Quaternion.identity);
            previousSpawnTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            previousSpawnTime = Time.time;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            previousSpawnTime = float.MaxValue;

    }
}
