using UnityEngine;

public class CreateParticleEffectOnDeath : MonoBehaviour, IDying
{
    public GameObject particleEffect;

    public void OnDying()
    {
        if (particleEffect != null)
        {
            Instantiate(particleEffect, transform.position, Quaternion.identity);
        }
    }

}
