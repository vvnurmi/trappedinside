/// <summary>
/// Snapshot of player input state. Use <see cref="TIInputStateManager"/> to handle
/// input events and event flags.
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
}
