using UnityEngine;

/// <summary>
/// When damaged, any possible attack or shielding will be interrupted.
/// </summary>
public class DamageInterruptsAttack : MonoBehaviour, IDamaged
{
    // Set about once, probably in Start().
    private Animator animator;
    private CharacterController2D characterController;

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Assert(animator != null, $"{nameof(Animator)} not found in children of {name}");
        characterController = GetComponentInParent<CharacterController2D>();
        Debug.Assert(characterController != null, $"{nameof(CharacterController2D)} not found in parents of {name}");

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);
    }

    private void Update()
    {
        timedAnimTriggers.Update();
    }

    #endregion

    public void OnDamaged()
    {
        timedAnimTriggers.Set("StopMelee");
    }
}
