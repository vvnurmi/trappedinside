using UnityEngine;

[CreateAssetMenu(fileName = "AssetBundleConfig", menuName = "System/AssetBundleConfig")]
public class AssetBundleConfig : ScriptableObject
{
    [Tooltip("Location of the asset bundle, e.g. 'http://foo.com/AssetBundles/music'")]
    public string uri;
}
