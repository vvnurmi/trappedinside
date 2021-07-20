using UnityEngine;

public class ColorSetter : MonoBehaviour
{
    public Color color;
    public GameObject coloredObject;

    void Start()
    {
        var spriteRenderer = coloredObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }
}
