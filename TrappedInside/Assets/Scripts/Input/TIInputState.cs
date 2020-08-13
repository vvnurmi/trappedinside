using UnityEngine;

/// <summary>
/// Snapshot of player input state. Call <see cref="TIInputStateManager.CreateContext"/>
/// to create a context that will provide populated input states.
/// </summary>
public struct TIInputState
{
    public bool fire1Pressed;
    public bool fire2Pressed;
    public bool fire2Active;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool jumpActive;
    public float horizontal;
    public float vertical;
    public Vector2 uiNavigate;
    public bool uiSubmitPressed;
    public bool uiSubmitActive;
    public bool uiCancelPressed;
    public bool uiCancelActive;
}
