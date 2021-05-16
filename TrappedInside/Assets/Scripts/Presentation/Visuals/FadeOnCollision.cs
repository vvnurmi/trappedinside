using UnityEngine;
using Scalar = TI.Scalar;

/// <summary>
/// Fades the game object on collision. Good for creating secret areas whose
/// cover fades away when the player goes there.
/// </summary>
public class FadeOnCollision : MonoBehaviour
{
    [Tooltip("Object whose overlap will control the fade.")]
    public GameObject fadeTrigger;

    [Tooltip("Game object to fade.")]
    public GameObject fader;

    [Tooltip("How fast to fade in and out.")]
    public float fadeSeconds = 1;

    private float alpha = 1;
    private float alphaTarget = 1;

    #region MonoBehaviour overrides

    private void Update()
    {
        if (alpha == alphaTarget) return;

        alpha = Scalar.LerpTowards(alpha, alphaTarget, Time.deltaTime / fadeSeconds);

        var renderer = fader.GetComponent<Renderer>();
        var newColor = renderer.material.color;
        newColor.a = alpha;
        renderer.material.color = newColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == fadeTrigger)
            FadeOut();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == fadeTrigger)
            FadeIn();
    }

    #endregion

    private void FadeOut()
    {
        alphaTarget = 0;
    }

    private void FadeIn()
    {
        alphaTarget = 1;
    }
}
