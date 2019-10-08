using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class FlyMovement : MonoBehaviour
{

    [Tooltip("Speed of fly movement, in world units per second.")]
    public float speed = 0.3f;

    private CharacterController2D characterController;
    private GameObject player;
    private bool followPlayer = false;
    private bool IsFacingRight => characterController.state.collisions.faceDir == 1;

    void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        characterController.state.collisions.faceDir = -1;
    }

    void FixedUpdate()
    {
        if (followPlayer)
        {
            var ownPosition = transform.position;
            var playerPosition = player.transform.position;
            var direction = playerPosition - ownPosition;
            var deltaPosition = direction.normalized * speed * Time.deltaTime;
            transform.Translate(deltaPosition, Space.World);

            if ((deltaPosition.x < 0 && IsFacingRight) || (deltaPosition.x > 0 && !IsFacingRight))
                Flip();

        }
    }

    public void Flip()
    {
        var collisions = characterController.state.collisions;
        collisions.faceDir = -collisions.faceDir;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player == null)
                player = collision.gameObject;

            followPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            followPlayer = false;
    }
}
