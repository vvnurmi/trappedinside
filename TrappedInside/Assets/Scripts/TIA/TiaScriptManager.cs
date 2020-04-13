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

    private Task<IList<TextAsset>> loadTask;
    private Dictionary<string, string> scriptYamls; // TIA script name => TIA script as YAML

    public static TiaScriptManager Instance => host.GetComponent<TiaScriptManager>();

    public async Task<TiaScript> Get(string name)
    {
        await loadTask;
        if (loadTask.Status != TaskStatus.RanToCompletion)
        {
            Debug.LogWarning($"TIA script loading is {loadTask.Status}");
            return TiaScript.Empty;
        }

        scriptYamls = loadTask.Result.ToDictionary(
            textAsset => TiaScript.Read(textAsset.text).ScriptName,
            textAsset => textAsset.text);

        if (scriptYamls.TryGetValue(name, out string scriptYaml))
            return GetFromYaml(scriptYaml);

        Debug.LogWarning($"TIA script '{name}' not found, using empty script instead");
        return TiaScript.Empty;
    }

    public TiaScript GetFromYaml(string scriptYaml)
    {
        var script = TiaScript.Read(scriptYaml);
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
        var asyncOperation = Addressables.LoadAssetsAsync<TextAsset>("TiaScript", null);
        loadTask = asyncOperation.Task;
    }

    #endregion
}
