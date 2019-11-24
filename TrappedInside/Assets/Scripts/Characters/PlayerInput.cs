public struct PlayerInput
{
    public readonly bool fire1;
    public readonly bool fire2Pressed;
    public readonly bool fire2Released;
    public readonly bool jumpPressed;
    public readonly bool jumpReleased;
    public readonly float horizontal;
    public readonly float vertical;

    public PlayerInput(
        bool fire1,
        bool fire2Pressed,
        bool fire2Released,
        bool jumpPressed,
        bool jumpReleased,
        float horizontal,
        float vertical)
    {
        this.fire1 = fire1;
        this.fire2Pressed = fire2Pressed;
        this.fire2Released = fire2Released;
        this.jumpPressed = jumpPressed;
        this.jumpReleased = jumpReleased;
        this.horizontal = horizontal;
        this.vertical = vertical;
    }
}
