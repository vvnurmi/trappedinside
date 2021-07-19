using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Static methods that can be called from a TIA script by a <see cref="TiaInvoke"/> action.
/// </summary>
public static partial class TiaMethods
{
#if UNITY_EDITOR
    // For automated testing only.
    public static bool testFlag;
    public static string testString1, testString2;
    public static void SetTestFlagToTrue() => testFlag = true;
    public static void SetTestString(string value) => testString1 = value;
    public static void SetTestStrings(string value1, string value2) => (testString1, testString2) = (value1, value2);
#endif

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

    /// <summary>
    /// Finds an object called <paramref name="tiaPlayerName"/> that has a <see cref="TiaPlayer"/>
    /// component and makes it play a TIA script that can be found by the Addressable name
    /// <paramref name="tiaScriptAddressableName"/>. Returns true if the player and script were
    /// found and playing started successfully.
    /// 
    /// Name prefix <see cref="TiaTools.AddressableNamePrefix"/> may be used to look for an Addressable asset.
    /// </summary>
    public static async Task<bool> TryPlayScript(ITiaActionContext context, string tiaPlayerName, string tiaScriptName)
    {
        var tiaPlayer = Object.FindObjectsOfType<TiaPlayer>()
            .FirstOrDefault(player => player.gameObject.name == tiaPlayerName);
        if (tiaPlayer == null)
        {
            TiaDebug.Log($"There's no TIA player named '{tiaPlayerName}'");
            return false;
        }

        var script = await TiaTools.FindObject<TiaScript>(context, tiaScriptName);
        if (script == null)
        {
            TiaDebug.Log($"There's no TIA script named '{tiaScriptName}'");
            return false;
        }

        tiaPlayer.Play(script);
        return true;
    }
}
