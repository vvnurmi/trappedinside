using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HornetState
{
    Flying, Attacking
}

[Serializable]
public class BossHornetSettings
{
    public float finalCircleAngle;
    public float circleRadius;
    public float angularVelocity;
    public float attackVelocity;
    public float movementStartTime;
    public float attackStartTime;
}

[Serializable]
public class TiaScriptSettings
{
    public string scriptName;
}

[Serializable]
public class BossHornetWaveSettings
{
    public BossHornetSettings[] bossHornetSettings;
    public TiaScriptSettings tiaScriptSettings;
}

public class BossHornetWave : UnityEngine.Object
{
    private List<BossHornet> _bossHornets;
    public float movementStartTimeDiffBetweenHornets = 0.5f;

    public BossHornetWave(GameObject bossHornetPrefab, BossHornetSettings[] bossHornetSettings, Vector3 centerPoint)
    {
        _bossHornets = new List<BossHornet>();
        int randomSeed = 0;
        foreach(var settings in bossHornetSettings)
        {
            var hornetStartingPosition = centerPoint + new Vector3(settings.circleRadius, 0, 0);
            _bossHornets.Add(
                new BossHornet(
                    Instantiate(bossHornetPrefab, hornetStartingPosition, Quaternion.identity), 
                    settings,
                    centerPoint,
                    randomSeed++));
        }
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
        Vector3 centerPoint,
        int randomSeed)
    {
        _hornet = hornet;
        _animator = hornet.GetComponentInChildren<Animator>();
        CenterPoint = centerPoint;
        FinalCircleAngle = 2 * Mathf.PI * settings.finalCircleAngle;
        CircleRadius = settings.circleRadius;
        AngularVelocity = settings.angularVelocity;
        AttackVelocity = settings.attackVelocity;
        _isFacingLeft = true;
        _movementStartTime = Time.time + settings.movementStartTime;
        _attackStartTime = Time.time + settings.attackStartTime;
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
    public TiaPlayer tiaPlayer;
    public BossHornetWaveSettings[] bossHornetWaveSettings;
    private int _bossHornetWaveIndex = 0;
    private BossHornetWave _currentWave;
    private bool _scriptPlayed;

    void Start()
    {
        _currentWave = new BossHornetWave(
            bossHornetPrefab, 
            bossHornetWaveSettings[_bossHornetWaveIndex].bossHornetSettings, 
            transform.position);
        _scriptPlayed = false;
    }

    void FixedUpdate()
    {
        if(_currentWave.Complete && !_scriptPlayed)
        {
            TiaMethods.TryPlayScript(
                tiaPlayer.name, 
                bossHornetWaveSettings[_bossHornetWaveIndex].tiaScriptSettings.scriptName)
                .ContinueWith(
                    t => Debug.LogError(t.Exception),
                    System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
            _scriptPlayed = true;
        }
        else if(_currentWave.Complete && !tiaPlayer.IsPlaying && _bossHornetWaveIndex < bossHornetWaveSettings.Length - 1)
        {
            _bossHornetWaveIndex++;
            _currentWave = new BossHornetWave(
                bossHornetPrefab,
                bossHornetWaveSettings[_bossHornetWaveIndex].bossHornetSettings, 
                transform.position);
            _scriptPlayed = false;
        }

        _currentWave.FixedUpdate();
    }

}
