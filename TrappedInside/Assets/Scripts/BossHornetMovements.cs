using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AttackType
{
    FlyInCircle,
    FlyInDividedCircle
}

[System.Serializable]
public class BossHornetWave
{
    public int numberOfHornets;
    public float attackVelocity;
    public float angularVelocity;
    public float numberOfCircles;
    public AttackType attackType;
}

public interface IBossHornet
{
    string AnimationState { set; }
    Vector3 Position { get; set; }
    bool IsActive { get; }
    bool IsFacingLeft { get; set; }
    bool ReadyToTransition { get; set; }
    int FlyingPosition { get; }
    void TranslatePosition(Vector3 update);
    void Flip();
}

public class BossHornet : IBossHornet
{
    private static readonly List<string> AnimationStates = new List<string> { "IsAttacking", "IsFlying" };
    private GameObject _hornet;
    private Animator _animator;

    public BossHornet(GameObject hornet, bool isFacingLeft, int flyingPosition)
    {
        _hornet = hornet;
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

    public Vector3 Position
    {
        get
        {
            return _hornet.transform.position;
        }
        set
        {
            _hornet.transform.position = value;
        }
    }

    public bool IsActive => _hornet != null;

    public bool IsFacingLeft { get; set; }
    public bool ReadyToTransition { get; set; }
    public int FlyingPosition { get; }
    public void TranslatePosition(Vector3 update)
    {
        _hornet.transform.Translate(update);
    }

    public void Flip()
    {
        IsFacingLeft = !IsFacingLeft;
        var originalScale = _hornet.transform.localScale;
        _hornet.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }
}

public class BossHornetMovements : MonoBehaviour, IBossHornetMovements
{
    public GameObject bossHornetPrefab;
    public float circleRadius = 2.0f;
    public float firstHornetMovementStartTime = 5.0f;
    public float movementStartTimeDiffBetweenHornets = 0.5f;
    public BossHornetWave[] bossHornetWaves;

    private BossHornetState _state;
    private List<IBossHornet> _bossHornets = new List<IBossHornet>();
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

        _state = new BossHornetStartWait(new TimeData(), firstHornetMovementStartTime, bossHornetWaves[_waveNumber].attackType);
        _state.SetContext(this);

        _bossHornets.Clear();

        var position = transform.position + new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < bossHornetWaves[_waveNumber].numberOfHornets; i++)
        {
            _bossHornets.Add(
                new BossHornet(
                    hornet: Instantiate(bossHornetPrefab, position, Quaternion.identity),
                    isFacingLeft: true,
                    flyingPosition: i));
        }

    }

    public IEnumerable<IBossHornet> ActiveBossHornets => _bossHornets.Where(h => h.IsActive);
    public bool AllHornetsInCurrentWaveDead => _bossHornets.All(h => !h.IsActive);

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
    public float MovementStartTimeDiffBetweenHornets => movementStartTimeDiffBetweenHornets;
    public float CircleRadius => circleRadius;
    public Vector3 Position => transform.position;
}

public interface IBossHornetMovements
{
    IEnumerable<IBossHornet> ActiveBossHornets { get; }
    bool AllHornetsInCurrentWaveDead { get; }
    void TransitionTo(BossHornetState state, string animationState);
    bool ReadyToStateTransition { get; }
    float CurrentCircleAngle { get; }
    float CurrentAngularVelocity { get; }
    float CurrentAttackVelocity { get; }
    float MovementStartTimeDiffBetweenHornets { get; }
    float CircleRadius { get; }
    Vector3 Position { get; }
}

public interface ITime
{
    float RealtimeSinceStartup { get; }
    float DeltaTime { get; }
}

public class TimeData : ITime
{
    public float RealtimeSinceStartup => Time.realtimeSinceStartup;
    public float DeltaTime => Time.deltaTime;
}

public abstract class BossHornetState
{
    protected IBossHornetMovements _context;
    protected ITime _time;

    public BossHornetState(ITime time)
    {
        _time = time;
    }

    public void SetContext(IBossHornetMovements context)
    {
        _context = context;
    }

    public abstract void Handle();
}

public class BossHornetStartWait : BossHornetState
{
    private readonly float _waitTime;
    private readonly float _waitStartTime;
    private readonly AttackType _attackType;

    public BossHornetStartWait(ITime time, float waitTime, AttackType attackType) : base(time)
    {
        _waitTime = waitTime;
        _waitStartTime = time.RealtimeSinceStartup;
        _attackType = attackType;
    }

    public override void Handle()
    {
        if (_time.RealtimeSinceStartup - _waitStartTime > _waitTime)
        {
            if (_attackType == AttackType.FlyInCircle)
                _context.TransitionTo(new BossHornetFlyInCircle(_time, new CirclePositionUpdater(), 0), "IsFlying");
            else
                _context.TransitionTo(new BossHornetFlyInCircle(_time, new DividedCirclePositionUpdater(), 0), "IsFlying");

        }
    }
}

public class BossHornetFlyInCircle : BossHornetState
{
    private readonly float _stateStartTime;
    private readonly IHornetPositionUpdater _hornetPositionUpdater;
    private readonly float _startAngle;

    public BossHornetFlyInCircle(ITime time, IHornetPositionUpdater hornetPositionUpdater, float startAngle) : base(time)
    {
        _stateStartTime = time.RealtimeSinceStartup;
        _hornetPositionUpdater = hornetPositionUpdater;
        _startAngle = startAngle;

    }

    public override void Handle()
    {
        foreach (var bossHornet in _context.ActiveBossHornets)
        {
            UpdateHornetPosition(bossHornet);
        }

        if (_context.ReadyToStateTransition)
            _context.TransitionTo(new BossHornetAttack(_time, AttackType.FlyInCircle), "IsAttacking");
            
    }

    public void UpdateHornetPosition(IBossHornet bossHornet)
    {
        if (_time.RealtimeSinceStartup > MovementStartTime(bossHornet))
        {
            var angle = _startAngle + (_time.RealtimeSinceStartup - MovementStartTime(bossHornet)) * _context.CurrentAngularVelocity;

            if (angle - _startAngle > _context.CurrentCircleAngle)
            {
                bossHornet.ReadyToTransition = true;
                return;
            }

            bossHornet.Position = _hornetPositionUpdater.CalculateNewPosition(_context, bossHornet, angle);

            if (FlipRequired(bossHornet))
                bossHornet.Flip();
        }
    }

    public float MovementStartTime(IBossHornet bossHornet) =>
        _stateStartTime + _context.MovementStartTimeDiffBetweenHornets * bossHornet.FlyingPosition;

    public bool FlipRequired(IBossHornet bossHornet) =>
        bossHornet.Position.x < _context.Position.x && bossHornet.IsFacingLeft ||
        bossHornet.Position.x >= _context.Position.x && !bossHornet.IsFacingLeft;
}


public interface IHornetPositionUpdater
{
    Vector3 CalculateNewPosition(IBossHornetMovements bossHornetMovements, IBossHornet bossHornet, float angle);
}

public class DividedCirclePositionUpdater : IHornetPositionUpdater
{
    public Vector3 CalculateNewPosition(IBossHornetMovements bossHornetMovements, IBossHornet bossHornet, float angle) => 
        new Vector3(
            bossHornetMovements.Position.x + bossHornetMovements.CircleRadius* Mathf.Cos(angle),
            bossHornetMovements.Position.y + bossHornetMovements.CircleRadius* Mathf.Sin(angle) * BossHornetFlyingDirection(bossHornet), 0);

    public float BossHornetFlyingDirection(IBossHornet bossHornet) =>
        bossHornet.FlyingPosition % 2 == 0 ? 1 : -1;

}

public class CirclePositionUpdater : IHornetPositionUpdater
{
    public Vector3 CalculateNewPosition(IBossHornetMovements bossHornetMovements, IBossHornet bossHornet, float angle) =>
        new Vector3(
            bossHornetMovements.Position.x + bossHornetMovements.CircleRadius * Mathf.Cos(angle),
            bossHornetMovements.Position.y + bossHornetMovements.CircleRadius * Mathf.Sin(angle), 0);
}

public class BossHornetAttack : BossHornetState
{
    private readonly float _stateStartTime;
    private float? _attackDirection;
    private AttackType _attackType;

    public BossHornetAttack(ITime time, AttackType attackType) : base(time)
    {
        _stateStartTime = time.RealtimeSinceStartup;
        _attackType = attackType;
    }

    public override void Handle()
    {
        foreach (var bossHornet in _context.ActiveBossHornets)
        {
            UpdateHornetPosition(bossHornet, _stateStartTime + _context.MovementStartTimeDiffBetweenHornets * bossHornet.FlyingPosition);
        }

        if (_context.ReadyToStateTransition)
        {
            if(_attackType == AttackType.FlyInCircle)
                _context.TransitionTo(new BossHornetFlyInCircle(_time, new CirclePositionUpdater() ,CurrentCircleAngle), "IsFlying");
            else
                _context.TransitionTo(new BossHornetFlyInCircle(_time, new DividedCirclePositionUpdater(), CurrentCircleAngle), "IsFlying");
        }
    }

    void UpdateHornetPosition(IBossHornet bossHornet, float movementStartTime)
    {
        if (!_attackDirection.HasValue)
            _attackDirection = Mathf.Sign(_context.Position.x - bossHornet.Position.x);

        if (_time.RealtimeSinceStartup > movementStartTime)
        {
            if (HornetReachedFinalPosition(bossHornet))
            {
                bossHornet.ReadyToTransition = true;
                return;
            }

            bossHornet.TranslatePosition(new Vector3(_attackDirection.Value * _context.CurrentAttackVelocity * _time.DeltaTime, 0, 0));
        }
    }

    float CurrentCircleAngle => _attackDirection > 0 ? 0.0f : Mathf.PI;

    bool HornetReachedFinalPosition(IBossHornet bossHornet)
    {
        var hornetX = bossHornet.Position.x;
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
        ? _context.Position.x + _context.CircleRadius
        : _context.Position.x - _context.CircleRadius;
}

