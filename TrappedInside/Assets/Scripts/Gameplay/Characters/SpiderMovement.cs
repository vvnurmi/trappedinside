using System.Collections.Generic;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{

    enum State { ReadyForAttack, GettingUp, Attacking }

    public float attackSpeed = 2.0f;
    public float returnSpeed = 0.5f;
    

    private AttackTrigger attackTrigger;
    private ContactTrigger contactTrigger;
    private Animator animator;
    private State state = State.ReadyForAttack;
    private Vector2 startingPosition;

    private static readonly string biting = "Biting";
    private static readonly string still = "Still";
    private readonly List<string> animatorStates = new List<string> { biting, still };


    void Start()
    {
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        contactTrigger = GetComponentInChildren<ContactTrigger>();
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (state == State.ReadyForAttack)
        {
            if (attackTrigger.PlayerInAttackRange)
            {
                SetAnimatorState(biting);
                state = State.Attacking;
            }
        }
        else
        {
            var x = 0.02f * WebLength * Mathf.Sin(2f * Time.time);
            transform.position = new Vector3(startingPosition.x + x, transform.position.y);
            if (state == State.Attacking)
            {
                transform.Translate(new Vector3(0, -attackSpeed * Time.deltaTime));
                if (contactTrigger.Contact)
                {
                    state = State.GettingUp;
                }
            }
            else if (state == State.GettingUp)
            {
                transform.Translate(new Vector3(0, returnSpeed * Time.deltaTime));
                if (Mathf.Abs(transform.position.y - startingPosition.y) < 0.01)
                {
                    state = State.ReadyForAttack;
                    transform.position = startingPosition;
                    SetAnimatorState(still);
                }
            }
        }
    }

    private float WebLength => Mathf.Abs(startingPosition.y - transform.position.y);

    private void SetAnimatorState(string state)
    {
        ClearAnimatorState();
        animator.SetBool(state, true);
    }

    private void ClearAnimatorState()
    {
        foreach (var animatorState in animatorStates)
            animator.SetBool(animatorState, false);
    }


}
