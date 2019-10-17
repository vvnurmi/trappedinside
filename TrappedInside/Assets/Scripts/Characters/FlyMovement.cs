using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class FlyMovement : MonoBehaviour
{
    [Tooltip("Speed of fly movement, in world units per second.")]
    public float speed = 0.5f;

    private readonly float directionUpdateDelay = 0.3f;
    private float latestMovementUpdateTime = -1.0f;
    private Vector3 movementDirection = Vector3.zero;

    private CharacterController2D characterController;
    private bool IsFacingRight => characterController.state.collisions.faceDir == 1;
    private ProximityTrigger proximityTrigger;
    private GameObject player;
    private System.Random random = new System.Random();

    void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        proximityTrigger = GetComponentInChildren<ProximityTrigger>();
        player = GameObject.FindWithTag("Player");
        characterController.state.collisions.faceDir = -1;
    }

    void FixedUpdate()
    {
        if (TimeToUpdateDirection())
        {
            latestMovementUpdateTime = Time.realtimeSinceStartup;
            if (proximityTrigger.PlayerInProximity && characterController.state.CanMoveHorizontally)
            {
                Debug.Assert(player != null, "Player was null in FlyMovement.cs");
                var direction = (player.transform.position - transform.position).normalized;

                if ((direction.x < 0 && IsFacingRight) || (direction.x > 0 && !IsFacingRight))
                    Flip();

                var randomComponent = 2 * new Vector3(0, (float)random.NextDouble() - 0.5f);
                movementDirection = direction + randomComponent;
            }
            else
            {
                movementDirection = Vector3.zero;
            }
        }

        transform.Translate(movementDirection.normalized * speed * Time.deltaTime, Space.World);
    }

    private bool TimeToUpdateDirection() =>
        Time.realtimeSinceStartup - latestMovementUpdateTime > directionUpdateDelay;

    public void Flip()
    {
        var collisions = characterController.state.collisions;
        collisions.faceDir = -collisions.faceDir;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);
    }

}
