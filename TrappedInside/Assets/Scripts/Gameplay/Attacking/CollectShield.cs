using UnityEngine;

/// <summary>
/// Makes a shield get collected by the player on collision.
/// </summary>
public class CollectShield : MonoBehaviour
{
    [Tooltip("Root object of the hierarchy to delete on collection.")]
    public GameObject shieldRoot;

    [Tooltip("How many seconds until collection is possible.")]
    public float collectWarmupSeconds = 0.1f;

    private float collectWarmupEnd;

    #region MonoBehaviour overrides

    private void OnEnable()
    {
        collectWarmupEnd = Time.time + collectWarmupSeconds;    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time < collectWarmupEnd) return;
        if (!collision.CompareTag("Player")) return;

        shieldRoot.SetActive(false);
    }

    #endregion
}
