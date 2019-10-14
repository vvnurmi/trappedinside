using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class FlyMovement : MonoBehaviour
{
    [Tooltip("Speed of fly movement, in world units per second.")]
    public float speed = 0.5f;

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
        if (proximityTrigger.PlayerInProximity && characterController.state.CanMoveHorizontally)
        {
            Debug.Assert(player != null, "Player was null in FlyMovement.cs");
            var direction = player.transform.position - transform.position;
            var deltaPosition = direction.normalized * speed * Time.deltaTime;
            var randomComponent = deltaPosition.magnitude * new Vector3(0, (float)random.NextDouble() - 0.5f);
            transform.Translate(deltaPosition + randomComponent, Space.World);

            if ((deltaPosition.x < 0 && IsFacingRight) ||  (deltaPosition.x > 0 && !IsFacingRight))
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

}
