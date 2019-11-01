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

    private BossHornetState _state;
    private List<BossHornetData> _bossHornets =  new List<BossHornetData>();

    public List<BossHornetData> GetBossHornets() => _bossHornets;

    void Start()
    {
        Debug.Assert(bossHornetPrefab != null, "Boss Hornet Prefab is null");

        _state = new StartWait(firstHornetMovementStartTime);
        _state.SetContext(this);

        var position = transform.position + new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < numberOfHornets; i++)
        {
            _bossHornets.Add(new BossHornetData
            {
                hornet = Instantiate(bossHornetPrefab, position, Quaternion.identity),
                isFacingLeft = true
            });
        }
    }

    void FixedUpdate()
    {
        _state.Handle();
    }

    public void TransitionTo(BossHornetState state)
    {
        _state = state;
        _state.SetContext(this);
    }

}

public abstract class BossHornetState
{
    protected BossHornetMovements _context;

    public void SetContext(BossHornetMovements context)
    {
        _context = context;
    }

    public abstract void Handle();
}

class StartWait : BossHornetState
{
    private readonly float _waitTime;
    private readonly float _waitStartTime;

    public StartWait(float waitTime)
    {
        _waitTime = waitTime;
        _waitStartTime = Time.realtimeSinceStartup;
    }

    public override void Handle()
    {
        if (Time.realtimeSinceStartup - _waitStartTime > _waitTime)
        {
            _context.TransitionTo(new FlyInCircle());
        }
    }
}

class FlyInCircle : BossHornetState
{
    public override void Handle()
    {
        var bossHornets = _context.GetBossHornets();
        for (int i = 0; i < bossHornets.Count; i++)
        {
            UpdateHornetPosition(bossHornets[i], _context.firstHornetMovementStartTime + _context.movementStartTimeDiffBetweenHornets * i);
        }
    }

    void UpdateHornetPosition(BossHornetData hornetData, float movementStartTime)
    {
        if (hornetData.hornet == null)
            return;

        if (Time.timeSinceLevelLoad > movementStartTime)
        {
            var transform = _context.transform;
            var angle = (Time.timeSinceLevelLoad - movementStartTime) * _context.angularVelocity;
            var xCoordinate = transform.position.x + _context.circleRadius * Mathf.Cos(angle);
            var yCoordinate = transform.position.y + _context.circleRadius * Mathf.Sin(angle);

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


