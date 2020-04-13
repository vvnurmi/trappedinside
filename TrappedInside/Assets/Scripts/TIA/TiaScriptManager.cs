using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary>
/// Handles access to <see cref="TiaScript"/> instances.
/// 
/// The name of a TIA script is its relative asset path in the folder that's
/// labeled in Addressable Assets as "TiaScript".
/// </summary>
public class TiaScriptManager : MonoBehaviour
{
    private static GameObject host;

    private Task<IList<TextAsset>> loadTask;
    private Task<IList<IResourceLocation>> locationTask;
    private Dictionary<string, string> scriptYamls; // TIA script name => TIA script as YAML

    public static TiaScriptManager Instance => host.GetComponent<TiaScriptManager>();

    public async Task<TiaScript> Get(string name)
    {
        if (!await TryEnsureScriptsLoaded())
            return TiaScript.Empty;

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

    /// <summary>
    /// Awaits until scripts have been loaded.
    /// Returns true on success and false on failure. 
    /// </summary>
    private async Task<bool> TryEnsureScriptsLoaded()
    {
        if (scriptYamls != null) return true;

        await locationTask;
        if (locationTask.Status != TaskStatus.RanToCompletion)
        {
            Debug.LogWarning($"TIA script discovery is {locationTask.Status}");
            return false;
        }
        var locations = locationTask.Result;

        await loadTask;
        if (loadTask.Status != TaskStatus.RanToCompletion)
        {
            Debug.LogWarning($"TIA script loading is {loadTask.Status}");
            return false;
        }
        scriptYamls = locations
            .Zip(loadTask.Result, (location, textAsset) => (location, textAsset))
            .ToDictionary<(IResourceLocation, TextAsset), string, string>(
                x => GetScriptName(x.Item1),
                x => x.Item2.text);

        // Remove this debug logging later when there's more scripts and TIA feels more stable.
        foreach (var x in scriptYamls)
            Debug.Log($"Found TIA script '{x.Key}'");

        return true;
    }

    private string GetScriptName(IResourceLocation resourceLocation)
    {
        // ResourceLocation will be something like "Assets/Data/TiaScripts/MyFolder/Test1.yaml"
        // and we'll shorten it to "MyFolder/Test1"
        var path = resourceLocation.PrimaryKey;
        var pathPrefix = "Assets/Data/TiaScripts/";
        var pathSuffix = ".yaml";
        Debug.Assert(path.StartsWith(pathPrefix) && path.EndsWith(pathSuffix),
            $"Script path is not of expected form '{pathPrefix}XXX{pathSuffix}': '{path}'");
        return path.Substring(pathPrefix.Length, path.Length - pathPrefix.Length - pathSuffix.Length);
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
        locationTask = Addressables.LoadResourceLocationsAsync("TiaScript", typeof(TextAsset)).Task;
        var asyncOperation = Addressables.LoadAssetsAsync<TextAsset>("TiaScript", null);
        loadTask = asyncOperation.Task;
    }

    #endregion
}
