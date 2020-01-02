using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades an <see cref="Image"/> in and out.
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageFade : MonoBehaviour
{
    private enum FadeMode
    {
        None,
        In,
        Out,
    }

    [Tooltip("How many seconds to fade out.")]
    public float fadeOutSeconds = 1.0f;

    [Tooltip("How many seconds to fade in.")]
    public float fadeInSeconds = 0.5f;

    public bool IsFadeComplete => fadeMode == FadeMode.None;

    // Set once in the beginning.
    private Image image;

    // Modified during execution.
    private FadeMode fadeMode = FadeMode.None;
    private float fadeStartSeconds;

    public void FadeImageIn()
    {
        fadeMode = FadeMode.In;
        fadeStartSeconds = Time.unscaledTime;
    }

    public void FadeImageOut()
    {
        fadeMode = FadeMode.Out;
        fadeStartSeconds = Time.unscaledTime;
    }

    #region MonoBehaviour overrides

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (fadeMode == FadeMode.None) return;

        float lerpParam = 0;
        float alpha = 0;
        if (fadeMode == FadeMode.In)
        {
            lerpParam = Mathf.InverseLerp(fadeStartSeconds, fadeStartSeconds + fadeInSeconds, Time.unscaledTime);
            alpha = Mathf.Lerp(0, 1, lerpParam);
        }
        if (fadeMode == FadeMode.Out)
        {
            lerpParam = Mathf.InverseLerp(fadeStartSeconds, fadeStartSeconds + fadeOutSeconds, Time.unscaledTime);
            alpha = Mathf.Lerp(1, 0, lerpParam);
        }

        image.canvasRenderer.SetAlpha(alpha);

        if (lerpParam == 1.0f)
            fadeMode = FadeMode.None;
    }

    #endregion
}
