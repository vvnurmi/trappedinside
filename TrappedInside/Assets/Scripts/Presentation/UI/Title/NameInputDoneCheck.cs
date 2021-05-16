using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles title screen name input functionality.
/// Used by the name input <see cref="TMP_InputField"/> to signal
/// <see cref="ActivateChildrenSequentially"/> that the next child
/// object is to be activated.
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class NameInputDoneCheck : MonoBehaviour
{
    [Tooltip("How fast to fade out after name input is finished.")]
    public float fadeOutSeconds = 1.0f;

    private AlphaFader alphaFader;

    public void OnEndEdit()
    {
        // Fade the UI out.
        alphaFader = new AlphaFader();
        var renderers = GetComponentsInChildren<CanvasRenderer>();
        alphaFader.StartFade(fadeOutSeconds, 1, 0, alpha =>
        {
            foreach (var renderer in renderers)
                renderer.SetAlpha(alpha);
        });

        // Explicitly take focus away from the input field.
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Start()
    {
        var inputField = GetComponentInChildren<TMP_InputField>();

        // Hack: Hide caret after input ends. Otherwise it keeps blinking.
        inputField.onDeselect.AddListener(str => inputField.caretWidth = 0);

        // Help the player by focusing the input field automatically.
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }

    private void Update()
    {
        // Nothing to do unless we're fading out.
        if (alphaFader == null) return;

        alphaFader.Update();

        // When we've faded out, deactivate self to signal
        // ActivateChildrenSequentially to activate the next child.
        if (alphaFader.IsDone)
            gameObject.SetActive(false);
    }
}
