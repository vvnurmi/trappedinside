using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles title screen name input functionality.
/// CUsed by the name input <see cref="TMP_InputField"/> to signal
/// <see cref="ActivateChildrenSequentially"/> that the next child
/// object is to be activated.
/// </summary>
public class NameInputDoneCheck : MonoBehaviour, ISequentialChild
{
    public bool isDone = false;

    public void OnEndEdit()
    {
        isDone = true;

        // Explicitly take focus away from the input field.
        EventSystem.current.SetSelectedGameObject(null);
    }

    public bool IsDone() => isDone;

    private void Start()
    {
        var inputField = GetComponentInChildren<TMP_InputField>();

        // Hack: Hide caret after input ends. Otherwise it keeps blinking.
        inputField.onDeselect.AddListener(str => inputField.caretWidth = 0);

        // Help the player by focusing the input field automatically.
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }
}
