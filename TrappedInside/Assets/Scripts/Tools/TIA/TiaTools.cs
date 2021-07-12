using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Static methods that are of general use to other TIA classes.
/// </summary>
public static class TiaTools
{
    /// <summary>
    /// Prefix to use in some names to indicate an Addressable asset name.
    /// </summary>
    public const string AddressableNamePrefix = "addressable:";

    /// <summary>
    /// Finds an object by name and returns it. Returns null if the object wasn't found.
    /// Name prefix <see cref="AddressableNamePrefix"/> may be used to look for an Addressable asset.
    /// Otherwise will look for a game object under the TIA root object.
    /// </summary>
    public static async Task<TObject> FindObject<TObject>(ITiaActionContext context, string name) where TObject : Object
    {
        if (name.StartsWith(AddressableNamePrefix))
        {
            var addressableName = name.Substring(AddressableNamePrefix.Length);
            var loadTask = Addressables.LoadAssetAsync<TObject>(addressableName).Task;
            await loadTask;
            if (loadTask.Status != TaskStatus.RanToCompletion)
            {
                Debug.LogWarning($"{nameof(TiaSpeak)} loading addressable '{addressableName}' ended as {loadTask.Status}");
                return null;
            }
            return loadTask.Result;
        }

        // Note: We can only find GameObjects below, and TObject may not be a subclass of GameObject,
        // so even if an object by the name is found, the conversion may fail and result in null.
        Object temp = context.TiaRoot.FindChildByName(name);
        return temp as TObject;
    }
}
