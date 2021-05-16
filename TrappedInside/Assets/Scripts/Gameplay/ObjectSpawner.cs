using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Tooltip("Object spawn interval in seconds.")]
    public float spawnInterval = 2.0f;

    [Tooltip("The type of an object to be spawned.")]
    public GameObject objectType;

    [Tooltip("The maximum number of objects that is allowed simultaneously.")]
    public int maxObjectCount = int.MaxValue;

    [Tooltip("Offset from the center where objects are spawned.")]
    public Vector3 spawnOffset = Vector3.zero;

    private float previousSpawnTime = float.MaxValue;
    private List<GameObject> activeGameObjects = new List<GameObject>();

    void Start()
    {
        Debug.Assert(objectType != null, "Object spawner requires a prefab object.");
    }

    private void FixedUpdate()
    {
        RemoveDeletedGameObjects();
        if (activeGameObjects.Count < maxObjectCount && Time.time - previousSpawnTime > spawnInterval)
        {
            activeGameObjects.Add(Instantiate(objectType, gameObject.transform.position + spawnOffset, Quaternion.identity));
            previousSpawnTime = Time.time;
        }
    }

    private void RemoveDeletedGameObjects()
    {
        for (int i = 0; i < activeGameObjects.Count; i++)
        {
            if (activeGameObjects[i] == null)
                activeGameObjects.RemoveAt(i);
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
