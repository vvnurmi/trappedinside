﻿using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class FlyMovement : MonoBehaviour
{
    private CharacterController2D characterController;
    private ProximityTrigger proximityTrigger;
    private AttackTrigger attackTrigger;
    private FlyState state;

    public bool IsFacingRight => characterController.state.collisions.faceDir == 1;
    public GameObject Player { get; private set; }
    public bool PlayerInProximity => proximityTrigger.PlayerInProximity;
    public bool PlayerInAttackRange => attackTrigger.PlayerInAttackRange;
    public Vector3 NormalizedMovementDirection { get; set; }

    void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        proximityTrigger = GetComponentInChildren<ProximityTrigger>();
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        Player = GameObject.FindWithTag("Player");
        characterController.state.collisions.faceDir = -1;
        TransitionTo(new Idle());
    }

    void Update()
    {
        if (!characterController.state.CanMoveHorizontally)
        {
            return;
        }
        state.Handle();
        transform.Translate(NormalizedMovementDirection * Time.deltaTime, Space.World);
    }

    public void TransitionTo(FlyState state)
    {
        this.state = state;
        state.Context = this;
    }

    public void Flip()
    {
        var collisions = characterController.state.collisions;
        collisions.faceDir = -collisions.faceDir;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);
    }

}

public abstract class FlyState
{
    public FlyMovement Context { get; set; }
    public abstract void Handle();
}

public class Idle : FlyState
{
    public override void Handle()
    {
        if (Context.PlayerInProximity)
        {
            Context.TransitionTo(new Move());
        }
        Context.NormalizedMovementDirection = Vector3.zero;
    }
}

public class Move : FlyState
{
    private System.Random random = new System.Random();
    private float latestMovementUpdateTime = -1.0f;
    private readonly float directionUpdateDelay = 0.3f;
    private readonly float speed = 0.5f;

    public override void Handle()
    {
        if (!Context.PlayerInProximity)
        {
            Context.TransitionTo(new Idle());
        }

        if (Context.PlayerInAttackRange)
        {
            Context.TransitionTo(new PrepareAttack());
        }

        if (TimeToUpdateMovement())
        {
            latestMovementUpdateTime = Time.realtimeSinceStartup;
            Debug.Assert(Context.Player != null, "Player was null in FlyMovement.cs");
            var direction = (Context.Player.transform.position - Context.transform.position).normalized;

            if ((direction.x < 0 && Context.IsFacingRight) || (direction.x > 0 && !Context.IsFacingRight))
                Context.Flip();

            var randomComponent = 2 * new Vector3(0, (float)random.NextDouble() - 0.5f);
            Context.NormalizedMovementDirection = speed * (direction + randomComponent).normalized;
        }
    }

    private bool TimeToUpdateMovement() =>
        Time.realtimeSinceStartup - latestMovementUpdateTime > directionUpdateDelay;

}

public class PrepareAttack : FlyState
{
    private readonly float attackPreparationTime = 0.5f;
    private readonly float prepareStartTime;

    public PrepareAttack()
    {
        prepareStartTime = Time.realtimeSinceStartup;
    }

    public override void Handle()
    {
        if (TimeToUpdateState())
        {
            var attackDirection = (Context.Player.transform.position - Context.transform.position).normalized;
            Context.TransitionTo(new Attack(attackDirection));
        }
        Context.NormalizedMovementDirection = Vector3.zero;
    }

    private bool TimeToUpdateState() =>
        Time.realtimeSinceStartup - prepareStartTime > attackPreparationTime;
}

public class Attack : FlyState
{
    private readonly Vector3 attackDirection;
    private readonly float attackSpeed = 2.0f;
    private readonly float attackTime = 0.75f;
    private readonly float attackStartTime;

    public Attack(Vector3 attackDirection)
    {
        this.attackDirection = attackDirection;
        attackStartTime = Time.realtimeSinceStartup;
    }

    public override void Handle()
    {

        if (TimeToUpdateState())
        {
            Context.TransitionTo(new Idle());
        }

        Context.NormalizedMovementDirection = attackSpeed * attackDirection;
            
    }

    private bool TimeToUpdateState() =>
        Time.realtimeSinceStartup - attackStartTime > attackTime;

}