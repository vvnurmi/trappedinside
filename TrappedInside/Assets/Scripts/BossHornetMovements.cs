using System.Collections.Generic;
using UnityEngine;

public class BossHornetData
{
    public GameObject hornet;
    public bool isFacingLeft = true;
}

public class BossHornetMovements : MonoBehaviour
{
    public GameObject bossHornetPrefab;
    public int numberOfHornets = 5;
    public float angularVelocity = 1.0f;
    public float circleRadius = 2.0f;
    public float firstHornetMovementStartTime = 5.0f;
    public float movementStartTimeDiffBetweenHornets = 0.5f;

    private List<BossHornetData> bossHornets =  new List<BossHornetData>();

    void Start()
    {
        Debug.Assert(bossHornetPrefab != null, "Boss Hornet Prefab is null");
        var position = transform.position + new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < numberOfHornets; i++)
        {
            bossHornets.Add(new BossHornetData
            {
                hornet = Instantiate(bossHornetPrefab, position, Quaternion.identity),
                isFacingLeft = true
            });
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < bossHornets.Count; i++)
        {
            UpdateHornetPosition(bossHornets[i], firstHornetMovementStartTime + movementStartTimeDiffBetweenHornets * i);
        }
    }

    void UpdateHornetPosition(BossHornetData hornetData, float movementStartTime)
    {
        if (hornetData.hornet == null)
            return;

        if (Time.timeSinceLevelLoad > movementStartTime)
        {
            var angle = (Time.timeSinceLevelLoad - movementStartTime) * angularVelocity;
            var xCoordinate = transform.position.x + circleRadius * Mathf.Cos(angle);
            var yCoordinate = transform.position.y + circleRadius * Mathf.Sin(angle);

            hornetData.hornet.transform.position = new Vector3(xCoordinate, yCoordinate, 0);

            if (xCoordinate < transform.position.x && hornetData.isFacingLeft)
                FlipHornet(hornetData);
            else if (xCoordinate >= transform.position.x && !hornetData.isFacingLeft)
                FlipHornet(hornetData);
        }
    }

    public void FlipHornet(BossHornetData hornetData)
    {
        hornetData.isFacingLeft = !hornetData.isFacingLeft;
        var originalScale = hornetData.hornet.transform.localScale;
        hornetData.hornet.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }

}
