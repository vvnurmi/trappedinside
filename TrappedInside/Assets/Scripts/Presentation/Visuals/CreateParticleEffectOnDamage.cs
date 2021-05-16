using UnityEngine;

public class CreateParticleEffectOnDamage : MonoBehaviour, IDamaged
{
    public GameObject[] particleEffects;

    public void OnDamaged()
    {
        if (particleEffects != null)
        {
            foreach (var particleEffect in particleEffects)
                Instantiate(particleEffect, transform.position, Quaternion.identity);
        }
    }
}
