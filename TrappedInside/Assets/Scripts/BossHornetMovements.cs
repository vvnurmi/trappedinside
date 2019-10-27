using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHornetMovements : MonoBehaviour
{
    public GameObject bossHornetPrefab;
    public int numberOfHornets = 5;
    public float angularVelocity = 1.0f;
    public float circleRadius = 2.0f;

    private GameObject bossHornet;

    void Start()
    {
        Debug.Assert(bossHornetPrefab != null, "Boss Hornet Prefab is null");
        var position = transform.position + new Vector3(circleRadius, 0, 0);
        bossHornet = Instantiate(bossHornetPrefab, position, Quaternion.identity);
    }

    void FixedUpdate()
    {
        var xCoordinate = transform.position.x + circleRadius * Mathf.Cos(Time.realtimeSinceStartup * angularVelocity);
        var yCoordinate = transform.position.y + circleRadius * Mathf.Sin(Time.realtimeSinceStartup * angularVelocity);

        bossHornet.transform.position = new Vector3(xCoordinate, yCoordinate, 0);
    }
}
