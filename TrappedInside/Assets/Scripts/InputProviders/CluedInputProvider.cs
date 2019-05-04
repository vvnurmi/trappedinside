using UnityEngine;

public enum Direction { Left, Right, Down, Up }

[RequireComponent(typeof(CharacterController2D))]
public class CluedInputProvider : InputProvider
{
    public Direction walkingDirection = Direction.Right;

    public Direction wallDirection = Direction.Down;

    private CharacterController2D characterController;

    private float HorizontalMove {
        get {
            if (walkingDirection == Direction.Right || (wallDirection == Direction.Right && MovementTowardsWallEnabled))
                return 1;
            else if (walkingDirection == Direction.Left || (wallDirection == Direction.Left && MovementTowardsWallEnabled))
                return -1;
            else
                return 0;
        }
    }
    private float VerticalMove {
        get {
            if (walkingDirection == Direction.Up || (wallDirection == Direction.Up && MovementTowardsWallEnabled))
                return 1;
            else if (walkingDirection == Direction.Down || (wallDirection == Direction.Down && MovementTowardsWallEnabled))
                return -1;
            else
                return 0;
        }
    }

    private bool MovingVertically => walkingDirection == Direction.Up || walkingDirection == Direction.Down;

    private bool MovingHorizontally => !MovingVertically;

    private bool HasHorizontalCollisions => characterController.state.collisions.HasHorizontalCollisions;

    private bool HasVerticalCollisions => characterController.state.collisions.HasVerticalCollisions;

    private bool HasCollisions => HasHorizontalCollisions || HasVerticalCollisions;

    private bool MovementTowardsWallEnabled => framesToWaitWallMovement < 0;

    private bool firstCollisionDetected = false;

    private readonly int wallMovementDelay = 10;
    private int framesToWaitWallMovement = -1;

    private void Start()
    {
        characterController = GetComponent<CharacterController2D>();
    }

    private void FixedUpdate()
    {
        framesToWaitWallMovement--;
        if(HasCollisions)
        {
            firstCollisionDetected = true;
        }

        if (firstCollisionDetected && !HasCollisions)
        {
            firstCollisionDetected = false;
            framesToWaitWallMovement = wallMovementDelay;
            var previousWalkingDirection = walkingDirection;
            if (MovingVertically)
            {
                walkingDirection = wallDirection;
                wallDirection = previousWalkingDirection == Direction.Down ? Direction.Up : Direction.Down;
            } else
            {
                walkingDirection = wallDirection;
                wallDirection = previousWalkingDirection == Direction.Right ? Direction.Left : Direction.Right;
            }
        }
    }

    public override PlayerInput GetInput() => 
        new PlayerInput(
            fire1: false,
            fire2: false,
            jumpPressed: false,
            jumpReleased: false,
            horizontal: HorizontalMove,
            vertical: VerticalMove);


}