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

    [Tooltip("How many seconds to fade the image out.")]
    public float fadeOutSeconds = 1.0f;

    [Tooltip("How many seconds to fade the image in.")]
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

        // Make cover image transparent right away to avoid unintended momentary screen disappearance.
        image.canvasRenderer.SetAlpha(0);
    }

    public void FadeImageOut()
    {
        fadeMode = FadeMode.Out;
        fadeStartSeconds = Time.unscaledTime;

        // Make cover image opaque right away to avoid unintended momentary screen appearance.
        image.canvasRenderer.SetAlpha(1);
    }

    #region MonoBehaviour overrides

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (fadeMode == FadeMode.None) return;

        (float alpha, float lerpParam) = GetAlphaAndLerpParam();

        image.canvasRenderer.SetAlpha(alpha);

        if (lerpParam == 1.0f)
            fadeMode = FadeMode.None;
    }

    #endregion

    private (float alpha, float lerpParam) GetAlphaAndLerpParam()
    {
        switch (fadeMode)
        {
            case FadeMode.In:
                return GetAlphaAndLerpParam(fadeInSeconds, 0, 1);
            case FadeMode.Out:
                return GetAlphaAndLerpParam(fadeOutSeconds, 1, 0);
            default:
                Debug.LogAssertion($"Unhandled {nameof(FadeMode)} {fadeMode}");
                return (0, 0);
        }
    }

    private (float alpha, float lerpParam) GetAlphaAndLerpParam(float fadeSeconds, float alphaBegin, float alphaEnd)
    {
        var lerpParam = Mathf.InverseLerp(fadeStartSeconds, fadeStartSeconds + fadeSeconds, Time.unscaledTime);
        var alpha = Mathf.Lerp(alphaBegin, alphaEnd, lerpParam);
        return (alpha, lerpParam);
    }
}
