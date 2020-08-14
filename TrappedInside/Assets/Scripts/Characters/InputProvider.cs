/// <summary>
/// Provides control input to other scripts.
/// The input may come from a player or from some other logic.
/// </summary>
public interface IInputProvider
{
    /// <summary>
    /// Returns the current state of the input.
    /// </summary>
    TIInputState GetInput();
}
