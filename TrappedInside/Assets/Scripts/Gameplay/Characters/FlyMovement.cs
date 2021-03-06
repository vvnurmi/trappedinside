﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CharacterState))]
public class FlyMovement : MonoBehaviour
{
    private CharacterState characterState;
    private ProximityTrigger proximityTrigger;
    private AttackTrigger attackTrigger;
    private Animator animator;
    private FlyState state;
    private RaycastCollider groundCollider;
    private BoxCollider2D boxCollider;
    private HitPoints hitPoints;
    private readonly List<string> animationStates = new List<string> { "IsFlying", "IsPreparingAttack", "IsAttacking" };

    public RaycastColliderConfig groundColliderConfig;

    public bool IsFacingRight => characterState.collisions.faceDir == 1;
    public GameObject Player { get; private set; }
    public bool PlayerInProximity => proximityTrigger.PlayerInProximity;
    public bool PlayerInAttackRange => attackTrigger.PlayerInAttackRange;
    public Vector3 NormalizedMovementDirection { get; set; }

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        characterState = GetComponent<CharacterState>();
        groundCollider = new RaycastCollider(
                groundColliderConfig,
                boxCollider,
                characterState.collisions);
        proximityTrigger = GetComponentInChildren<ProximityTrigger>();
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        animator = GetComponentInChildren<Animator>();
        hitPoints = GetComponentInChildren<HitPoints>();
        Player = GameObject.FindWithTag("Player");
        characterState.collisions.faceDir = -1;
        TransitionTo(new Idle());
    }

    void FixedUpdate()
    {
        if (!characterState.CanMoveHorizontally)
        {
            return;
        }
        state.Handle();
        Move(NormalizedMovementDirection * Time.deltaTime);

        if (CollisionWhileAttacking)
        {
            NormalizedMovementDirection = Vector2.zero;
            hitPoints.Damage(1);
        }


    }

    private bool CollisionWhileAttacking =>
        state.AnimationTransitionTrigger == "IsAttacking" &&
        (characterState.collisions.HasVerticalCollisions || characterState.collisions.HasHorizontalCollisions);

    private void Move(Vector2 moveAmount)
    {
        groundCollider.UpdateRaycastOrigins();

        CollisionInfo collisions = characterState.collisions;
        collisions.Reset();

        groundCollider.HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
            groundCollider.VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);
    }

    public void TransitionTo(FlyState state)
    {
        this.state = state;
        state.Context = this;
        animationStates.ForEach(str => animator.SetBool(str, false));
        animator.SetBool(state.AnimationTransitionTrigger, true);
    }

    public void Flip()
    {
        var collisions = characterState.collisions;
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
    public abstract string AnimationTransitionTrigger { get; }
}

public class Idle : FlyState
{
    public override string AnimationTransitionTrigger => "IsFlying";

    public override void Handle()
    {

        if (Context.PlayerInProximity)
        {
            Context.TransitionTo(new ApproachPlayer());
        }
        Context.NormalizedMovementDirection = Vector3.zero;
    }
}

public class ApproachPlayer : FlyState
{
    private System.Random random = new System.Random();
    private float latestMovementUpdateTime = -1.0f;
    private float? flyStateStartTime = null;

    private readonly float directionUpdateDelay = 0.3f;
    private readonly float minimumFlyStateDuration = 1.0f;
    private readonly float speed = 0.5f;

    public override string AnimationTransitionTrigger => "IsFlying";

    public override void Handle()
    {
        if (flyStateStartTime == null)
            flyStateStartTime = Time.time;

        if (!Context.PlayerInProximity && MinimumFlyStateTimeExceeded)
        {
            Context.TransitionTo(new Idle());
        }

        if (Context.PlayerInAttackRange && MinimumFlyStateTimeExceeded)
        {
            Context.TransitionTo(new PrepareAttack());
        }

        if (TimeToUpdateMovement)
        {
            latestMovementUpdateTime = Time.realtimeSinceStartup;
            Debug.Assert(Context.Player != null, "Player was null in FlyMovement.cs");

            var direction = (Context.Player.transform.position + DirectionOffset - Context.transform.position).normalized;
            var playerDirection = (Context.Player.transform.position - Context.transform.position).normalized;

            if ((playerDirection.x < 0 && Context.IsFacingRight) || (playerDirection.x > 0 && !Context.IsFacingRight))
                Context.Flip();

            var randomComponent = 2 * new Vector3(0, (float)random.NextDouble() - 0.5f);
            Context.NormalizedMovementDirection = speed * (direction + randomComponent).normalized;
        }
    }

    private bool MinimumFlyStateTimeExceeded => Time.time - flyStateStartTime > minimumFlyStateDuration;

    //Instead of moving directly to player, prefer side attacks that are easier to dodge.
    private Vector3 DirectionOffset =>
        Context.Player.transform.position.x > Context.transform.position.x
            ? new Vector3(-0.5f, 0f, 0f)
            : new Vector3(0.5f, 0f, 0f);

    private bool TimeToUpdateMovement =>
        Time.realtimeSinceStartup - latestMovementUpdateTime > directionUpdateDelay;

}

public class PrepareAttack : FlyState
{
    private readonly float attackPreparationTime = 0.5f;
    private readonly float prepareStartTime;
    private Vector3 attackDirection = Vector3.zero;

    public override string AnimationTransitionTrigger => "IsPreparingAttack";

    public PrepareAttack()
    {
        prepareStartTime = Time.time;
    }

    public override void Handle()
    {
        //Fix attack direction before attack so that player has a change to avoid the attack
        if (attackDirection.magnitude == 0.0f)
            attackDirection = (Context.Player.transform.position - Context.transform.position).normalized;

        if (TimeToUpdateState)
            Context.TransitionTo(new Attack(attackDirection));

        Context.NormalizedMovementDirection = Vector3.zero;
    }

    private bool TimeToUpdateState =>
        Time.time - prepareStartTime > attackPreparationTime;
}

public class Attack : FlyState
{
    private readonly Vector3 attackDirection;
    private readonly float attackSpeed = 2.0f;
    private readonly float attackTime = 0.75f;
    private readonly float attackStartTime;

    public override string AnimationTransitionTrigger => "IsAttacking";

    public Attack(Vector3 attackDirection)
    {
        this.attackDirection = attackDirection;
        attackStartTime = Time.realtimeSinceStartup;
    }

    public override void Handle()
    {

        if (TimeToUpdateState)
        {
            Context.TransitionTo(new Idle());
        }

        Context.NormalizedMovementDirection = attackSpeed * attackDirection;
            
    }

    private bool TimeToUpdateState =>
        Time.realtimeSinceStartup - attackStartTime > attackTime;

}