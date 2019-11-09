using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class BossHornetWave
{
    public int numberOfHornets;
    public float attackVelocity;
    public float angularVelocity;
    public float numberOfCircles;
}

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
    public float circleRadius = 2.0f;
    public float firstHornetMovementStartTime = 5.0f;
    public float movementStartTimeDiffBetweenHornets = 0.5f;
    public BossHornetWave[] bossHornetWaves;

    private BossHornetState _state;
    private List<BossHornetData> _bossHornets = new List<BossHornetData>();
    private int _waveNumber = 0;

    void Start()
    {
        InitWave();
    }

    void FixedUpdate()
    {
        if (AllHornetsInCurrentWaveDead)
            InitNextWave();
        _state.Handle();
    }

    void InitNextWave()
    {
        if (_waveNumber + 1 < bossHornetWaves.Length)
        {
            _waveNumber++;
            InitWave();
        }
    }

    void InitWave()
    {
        Debug.Assert(bossHornetPrefab != null, "Boss Hornet Prefab is null");

        _state = new BossHornetStartWait(firstHornetMovementStartTime);
        _state.SetContext(this);

        _bossHornets.Clear();

        var position = transform.position + new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < bossHornetWaves[_waveNumber].numberOfHornets; i++)
        {
            _bossHornets.Add(
                new BossHornetData(
                    hornet: Instantiate(bossHornetPrefab, position, Quaternion.identity),
                    isFacingLeft: true,
                    flyingPosition: i));
        }

    }

    public IEnumerable<BossHornetData> ActiveBossHornets => _bossHornets.Where(h => h.Hornet != null);
    public bool AllHornetsInCurrentWaveDead => _bossHornets.All(h => h.Hornet == null);

    public void TransitionTo(BossHornetState state, string animationState)
    {
        _state = state;
        _state.SetContext(this);
        foreach(var bossHornet in ActiveBossHornets)
        {
            bossHornet.ReadyToTransition = false;
            bossHornet.AnimationState = animationState;
        }
    }

    public bool ReadyToStateTransition => ActiveBossHornets.All(bossHornet => bossHornet.ReadyToTransition);
    public float CurrentCircleAngle => bossHornetWaves[_waveNumber].numberOfCircles * 2.0f * Mathf.PI;
    public float CurrentAngularVelocity => bossHornetWaves[_waveNumber].angularVelocity;
    public float CurrentAttackVelocity => bossHornetWaves[_waveNumber].attackVelocity;

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
            _context.TransitionTo(new BossHornetFlyInCircle(0.0f), "IsFlying");
        }
    }
}

class BossHornetFlyInCircle : BossHornetState
{
    private readonly float _stateStartTime;
    private readonly float _startAngle;

    public BossHornetFlyInCircle(float startAngle)
    {
        _stateStartTime = Time.realtimeSinceStartup;
        _startAngle = startAngle;
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
            var angle = _startAngle + (Time.realtimeSinceStartup - movementStartTime) * _context.CurrentAngularVelocity;

            if (angle - _startAngle > _context.CurrentCircleAngle)
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
            _context.TransitionTo(new BossHornetFlyInCircle(CurrentCircleAngle), "IsFlying");
    }

    void UpdateHornetPosition(BossHornetData hornetData, float movementStartTime)
    {
        if (!_attackDirection.HasValue)
            _attackDirection = Mathf.Sign(_context.transform.position.x - hornetData.Hornet.transform.position.x);

        if (Time.realtimeSinceStartup > movementStartTime)
        {
            if (HornetReachedFinalPosition(hornetData))
            {
                hornetData.ReadyToTransition = true;
                return;
            }

            hornetData.Hornet.transform.Translate(new Vector3(_attackDirection.Value * _context.CurrentAttackVelocity * Time.deltaTime, 0, 0));
        }
    }

    float CurrentCircleAngle => _attackDirection > 0 ? 0.0f : Mathf.PI;

    bool HornetReachedFinalPosition(BossHornetData hornetData)
    {
        var hornetX = hornetData.Hornet.transform.position.x;
        if (_attackDirection > 0)
        {
            return hornetX > FinalX;
        }
        else
        {
            return hornetX < FinalX;
        }
    }

    float FinalX =>
        _attackDirection > 0
        ? _context.transform.position.x + _context.circleRadius
        : _context.transform.position.x - _context.circleRadius;
}

