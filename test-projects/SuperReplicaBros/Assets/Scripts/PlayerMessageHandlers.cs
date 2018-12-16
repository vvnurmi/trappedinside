using UnityEngine;

/// <summary>
/// Methods that handle messages sent via <see cref="GameObject.SendMessage(string)"/>.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerMessageHandlers : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    public void OutOfBounds()
    {
        player.KillInstantly();
    }
}
