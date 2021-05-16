using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Public interface and main implementation of the PressurePlate prefab.
/// </summary>
public class PressurePlateController : MonoBehaviour
{
    /// <summary>
    /// Called when the pressure plate is triggered.
    /// </summary>
    public UnityEvent onTriggered;

    /// <summary>
    /// Called when the pressure plate is released.
    /// </summary>
    public UnityEvent onReleased;

    #region MonoBehaviour overrides

    public void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggered?.Invoke();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        onReleased?.Invoke();
    }

    #endregion
}
