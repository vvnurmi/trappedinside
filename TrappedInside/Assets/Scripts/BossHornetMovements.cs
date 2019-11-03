using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossHornetData
{
    private static readonly List<string> AnimationStates = new List<string> { "IsAttacking", "IsFlying" };
    private Animator _animator;

    public BossHornetData(GameObject hornet, bool isFacingLeft, int flyingPosition)
    {
        Hornet = hornet;
        _animator = hornet.GetComponentInChildren<Animator>();
        IsFacingLeft = isFacingLeft;
        ReadyToTransition = false;
        FlyingPosition = flyingPosition;
    }

    public string AnimationState 
    {
        set 
        {
            AnimationStates.ForEach(s => _animator.SetBool(s, false));
            _animator.SetBool(value, true);
        }
    }

    public GameObject Hornet { get; }
    public bool IsFacingLeft { get; set; }
    public bool ReadyToTransition { get; set; }
    public int FlyingPosition { get; }
}

public class BossHornetMovements : MonoBehaviour
{
    public GameObject bossHornetPrefab;
    public int numberOfHornets = 5;
    public float angularVelocity = 1.0f;
    public float linearVelocity = 2.0f;
    public float circleRadius = 2.0f;
    public float firstHornetMovementStartTime = 5.0f;
    public float movementStartTimeDiffBetweenHornets = 0.5f;
    public float circleAngle = Mathf.PI;

    private BossHornetState _state;
    private List<BossHornetData> _bossHornets =  new List<BossHornetData>();

    void Start()
    {
        Debug.Assert(bossHornetPrefab != null, "Boss Hornet Prefab is null");

        _state = new BossHornetStartWait(firstHornetMovementStartTime);
        _state.SetContext(this);

        var position = transform.position + new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < numberOfHornets; i++)
        {
            _bossHornets.Add(
                new BossHornetData(
                    hornet: Instantiate(bossHornetPrefab, position, Quaternion.identity), 
                    isFacingLeft: true,
                    flyingPosition: i));
        }
    }

    void FixedUpdate()
    {
        _state.Handle();
    }

    public IEnumerable<BossHornetData> ActiveBossHornets => _bossHornets.Where(h => h.Hornet != null);

    public void TransitionTo(BossHornetState state, string animationState)
    {
        _state = state;
        _state.SetContext(this);
        _bossHornets.ForEach(bossHornet => 
        {
            bossHornet.ReadyToTransition = false;
            bossHornet.AnimationState = animationState;
        } );
    }

    public bool ReadyToStateTransition => ActiveBossHornets.All(bossHornet => bossHornet.ReadyToTransition);

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

class BossHornetStartWait : BossHornetState
{
    private readonly float _waitTime;
    private readonly float _waitStartTime;

    public BossHornetStartWait(float waitTime)
    {
        _waitTime = waitTime;
        _waitStartTime = Time.realtimeSinceStartup;
    }

    public override void Handle()
    {
        if (Time.realtimeSinceStartup - _waitStartTime > _waitTime)
        {
            _context.TransitionTo(new BossHornetFlyInCircle(), "IsFlying");
        }
    }
}

class BossHornetFlyInCircle : BossHornetState
{
    private readonly float _stateStartTime;

    public BossHornetFlyInCircle()
    {
        _stateStartTime = Time.realtimeSinceStartup;
    }

    public override void Handle()
    {
        foreach (var bossHornet in _context.ActiveBossHornets)
        {
            UpdateHornetPosition(bossHornet, _stateStartTime + _context.movementStartTimeDiffBetweenHornets * bossHornet.FlyingPosition);
        }

        if (_context.ReadyToStateTransition)
            _context.TransitionTo(new BossHornetAttack(), "IsAttacking");
            
    }

    void UpdateHornetPosition(BossHornetData hornetData, float movementStartTime)
    {
        if (Time.realtimeSinceStartup > movementStartTime)
        {
            var transform = _context.transform;
            var angle = (Time.realtimeSinceStartup - movementStartTime) * _context.angularVelocity;

            if (angle > _context.circleAngle)
            {
                hornetData.ReadyToTransition = true;
                return;
            }

            var xCoordinate = transform.position.x + _context.circleRadius * Mathf.Cos(angle);
            var yCoordinate = transform.position.y + _context.circleRadius * Mathf.Sin(angle);

            hornetData.Hornet.transform.position = new Vector3(xCoordinate, yCoordinate, 0);

            if (xCoordinate < transform.position.x && hornetData.IsFacingLeft)
                FlipHornet(hornetData);
            else if (xCoordinate >= transform.position.x && !hornetData.IsFacingLeft)
                FlipHornet(hornetData);

        }
    }

    public void FlipHornet(BossHornetData hornetData)
    {
        hornetData.IsFacingLeft = !hornetData.IsFacingLeft;
        var originalScale = hornetData.Hornet.transform.localScale;
        hornetData.Hornet.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }
}

class BossHornetAttack : BossHornetState
{
    private readonly float _stateStartTime;
    private float? _attackDirection;

    public BossHornetAttack()
    {
        _stateStartTime = Time.realtimeSinceStartup;
    }

    public override void Handle()
    {
        foreach (var bossHornet in _context.ActiveBossHornets)
        {
            UpdateHornetPosition(bossHornet, _stateStartTime + _context.movementStartTimeDiffBetweenHornets * bossHornet.FlyingPosition);
        }

        if (_context.ReadyToStateTransition)
            _context.TransitionTo(new BossHornetFlyInCircle(), "IsFlying");
    }

    void UpdateHornetPosition(BossHornetData hornetData, float movementStartTime)
    {
        if (!_attackDirection.HasValue)
            _attackDirection = Mathf.Sign(_context.transform.position.x - hornetData.Hornet.transform.position.x);

        if (Time.realtimeSinceStartup > movementStartTime)
        {
            var transform = _context.transform;
            var finalX = transform.position.x + _context.circleRadius;

            if (hornetData.Hornet.transform.position.x > finalX * _attackDirection)
            {
                hornetData.ReadyToTransition = true;
                return;
            }

            hornetData.Hornet.transform.Translate(new Vector3(_attackDirection.Value * _context.linearVelocity * Time.deltaTime, 0, 0));
        }
    }

}

