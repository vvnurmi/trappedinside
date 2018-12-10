using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameObject liveDialog;

    /// <summary>
    /// Pauses the game, displays a dialog, and waits for player acknowledgement.
    /// </summary>
    public static void ShowDialog(GameObject dialog, Vector2 dialogPosition)
    {
        Time.timeScale = 0;
        var dialogPos = new Vector3(dialogPosition.x, dialogPosition.y, 0);
        liveDialog = Instantiate(dialog, dialogPos, Quaternion.identity);
    }

    public static void HideDialog()
    {
        Debug.Assert(liveDialog != null);
        if (liveDialog != null) Destroy(liveDialog);
        liveDialog = null;
        Time.timeScale = 1;
    }

    private void Update()
    {
        // If a dialog is open, close it on player acknowledgement.
        if (liveDialog != null)
        {
            var isAcknowledged = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump");
            if (isAcknowledged)
                HideDialog();
        }
    }
}
