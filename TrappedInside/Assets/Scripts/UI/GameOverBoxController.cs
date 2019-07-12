using UnityEngine;
using UnityEngine.UI;

public class GameOverBoxController : MonoBehaviour
{
    [Tooltip("Quips to display in the game over box.")]
    [TextArea]
    public string[] gameOverQuips;

    private void Awake()
    {
        var quip = GameObject.FindGameObjectWithTag("GameOverQuip");
        Debug.Assert(quip != null);
        var quipText = quip.GetComponent<Text>();
        quipText.text = gameOverQuips[Random.Range(0, gameOverQuips.Length)];
    }

    private void FixedUpdate()
    {
        var prompted =
            Input.GetButtonDown("Fire1") ||
            Input.GetButtonDown("Fire2") ||
            Input.GetButtonDown("Jump");
        if (prompted)
            UIController.Instance.RestartLevel();
    }
}
