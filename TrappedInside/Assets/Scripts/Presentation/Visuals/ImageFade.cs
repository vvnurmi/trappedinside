using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades an <see cref="Image"/> in and out.
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageFade : MonoBehaviour
{
    [Tooltip("How many seconds to fade the image out.")]
    public float fadeOutSeconds = 1.0f;

    [Tooltip("How many seconds to fade the image in.")]
    public float fadeInSeconds = 0.5f;

    public bool IsFadeComplete => alphaFader.IsDone;

    // Set once in the beginning.
    private Image image;
    private AlphaFader alphaFader = new AlphaFader();

    public void FadeImageIn()
    {
        alphaFader.StartFade(fadeInSeconds, 0, 1, image.canvasRenderer.SetAlpha);
    }

    public void FadeImageOut()
    {
        alphaFader.StartFade(fadeOutSeconds, 1, 0, image.canvasRenderer.SetAlpha);
    }

    #region MonoBehaviour overrides

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (!alphaFader.IsDone)
            alphaFader.Update();
    }

    #endregion
}
