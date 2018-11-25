using UnityEngine;

public static class GameState
{
    private static GameObject liveDialog;

    /// <summary>
    /// Pauses the game, displays a dialog, and waits for player acknowledgement.
    /// </summary>
    public static void ShowDialog(GameObject dialog, Vector2 dialogPosition)
    {
        //Time.timeScale = 0;
        var dialogPos = new Vector3(dialogPosition.x, dialogPosition.y, 0);
        liveDialog = Object.Instantiate(dialog, dialogPos, Quaternion.identity);
    }

    public static void HideDialog()
    {
        if (liveDialog == null) return;

        Object.Destroy(liveDialog);
        liveDialog = null;
        //Time.timeScale = 1;
    }
}
