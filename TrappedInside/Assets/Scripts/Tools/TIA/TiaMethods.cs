public static partial class TiaMethods
{
    public static bool testFlag;
    public static void SetTestFlagToTrue() => testFlag = true;

    /// <summary>
    /// Disables controls of the player character.
    /// </summary>
    public static void DisablePlayerInput() =>
        TIInputStateManager.instance.DisablePlayerInput();

    /// <summary>
    /// Enables controls of the player character.
    /// </summary>
    public static void EnablePlayerInput() =>
        TIInputStateManager.instance.EnablePlayerInput();
}
