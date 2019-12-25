using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class PlayMusicFromAssetBundle : MonoBehaviour
{
    [Tooltip("Asset bundle to load the audio clip from.")]
    public AssetBundleConfig assetBundle;

    [Tooltip("Name of the audio clip to load from the asset bundle.")]
    public string audioClipName;

    private void Start()
    {
        var audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayAudioClip(audioSource, assetBundle.uri, audioClipName));
    }

    private static IEnumerator PlayAudioClip(AudioSource audioSource, string assetBundleUri, string audioClipName)
    {
        var request = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleUri, 0);
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError($"Couldn't load asset bundle '{assetBundleUri}': {request.error}");
            yield break;
        }

        var bundle = DownloadHandlerAssetBundle.GetContent(request);
        var audioClip = bundle.LoadAsset<AudioClip>(audioClipName);
        if (audioClip == null)
        {
            Debug.LogError($"Couldn't load audio clip '{audioClipName}' from asset bundle '{assetBundleUri}'");
            yield break;
        }

        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
