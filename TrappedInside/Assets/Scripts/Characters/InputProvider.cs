using UnityEngine;

/// <summary>
/// Provides control input to other scripts.
/// The input may come from a player or from some other logic.
/// </summary>
public class InputProvider : MonoBehaviour
{
    private bool isControllable = true;
    private PlayerInput overrideControls; // Used unless 'isControllable'.

    /// <summary>
    /// Returns the current state of the input.
    /// </summary>
    public virtual PlayerInput GetInput()
    {
        return !isControllable
            ? overrideControls
            : new PlayerInput(
                fire1Pressed: Input.GetButtonDown("Fire1"),
                fire2Pressed: Input.GetButtonDown("Fire2"),
                fire2Active: Input.GetButton("Fire2"),
                jumpPressed: Input.GetButtonDown("Jump"),
                jumpReleased: Input.GetButtonUp("Jump"),
                horizontal: Input.GetAxis("Horizontal"),
                vertical: Input.GetAxis("Vertical"));
    }

    /// <summary>
    /// Overrides player input with the given input state.
    /// </summary>
    public void OverridePlayerControl(PlayerInput input)
    {
        isControllable = false;
        overrideControls = input;
    }

    /// <summary>
    /// Resumes using player input and disregards any previous input override.
    /// </summary>
    public void ResumePlayerControl()
    {
        isControllable = true;
    }
}
