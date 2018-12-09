using UnityEngine;

/// <summary>
/// Calls method named <see cref="message"/> for game objects that overlap
/// this 2D trigger.
/// </summary>
public class MessageTrigger : MonoBehaviour
{
    [Tooltip("Message to send to overlapping game objects.")]
    public string message = "OutOfBounds";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessage(message, SendMessageOptions.RequireReceiver);
    }
}
