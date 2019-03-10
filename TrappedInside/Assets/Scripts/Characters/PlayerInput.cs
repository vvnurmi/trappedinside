public struct PlayerInput
{
    public readonly bool fire1;
    public readonly bool fire2;
    public readonly bool jumpPressed;
    public readonly bool jumpReleased;
    public readonly float horizontal;
    public readonly float vertical;

    public PlayerInput(
        bool fire1,
        bool fire2,
        bool jumpPressed,
        bool jumpReleased,
        float horizontal,
        float vertical)
    {
        this.fire1 = fire1;
        this.fire2 = fire2;
        this.jumpPressed = jumpPressed;
        this.jumpReleased = jumpReleased;
        this.horizontal = horizontal;
        this.vertical = vertical;
    }
}
