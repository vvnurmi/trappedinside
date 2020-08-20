public static partial class TiaMethods
{
    public static bool testFlag;
    public static void SetTestFlagToTrue() => testFlag = true;

    public static void DisablePlayerInput() =>
        TIInputStateManager.instance.DisablePlayerInput();

    public static void EnablePlayerInput() =>
        TIInputStateManager.instance.EnablePlayerInput();
}
