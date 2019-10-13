using UnityEngine;

[RequireComponent(typeof(HitPoints))]
public class DestroyObjectOnDeath : MonoBehaviour, IDying
{
    public float delay = 1.0f;

    public void OnDying()
    {
        DestroyGameObject(gameObject);
    }

    private void DestroyGameObject(GameObject gObject)
    {
        var parent = gObject.transform.parent;
        if (parent != null)
        {
            DestroyGameObject(parent.gameObject);
        }
        else
        {
            Destroy(gObject, delay);
        }

    }
}
