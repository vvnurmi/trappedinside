using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Methods that handle messages sent via <see cref="GameObject.SendMessage(string)"/>.
/// </summary>
[RequireComponent(typeof(CopperPopperController))]
public class CopperPopperMessageHandlers : MonoBehaviour {
    private CopperPopperController copperPopper;

    private void Start()
    {
        copperPopper = GetComponent<CopperPopperController>();
    }

    public void HandleCollision(CollisionDetails collisionDetails)
    {
        if (copperPopper.IsStaticCocoon)
        {
            float direction = Mathf.Sign(collisionDetails.velocity.x);
            copperPopper.LaunchCocoon(direction);
        }
        else
        {
            if (collisionDetails.velocity.y < 0)
            {
                copperPopper.TakeDamage();
                collisionDetails.collisionObject.RecoilUp();
            }
            else if (collisionDetails.isAttack)
            {
                copperPopper.TakeDamage();
            }
            else
            {
                collisionDetails.collisionObject.TakeDamage();
            }
        }
    }
}
