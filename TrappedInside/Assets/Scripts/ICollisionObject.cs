using UnityEngine;
using System.Collections;

public interface ICollisionObject
{
    void TakeDamage();
    void RecoilUp();
    void Flip();
}
