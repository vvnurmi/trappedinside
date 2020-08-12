using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager
{
    private PlayerInput inputState;

    /// <summary>
    /// Call this exactly once every update. That way you'll get correct values for the event flags
    /// in <see cref="PlayerInput"/>.
    /// </summary>
    public PlayerInput GetStateAndResetEventFlags()
    {
        var currentInput = inputState;
        inputState.fire1Pressed = false;
        inputState.fire2Pressed = false;
        inputState.jumpPressed = false;
        inputState.jumpReleased = false;
        return currentInput;
    }

    public void InputEvent_Move(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        inputState.horizontal = value.x;
        inputState.vertical = value.y;
    }

    public void InputEvent_Jump(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();
        inputState.jumpPressed |= !inputState.jumpActive && value >= 0.5f;
        inputState.jumpReleased |= inputState.jumpActive && value < 0.5f;
        inputState.jumpActive = value >= 0.5f;
    }

    public void InputEvent_Shield(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();
        inputState.fire2Pressed |= !inputState.fire2Active && value >= 0.5f;
        inputState.fire2Active = value >= 0.5f;
    }
}

/// <summary>
/// Snapshot of player input state. Use <see cref="PlayerInputManager"/> to handle
/// input events and event flags.
/// </summary>
public struct PlayerInput
{
    public bool fire1Pressed;
    public bool fire2Pressed;
    public bool fire2Active;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool jumpActive;
    public float horizontal;
    public float vertical;
}
