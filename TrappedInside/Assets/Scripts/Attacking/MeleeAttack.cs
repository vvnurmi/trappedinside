using UnityEngine;

/// <summary>
/// Handles melee attacking.
/// 
/// Melee is initiated by MeleeAttack and it's phases are controlled by the
/// character's animation.
/// 
/// Usage tip: <see cref="weapon"/> should have a collider and a script to
/// inflict damage on attack collision.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(InputProvider))]
public class MeleeAttack : MonoBehaviour
{
    [Tooltip("The sound to play on melee attack.")]
    public AudioClip meleeSound;

    [Tooltip("Subobject that has the melee collider.")]
    public GameObject weapon;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterController2D characterController;
    private InputProvider inputProvider;

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController2D>();
        weapon.SetActive(false);
        inputProvider = GetComponent<InputProvider>();

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);
    }

    private void Update()
    {
        timedAnimTriggers.Update();

        var input = inputProvider.GetInput();
        HandleFireInput(input);
    }

    #endregion

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
        weapon.SetActive(true);
    }

    public void AnimEvent_StopAttacking()
    {
        characterController.state.isInMelee = false;
        weapon.SetActive(false);
        animator.SetTrigger("StopMelee");
    }
}
