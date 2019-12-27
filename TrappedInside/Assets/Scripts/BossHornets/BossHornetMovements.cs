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

[Serializable]
public class BossHornetWaveSettings
{
    public int numberOfHornets;
    public float attackVelocity;
    public float angularVelocity;
    public float numberOfCircles;
    public float circleRadius;
    public AttackType attackType;
}

public class BossHornet
{
    private static readonly List<string> AnimationStates = new List<string> { "IsAttacking", "IsPreparingAttack" };
    private GameObject _hornet;
    private Animator _animator;
    private bool _isFacingLeft;
    private readonly float _timeDiffBetweenHornets;

    public BossHornet(GameObject hornet, Vector3 centerPoint, float finalCircleAngle, float circleRadius, float angularVelocity, float attackVelocity, int flyingPosition, float timeDiffBetweenHornets)
    {
        _hornet = hornet;
        _animator = hornet.GetComponentInChildren<Animator>();
        CenterPoint = centerPoint;
        FinalCircleAngle = finalCircleAngle;
        CircleRadius = circleRadius;
        AngularVelocity = angularVelocity;
        AttackVelocity = attackVelocity;
        FlyingPosition = flyingPosition;
        _isFacingLeft = true;
        _timeDiffBetweenHornets = timeDiffBetweenHornets;
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
    public bool IsReadyForStateTransition
    {
        get {
            if (CurrentState == HornetState.Attacking)
            {
                return (CurrentPosition - CenterPoint).magnitude < 0.05;
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
     
    public int FlyingPosition { get; }
    public HornetState CurrentState { get; set; }

    public void Move(float stateStartTime, float deltaTime)
    {
        if (IsReadyForStateTransition)
            return;

        if (CurrentState == HornetState.Attacking)
        {

            if (Time.time > MovementStartTime(stateStartTime))
            {
                AnimationState = "IsAttacking";
                _hornet.transform.Translate(AttackDirection * AttackVelocity * deltaTime);
            }
            else if (Time.time > BlinkingStartTime(stateStartTime))
            {
                AnimationState = "IsPreparingAttack";
            }
        }
        else if (CurrentState == HornetState.Flying)
        {
            if (Time.time > MovementStartTime(stateStartTime))
            {
                CurrentCircleAngle += AngularVelocity * deltaTime;
                _hornet.transform.position = new Vector3(
                    CenterPoint.x + CircleRadius * Mathf.Cos(CurrentCircleAngle),
                    CenterPoint.y + CircleRadius * Mathf.Sin(CurrentCircleAngle), 0);

                if (FlipRequired)
                    Flip();
            }
        }
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

    internal void UpdateState(float stateStartTime, float movementStartTimeDiffBetweenHornets)
    {
        if (CurrentState == HornetState.Flying)
        {
            CurrentState = HornetState.Attacking;
            AttackDirection = (CenterPoint - CurrentPosition).normalized;
        }
    }

    private float BlinkingStartTime(float stateStartTime) => MovementStartTime(stateStartTime) - 0.5f;

    private float MovementStartTime(float stateStartTime) =>
        stateStartTime + _timeDiffBetweenHornets * (FlyingPosition + 1);

}

public class BossHornetCreator
{
    public BossHornet CreateBossHornet(GameObject bossHornet, Vector3 centerPosition, BossHornetWaveSettings bossHornetWave, int flyingPosition, float timeDiffBetweenHornets)
    {
        var numberOfCircles = bossHornetWave.numberOfCircles;
        var angularVelocity = bossHornetWave.angularVelocity;

        if (bossHornetWave.attackType == AttackType.FlyInDividedCircle)
        {
            if (flyingPosition % 2 == 0)
                angularVelocity = -bossHornetWave.angularVelocity;
        }
        else if (bossHornetWave.attackType == AttackType.EverySecondHornetStopsEarlierUpdated)
        {
            if (flyingPosition % 2 == 0)
                numberOfCircles = bossHornetWave.numberOfCircles + 0.5f;
        }
        else if (bossHornetWave.attackType == AttackType.AttackFromMultipleDirections)
        {
            var step = 0.5f / (bossHornetWave.numberOfHornets - 1);
            numberOfCircles = flyingPosition * step;
        }
        return new BossHornet(
            hornet: bossHornet,
            centerPoint: centerPosition,
            finalCircleAngle: numberOfCircles * 2 * Mathf.PI,
            circleRadius: bossHornetWave.circleRadius,
            angularVelocity: angularVelocity,
            attackVelocity: bossHornetWave.attackVelocity,
            flyingPosition: flyingPosition,
            timeDiffBetweenHornets: timeDiffBetweenHornets);

    }
}

public class BossHornetMovements : MonoBehaviour
{
    public GameObject bossHornetPrefab;
    public float firstHornetMovementStartTime = 3.0f;
    public float movementStartTimeDiffBetweenHornets = 0.5f;
    public BossHornetWaveSettings[] bossHornetWaves;

    private List<BossHornet> _bossHornets = new List<BossHornet>();
    private int _waveNumber = 0;
    private BossHornetCreator _bossHornetCreator = new BossHornetCreator();
    private float _waitStartTime;
    private float _stateStartTime = -1.0f;

    void Start()
    {
        InitWave();
    }

    void FixedUpdate()
    {
        if (AllHornetsInCurrentWaveDead)
        {
            InitNextWave();
            _stateStartTime = -1;
        }

        if (Time.realtimeSinceStartup - _waitStartTime < firstHornetMovementStartTime)
            return;

        if (_stateStartTime < 0.0)
            _stateStartTime = Time.realtimeSinceStartup;

        foreach (var bossHornet in ActiveBossHornets)
        {
            bossHornet.Move(_stateStartTime, Time.deltaTime);
        }

        if (ReadyToStateTransition)
        {
            UpdateHornetStates();
            _stateStartTime = Time.realtimeSinceStartup;
        }

    }

    private void UpdateHornetStates()
    {
        foreach (var bossHornet in ActiveBossHornets)
            bossHornet.UpdateState(_stateStartTime, movementStartTimeDiffBetweenHornets);
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
        _waitStartTime = Time.realtimeSinceStartup;

        _bossHornets.Clear();


        for (int i = 0; i < bossHornetWaves[_waveNumber].numberOfHornets; i++)
        {
            var hornetStartingPosition = transform.position + new Vector3(bossHornetWaves[_waveNumber].circleRadius, 0, 0);
            _bossHornets.Add(_bossHornetCreator.CreateBossHornet(
                             bossHornet: Instantiate(bossHornetPrefab, hornetStartingPosition, Quaternion.identity),
                             centerPosition: transform.position,
                             bossHornetWave: bossHornetWaves[_waveNumber],
                             flyingPosition: i,
                             timeDiffBetweenHornets: movementStartTimeDiffBetweenHornets));
        }
    }

    public IEnumerable<BossHornet> ActiveBossHornets => _bossHornets.Where(h => h.IsActive);
    public bool AllHornetsInCurrentWaveDead => _bossHornets.All(h => !h.IsActive);
    public bool ReadyToStateTransition => ActiveBossHornets.All(bossHornet => bossHornet.IsReadyForStateTransition);
}

