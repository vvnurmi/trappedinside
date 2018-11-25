using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Tooltip("What to display when the trigger is entered for the first time.")]
    public GameObject dialog;

    [Tooltip("Where to place dialog")]
    public Vector2 dialogPosition;

    private bool wasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (wasTriggered) return;

        wasTriggered = true;
        GameState.ShowDialog(dialog, dialogPosition);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameState.HideDialog();
    }
}
