public struct PlayerInput
{
    public readonly bool fire1Pressed;
    public readonly bool fire2Active;
    public readonly bool jumpPressed;
    public readonly bool jumpReleased;
    public readonly float horizontal;
    public readonly float vertical;

    public PlayerInput(
        bool fire1Pressed,
        bool fire2Active,
        bool jumpPressed,
        bool jumpReleased,
        float horizontal,
        float vertical)
    {
        this.fire1Pressed = fire1Pressed;
        this.fire2Active = fire2Active;
        this.jumpPressed = jumpPressed;
        this.jumpReleased = jumpReleased;
        this.horizontal = horizontal;
        this.vertical = vertical;
    }
}
