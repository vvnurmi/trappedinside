using UnityEngine;

/// <summary>
/// Activates a certain object when a game object overlaps
/// this 2D trigger.
/// </summary>
public class ActivationTrigger : MonoBehaviour
{
    [Tooltip("To-be activation state.")]
    public bool activate = true;

    [Tooltip("Who to activate or deactivate.")]
    public GameObject target;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        target.SetActive(activate);
    }
}
