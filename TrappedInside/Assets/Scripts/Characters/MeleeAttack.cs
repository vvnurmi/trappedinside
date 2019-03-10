using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(InputProvider))]
public class MeleeAttack : MonoBehaviour
{
    [Tooltip("The sound to play on melee attack.")]
    public AudioClip meleeSound;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterController2D characterController;
    private CircleCollider2D attackCollider;
    private InputProvider inputProvider;

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController2D>();
        attackCollider = GetComponent<CircleCollider2D>();
        attackCollider.enabled = false;
        inputProvider = GetComponent<InputProvider>();

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);
    }

    private void Update()
    {
        timedAnimTriggers.Update();

        var input = inputProvider.GetInput();
        HandleFireInput(input);
    }

    private void HandleFireInput(PlayerInput input)
    {
        if (input.fire1)
        {
            timedAnimTriggers.Set("StartMelee");
            audioSource.PlayOneShot(meleeSound);
        }
    }

    public void AnimEvent_StartAttacking()
    {
        characterController.state.isInMelee = true;
        attackCollider.enabled = true;
    }

    public void AnimEvent_StopAttacking()
    {
        characterController.state.isInMelee = false;
        attackCollider.enabled = false;
        animator.SetTrigger("StopMelee");
    }
}
