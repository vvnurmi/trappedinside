using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static partial class TiaMethods
{
    public static bool testFlag;
    public static string testString1, testString2;
    public static void SetTestFlagToTrue() => testFlag = true;
    public static void SetTestString(string value) => testString1 = value;
    public static void SetTestStrings(string value1, string value2) => (testString1, testString2) = (value1, value2);

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
    /// component and makes it play the script <paramref name="tiaScriptName"/>. Returns true if
    /// the player and script was found and playing started successfully.
    /// </summary>
    public static async Task<bool> TryPlayScript(string tiaPlayerName, string tiaScriptName)
    {
        var tiaPlayer = Object.FindObjectsOfType<TiaPlayer>()
            .FirstOrDefault(player => player.gameObject.name == tiaPlayerName);
        if (tiaPlayer == null)
        {
            TiaDebug.Log($"There's no TIA player named '{tiaPlayerName}'");
            return false;
        }

        var script = await TiaScriptManager.Instance.Get(tiaScriptName);
        if (script == null)
        {
            TiaDebug.Log($"There's no TIA script named '{tiaScriptName}'");
            return false;
        }

        tiaPlayer.Play(script);
        return true;
    }
}
