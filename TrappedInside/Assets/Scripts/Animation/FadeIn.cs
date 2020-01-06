using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeIn : MonoBehaviour
{
    [Tooltip("How long to fade in.")]
    public float fadeSeconds = 2.0f;

    private float startTime;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        startTime = Time.time;
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetAlpha(0);
    }

    private void Update()
    {
        var alpha = Mathf.InverseLerp(startTime, startTime + fadeSeconds, Time.time);
        SetAlpha(alpha);
    }

    private void SetAlpha(float alpha)
    {
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
