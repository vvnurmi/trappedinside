using UnityEngine;
/// <summary>
/// Makes a character walk aimlessly forward and turn around on collision.
/// </summary>
[RequireComponent(typeof(CharacterController2D))]
public class WanderingInputProvider : InputProvider
{
    [Tooltip("Start walking right at start?")]
    public bool startRight = true;

    // Set about once, probably in Start().
    private CharacterController2D characterController;

    // Modified during gameplay.
    private float horizontalMove;

    private void Start()
    {
        characterController = GetComponent<CharacterController2D>();

        horizontalMove = startRight ? 1.0f : -1.0f;
    }

    private void FixedUpdate()
    {
        if (characterController.state.collisions.HasHorizontalCollisions)
            horizontalMove = -horizontalMove;
    }

    public override PlayerInput GetInput()
    {
        return new PlayerInput(
            fire1: false,
            fire2Pressed: false,
            fire2Released: false,
            jumpPressed: false,
            jumpReleased: false,
            horizontal: horizontalMove,
            vertical: 0.0f);
    }
}
