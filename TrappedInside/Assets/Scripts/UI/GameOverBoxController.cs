using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameOverBoxController : MonoBehaviour
{
    [Tooltip("Quips to display in the game over box.")]
    [TextArea]
    public string[] gameOverQuips;

    private bool isAcknowledged;

    private void Awake()
    {
        var quip = GameObject.FindGameObjectWithTag("GameOverQuip");
        Debug.Assert(quip != null);
        var quipText = quip.GetComponent<Text>();
        quipText.text = gameOverQuips[Random.Range(0, gameOverQuips.Length)];
    }

    private void FixedUpdate()
    {
        if (isAcknowledged)
            UIController.Instance.RestartLevel();
    }

    public void InputEvent_Submit(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();
        isAcknowledged |= value >= 0.5f;
    }
}
