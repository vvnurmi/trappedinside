using System;

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

    public PlayerInput(
        bool fire1Pressed,
        bool fire2Pressed,
        bool fire2Active,
        bool jumpPressed,
        bool jumpReleased,
        float horizontal,
        float vertical)
    {
        this.fire1Pressed = fire1Pressed;
        this.fire2Pressed = fire2Pressed;
        this.fire2Active = fire2Active;
        this.jumpPressed = jumpPressed;
        this.jumpReleased = jumpReleased;
        this.jumpActive = jumpPressed; // !!!
        this.horizontal = horizontal;
        this.vertical = vertical;
    }

    internal void ResetEventFlags()
    {
        fire1Pressed = false;
        fire2Pressed = false;
        jumpPressed = false;
        jumpReleased = false;
    }
}
