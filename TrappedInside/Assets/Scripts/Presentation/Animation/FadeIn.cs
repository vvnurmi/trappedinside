using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeIn : MonoBehaviour
{
    [Tooltip("How long to fade in.")]
    public float fadeSeconds = 2.0f;

    private AlphaFader alphaFader = new AlphaFader();

    private void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        alphaFader.StartFade(fadeSeconds, 0, 1, alpha =>
        {
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        });
    }

    private void Update()
    {
        if (!alphaFader.IsDone)
            alphaFader.Update();
    }
}
