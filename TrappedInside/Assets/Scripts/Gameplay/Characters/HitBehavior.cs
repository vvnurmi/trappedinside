﻿using UnityEngine;

public class HitBehavior : MonoBehaviour, IDamaged
{
    [Tooltip("Seconds after hit until player can move.")]
    public float inputFreezeTime = 0.5f;

    [Tooltip("Seconds after hit until another hit is possible.")]
    public float invulnerabilityTime = 1.0f;

    private CharacterState characterState;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float takingDamageEndsAt = 0.0f;
    private float invulnerabilityEndsAt = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        characterState = GetComponentInParent<CharacterState>();
        animator = GetComponentInParent<Animator>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        characterState.isTakingDamage = takingDamageEndsAt > Time.time;
        characterState.isInvulnerable = invulnerabilityEndsAt > Time.time;

        if (!characterState.isInvulnerable)
        {
            Debug.Assert(!characterState.isTakingDamage); //Invulnerability should take longer than damage state
            spriteRenderer.enabled = true;
            return;
        }

        spriteRenderer.enabled = !spriteRenderer.enabled;

        if(!characterState.isDead)
            animator.SetBool("IsTakingDamage", characterState.isTakingDamage);

        if (characterState.isTakingDamage)
            characterState.isClimbing = false;
    }


    public void OnDamaged()
    {
        takingDamageEndsAt = Time.time + inputFreezeTime;
        invulnerabilityEndsAt = Time.time + invulnerabilityTime;
    }

}
