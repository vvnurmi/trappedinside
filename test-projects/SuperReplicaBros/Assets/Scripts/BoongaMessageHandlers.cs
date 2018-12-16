using UnityEngine;

/// <summary>
/// Methods that handle messages sent via <see cref="GameObject.SendMessage(string)"/>.
/// </summary>
[RequireComponent(typeof(BoongaController))]
public class BoongaMessageHandlers : MonoBehaviour
{
    private BoongaController boonga;

    private void Start()
    {
        boonga = GetComponent<BoongaController>();
    }

    public void HandleCollision(CollisionDetails collisionDetails)
    {
        if (collisionDetails.velocity.y < 0)
        {
            boonga.TakeDamage();
            collisionDetails.collisionObject.RecoilUp();
        }
        else if (collisionDetails.isAttack)
        {
            boonga.TakeDamage();
        }
        else
        {
            collisionDetails.collisionObject.TakeDamage();
        }
    }
}
