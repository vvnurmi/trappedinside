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
        if (parent != null && IsOnSameLayer(parent.gameObject))
        {
            DestroyGameObject(parent.gameObject);
        }
        else
        {
            var collider = GetComponent<BoxCollider2D>();
            if(collider != null)
                collider.enabled = false;
                
            Destroy(gObject, delay);
        }

    }

    private bool IsOnSameLayer(GameObject other) => gameObject.layer == other.layer;
}
