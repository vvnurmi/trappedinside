using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AttackType
{
    FlyInCircle,
    FlyInDividedCircle,
    EverySecondHornetStopsEarlierUpdated,
    AttackFromMultipleDirections
}

public enum HornetState
{
    Flying, Attacking
}

public class BossHornetSettings
{
    public BossHornetSettings(
        Vector3 centerPoint,
        float finalCircleAngle,
        float circleRadius,
        float angularVelocity,
        float attackVelocity,
        float movementStartTime,
        float attackStartTime)
    {
        CenterPoint = centerPoint;
        FinalCircleAngle = finalCircleAngle;
        CircleRadius = circleRadius;
        AngularVelocity = angularVelocity;
        AttackVelocity = attackVelocity;
        MovementStartTime = movementStartTime;
        AttackStartTime = attackStartTime;
    }

    public Vector3 CenterPoint { get; }
    public float FinalCircleAngle { get; }
    public float CircleRadius { get; }
    public float AngularVelocity { get; }
    public float AttackVelocity { get; }
    public float MovementStartTime { get; }
    public float AttackStartTime { get; }
}

public interface IBossStep
{
    void Start();
    void FixedUpdate();
    bool Complete { get; }
}

public class BossHornetWave : UnityEngine.Object, IBossStep 
{
    private List<BossHornet> _bossHornets;
    public float movementStartTimeDiffBetweenHornets = 0.5f;

    public BossHornetWave(GameObject bossHornetPrefab, List<BossHornetSettings> bossHornetSettings)
    {
        _bossHornets = new List<BossHornet>();
        int randomSeed = 0;
        foreach(var settings in bossHornetSettings)
        {
            var hornetStartingPosition = settings.CenterPoint + new Vector3(settings.CircleRadius, 0, 0);
            _bossHornets.Add(
                new BossHornet(
                    Instantiate(bossHornetPrefab, hornetStartingPosition, Quaternion.identity), 
                    settings,
                    randomSeed++));
        }
    }

    public void Start()
    {
    }

    public bool Complete => AllHornetsInCurrentWaveDead;
    public IEnumerable<BossHornet> ActiveBossHornets => _bossHornets.Where(h => h.IsActive);
    public bool AllHornetsInCurrentWaveDead => _bossHornets.All(h => !h.IsActive);

    public void FixedUpdate()
    {
        if (AllHornetsInCurrentWaveDead)
            return;

        foreach (var bossHornet in ActiveBossHornets)
            bossHornet.Move(Time.deltaTime);

    }
}


public class BossHornet
{
    private static readonly List<string> AnimationStates = new List<string> { "IsAttacking", "IsPreparingAttack" };
    private GameObject _hornet;
    private Animator _animator;
    private bool _isFacingLeft;
    private readonly float _movementStartTime;
    private readonly float _attackStartTime;
    private System.Random _random;

    public BossHornet(
        GameObject hornet,
        BossHornetSettings settings,
        int randomSeed)
    {
        _hornet = hornet;
        _animator = hornet.GetComponentInChildren<Animator>();
        CenterPoint = settings.CenterPoint;
        FinalCircleAngle = settings.FinalCircleAngle;
        CircleRadius = settings.CircleRadius;
        AngularVelocity = settings.AngularVelocity;
        AttackVelocity = settings.AttackVelocity;
        _isFacingLeft = true;
        _movementStartTime = Time.time + settings.MovementStartTime;
        _attackStartTime = Time.time + settings.AttackStartTime;
        _random = new System.Random(randomSeed);
    }

    public string AnimationState
    {
        set
        {
            if (!_animator.GetBool(value))
            {
                AnimationStates.ForEach(s => _animator.SetBool(s, false));
                _animator.SetBool(value, true);
            }
        }
    }
    public Vector3 CenterPoint { get; }
    public float FinalCircleAngle { get; }
    public float CircleRadius { get; }
    public float AngularVelocity { get; }
    public float AttackVelocity { get; }
    public Vector3 CurrentPosition => _hornet.transform.position;
    public bool IsActive => _hornet != null;
    public float CurrentCircleAngle { get; set; }

    public Vector3 AttackDirection { get; set; }

    public float StartingCircleAngle { get; set; }
    public bool StateFinalPositionReached
    {
        get {
            if (CurrentState == HornetState.Attacking)
            {
                return (CurrentPosition - CenterPoint).magnitude < 0.07;
            }
            else
            {
                if (AngularVelocity > 0)
                    return CurrentCircleAngle > StartingCircleAngle + FinalCircleAngle;
                else
                    return StartingCircleAngle - CurrentCircleAngle > FinalCircleAngle;
            }
        }
    } 
     
    public HornetState CurrentState { get; set; }

    public void Move(float deltaTime)
    {
        if (_hornet.GetComponent<CharacterState>().isDead)
            return;

        if (CurrentState == HornetState.Attacking)
        {
            if (StateFinalPositionReached)
                return;

            if (Time.time > _attackStartTime)
            {
                AnimationState = "IsAttacking";
                _hornet.transform.Translate(AttackDirection * AttackVelocity * deltaTime);
            }
            else if (Time.time > _attackStartTime - 0.5f)
            {
                AnimationState = "IsPreparingAttack";
            }
            else
            {
                RandomMovement(deltaTime);
            }
        }
        else if (CurrentState == HornetState.Flying)
        {
            if (StateFinalPositionReached)
            {
                StartAttack();
                return;
            }

            if (Time.time > _movementStartTime)
            {
                CurrentCircleAngle += AngularVelocity * deltaTime;
                _hornet.transform.position = Vector3.Lerp(CurrentPosition, new Vector3(
                    CenterPoint.x + CircleRadius * Mathf.Cos(CurrentCircleAngle),
                    CenterPoint.y + CircleRadius * Mathf.Sin(CurrentCircleAngle), 0), 0.5f);

                if (FlipRequired)
                    Flip();
            }
            else
            {
                RandomMovement(deltaTime);
            }
        }
    }

    public void RandomMovement(float deltaTime)
    {
        var direction = new Vector3((float)_random.NextDouble() - 0.5f, (float)_random.NextDouble() - 0.5f);
        _hornet.transform.position += 0.25f * direction * deltaTime;
    }

    private bool FlipRequired =>
        CurrentPosition.x < CenterPoint.x && _isFacingLeft ||
        CurrentPosition.x >= CenterPoint.x && !_isFacingLeft;

    public void Flip()
    {
        _isFacingLeft = !_isFacingLeft;
        var originalScale = _hornet.transform.localScale;
        _hornet.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }

    internal void StartAttack()
    {
        CurrentState = HornetState.Attacking;
        AttackDirection = (CenterPoint - CurrentPosition).normalized;
    }
}


public class BossHornetMovements : MonoBehaviour
{
    public GameObject bossHornetPrefab;
    private List<List<BossHornetSettings>> _bossHornetSettings;
    private int _bossHornetWaveIndex = 0;

    private IBossStep _bossStep;

    void Start()
    {
        var hornetStartingPosition = transform.position + new Vector3(1, 0, 0);

        _bossHornetSettings = new List<List<BossHornetSettings>>
        {
            new List<BossHornetSettings>
            {
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3 * Mathf.PI,
                    circleRadius: 0.8f,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 3,
                    attackStartTime: 10f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 2 * Mathf.PI,
                    circleRadius: 0.8f,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 5,
                    attackStartTime: 11f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3 * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 3,
                    attackStartTime: 12f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 2.5f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 4,
                    attackStartTime: 13f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 2 * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 5,
                    attackStartTime: 14f)
            },
            new List<BossHornetSettings>
            {
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 4 * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 3,
                    attackStartTime: 16f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3.75f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 4,
                    attackStartTime: 17f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3.25f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 5,
                    attackStartTime: 15f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 6,
                    attackStartTime: 14f),
            },
            new List<BossHornetSettings>
            {
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 4 * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 3,
                    attackStartTime: 14f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3.5f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 4,
                    attackStartTime: 15f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 5,
                    attackStartTime: 13f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 5.75f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 6,
                    attackStartTime: 20f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 5.5f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 7,
                    attackStartTime: 22f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 5.25f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 8,
                    attackStartTime: 21f),
            },
            new List<BossHornetSettings>
            {
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 4 * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 3,
                    attackStartTime: 16f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3.75f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 4,
                    attackStartTime: 15f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3.25f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 5,
                    attackStartTime: 17f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 3f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: -1.5f,
                    attackVelocity: 2,
                    movementStartTime: 6,
                    attackStartTime: 14f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 6 * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 3,
                    attackStartTime: 21f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 6.25f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 4,
                    attackStartTime: 24f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 6.75f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 5,
                    attackStartTime: 22f),
                new BossHornetSettings(
                    centerPoint: transform.position,
                    finalCircleAngle: 7f * Mathf.PI,
                    circleRadius: 1,
                    angularVelocity: 1.5f,
                    attackVelocity: 2,
                    movementStartTime: 6,
                    attackStartTime: 23f),
            },
        };

        _bossStep = new BossHornetWave(bossHornetPrefab, _bossHornetSettings[_bossHornetWaveIndex]);
        _bossStep.Start();
    }

    void FixedUpdate()
    {
        if(_bossStep.Complete && _bossHornetWaveIndex < _bossHornetSettings.Count - 1)
        {
            _bossHornetWaveIndex++;
            _bossStep = new BossHornetWave(bossHornetPrefab, _bossHornetSettings[_bossHornetWaveIndex]);
            _bossStep.Start();
        }

        _bossStep.FixedUpdate();
    }

}

