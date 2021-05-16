using UnityEngine;

public class CreateParticleEffectOnDeath : MonoBehaviour, IDying
{
    public GameObject[] particleEffects;

    public void OnDying()
    {
        if (particleEffects != null)
        {
            foreach(var particleEffect in particleEffects)
                Instantiate(particleEffect, transform.position, Quaternion.identity);
        }
    }
}
