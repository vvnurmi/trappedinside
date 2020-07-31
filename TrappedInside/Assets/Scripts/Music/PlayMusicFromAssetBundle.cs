using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayMusicFromAssetBundle : MonoBehaviour
{
    [Tooltip("Asset bundle to load the audio clip from.")]
    public AssetBundleConfig assetBundle;

    [Tooltip("Name of the audio clip to load from the asset bundle.")]
    public string audioClipName;

    [Tooltip("Audio source to play the music.")]
    public AudioSource audioSource;

    private static string cachedBundleName;
    private static AssetBundle cachedBundle;
    private static Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Start()
    {
        var instantiatedAudioSource = Instantiate(audioSource, transform);
        StartCoroutine(PlayAudioClip(instantiatedAudioSource, assetBundle.uri, audioClipName));
    }

    private static IEnumerable EnsureAssetBundleIsCached(string assetBundleUri)
    {
        if (cachedBundleName == assetBundleUri)
            yield break;

        Debug.Assert(cachedBundleName == null || cachedBundleName == assetBundleUri,
            "Only a single asset bundle is currently supported");

        var request = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleUri, 0);
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError($"Couldn't load asset bundle '{assetBundleUri}': {request.error}");
            yield break;
        }

        var bundle = DownloadHandlerAssetBundle.GetContent(request);
        if (bundle == null)
        {
            Debug.LogError($"Couldn't get content from asset bundle '{assetBundleUri}'");
            yield break;
        }

        if (cachedBundle != null)
        {
            cachedBundle.Unload(true);
            audioClips.Clear();
        }

        cachedBundle = bundle;
        cachedBundleName = assetBundleUri;
    }

    private static IEnumerable EnsureAudioClipIsCached(string audioClipName)
    {
        if (audioClips.ContainsKey(audioClipName))
            yield break;

        var audioClip = cachedBundle.LoadAsset<AudioClip>(audioClipName);
        if (audioClip == null)
        {
            Debug.LogError($"Couldn't load audio clip '{audioClipName}' from asset bundle");
            yield break;
        }
        audioClips.Add(audioClipName, audioClip);
    }

    private static void PlayAudioClip(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private static IEnumerator PlayAudioClip(AudioSource audioSource, string assetBundleUri, string audioClipName)
    {
        foreach (var x in EnsureAssetBundleIsCached(assetBundleUri))
            yield return x;

        foreach (var x in EnsureAudioClipIsCached(audioClipName))
            yield return x;

        PlayAudioClip(audioSource, audioClips[audioClipName]);
    }
}
