using UnityEngine;

/// <summary>
/// Slides the title content out nicely using a 2D rect mask.
/// </summary>
[RequireComponent(typeof(UnityEngine.UI.RectMask2D))]
public class TitleSlide : MonoBehaviour
{
    [Tooltip("How many seconds it takes for the mask to move back to it's original place.")]
    public float slideSeconds = 1;

    [Tooltip("How many seconds to keep it visible after the mask slide has finished.")]
    public float visibleSeconds = 5;

    // Modified at start.
    private float startTime;
    private RectTransform mask;
    private RectTransform content;
    private Vector2 finalAnchoredPositionMask;
    private Vector2 finalAnchoredPositionContent;
    private Vector2 startDisplacement;

    private void Start()
    {
        Debug.Assert(transform.childCount == 1);

        startTime = Time.time;
        mask = transform as RectTransform;
        content = transform.GetChild(0) as RectTransform;
        finalAnchoredPositionMask = mask.anchoredPosition;
        finalAnchoredPositionContent = content.anchoredPosition;

        var camera = FindObjectOfType<Camera>();
        Debug.Assert(camera != null);
        startDisplacement = new Vector2(-camera.scaledPixelWidth, 0);

        UpdateMaskPosition();
    }

    private void Update()
    {
        UpdateMaskPosition();

        // After a while, disable the game object to signal parent object's script
        // ActivateChildrenSequentially to activate the next sibling.
        if (Time.time >= startTime + slideSeconds + visibleSeconds)
            ActivateNextChildSequentially();
    }

    private void UpdateMaskPosition()
    {
        var lerpParam = Mathf.InverseLerp(startTime, startTime + slideSeconds, Time.time);

        mask.anchoredPosition = Vector2.Lerp(
            finalAnchoredPositionMask + startDisplacement,
            finalAnchoredPositionMask,
            lerpParam);
        content.anchoredPosition = Vector2.Lerp(
            finalAnchoredPositionContent - startDisplacement,
            finalAnchoredPositionContent,
            lerpParam);
    }

        // After a while, disable the game object to signal parent object's script
        // ActivateChildrenSequentially to activate the next sibling.
        if (Time.time >= startTime + slideSeconds + visibleSeconds)
            gameObject.SetActive(false);
    }
}
