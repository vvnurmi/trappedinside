using UnityEngine;

[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(Animator))]
public class HitBehavior : MonoBehaviour, IDamaged
{
    public float recoilForce = 1.0f;
    public float inputFreezeTime = 0.5f;

    private CharacterState characterState;
    private Animator animator;
    private float takingDamageEndsAt = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        characterState = GetComponent<CharacterState>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        characterState.isTakingDamage = takingDamageEndsAt > Time.time;
        animator.SetBool("IsTakingDamage", characterState.isTakingDamage);

        if (characterState.isTakingDamage)
            characterState.isClimbing = false;
    }


    public void OnDamaged()
    {
        takingDamageEndsAt = Time.time + inputFreezeTime;
    }

}
