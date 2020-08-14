using UnityEngine;
using UnityEngine.UI;

public class GameOverBoxController : MonoBehaviour
{
    [Tooltip("Quips to display in the game over box.")]
    [TextArea]
    public string[] gameOverQuips;

    private ITIInputContext inputContext;
    private bool isAcknowledged;

    private void Start()
    {
        inputContext = TIInputStateManager.instance.CreateContext();
    }

    private void OnDestroy()
    {
        inputContext?.Dispose();
    }

    private void Awake()
    {
        var quip = GameObject.FindGameObjectWithTag("GameOverQuip");
        Debug.Assert(quip != null);
        var quipText = quip.GetComponent<Text>();
        quipText.text = gameOverQuips[Random.Range(0, gameOverQuips.Length)];
    }

    private void FixedUpdate()
    {
        var inputState = inputContext.GetStateAndResetEventFlags();
        if (inputState.uiSubmitPressed)
            UIController.Instance.RestartLevel();
    }
}
