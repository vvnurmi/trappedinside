using UnityEngine;

/// <summary>
/// Makes a character walk aimlessly forward and turn around on collision.
/// </summary>
[RequireComponent(typeof(CharacterState))]
public class WanderingInputProvider : MonoBehaviour, IInputProvider
{
    [Tooltip("Start walking right at start?")]
    public bool startRight = true;

    // Set about once, probably in Start().
    private CharacterState characterState;

    // Modified during gameplay.
    private float horizontalMove;

    private void Start()
    {
        characterState = GetComponent<CharacterState>();

        horizontalMove = startRight ? 1.0f : -1.0f;
    }

    private void FixedUpdate()
    {
        if (characterState  .collisions.HasHorizontalCollisions)
            horizontalMove = -horizontalMove;
    }

    public PlayerInput GetInput()
    {
        return new PlayerInput(
            fire1Pressed: false,
            fire2Pressed: false,
            fire2Active: false,
            jumpPressed: false,
            jumpReleased: false,
            horizontal: horizontalMove,
            vertical: 0.0f);
    }
}
