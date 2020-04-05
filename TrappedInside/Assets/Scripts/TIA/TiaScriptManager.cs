using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Handles access to <see cref="TiaScript"/> instances.
/// </summary>
public class TiaScriptManager : MonoBehaviour
{
    private static GameObject host;

    private Task loadTask;
    private Dictionary<string, TiaScriptText> scriptTexts;

    public static TiaScriptManager Instance => host.GetComponent<TiaScriptManager>();

    public async Task<TiaScript> Get(string name)
    {
        await loadTask;
        Debug.Assert(scriptTexts != null, "TIA script loading failed");
        Debug.Assert(scriptTexts.ContainsKey(name), $"TIA script '{name}' not found");
        return Get(scriptTexts[name]);
    }

    public TiaScript Get(TiaScriptText scriptText)
    {
        var script = TiaScript.Read(scriptText.text);
        return script;
    }

    #region MonoBehaviour overrides

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Assert(host == null);
        host = new GameObject("Tia Script Manager", typeof(TiaScriptManager));
        DontDestroyOnLoad(host);
    }

    private void Awake()
    {
        loadTask = Addressables.LoadAssetsAsync<TiaScriptText>("TiaScript", null)
        .Task.ContinueWith(scripts =>
        {
            scriptTexts = scripts.Result.ToDictionary(
                script => TiaScript.Read(script.text).ScriptName,
                script => script);
            foreach (var x in scriptTexts)
                Debug.Log("!!! Found " + x.Key);
        });
    }

    #endregion
}
