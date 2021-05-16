using System.Collections;
using UnityEngine;

public class PreloadAssetBundle : MonoBehaviour, ISequentialChild
{
    [Tooltip("Asset bundle to preload.")]
    public AssetBundleConfig assetBundle;

    private bool isDone;

    public bool IsDone() => isDone;

    private IEnumerator Start()
    {
        var enumerable = PlayMusicFromAssetBundle.EnsureAssetBundleIsCached(assetBundle.uri);
        yield return StartCoroutine(enumerable.GetEnumerator());
        isDone = true;
    }
}
