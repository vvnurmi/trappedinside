using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [Tooltip("The parallax will follow this object's movement.")]
    public GameObject follow;
    [Tooltip("Bigger scale makes the parallax move slower.")]
    public float speedScale = 100;
    [Tooltip("A game object with a Quad component. Because lazy programmer didn't do it in code.")]
    public GameObject quadTemplate;
    [Tooltip("Material containing the parallax texture. Lazy programmer didn't do it in code.")]
    public Material material;
    private Canvas canvas;
    private MeshFilter mesh;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        Debug.Assert(canvas != null, "BackgroundParallax needs a Canvas child because lazy programmer didn't create it in code");

        var quad = Instantiate(quadTemplate, canvas.transform);
        mesh = quad.GetComponent<MeshFilter>();
        Debug.Assert(mesh != null, "Quad template is missing a mesh filter component");
        var renderer = quad.GetComponent<MeshRenderer>();
        Debug.Assert(renderer != null, "Quad template is missing a mesh renderer component");
        renderer.material = material;
    }

    private void Update()
    {
        var pos = follow != null ? follow.transform.position / speedScale : Vector3.zero;
        mesh.mesh.uv = new[]
        {
            new Vector2(pos.x + 0, pos.y + 0),
            new Vector2(pos.x + 1, pos.y + 1),
            new Vector2(pos.x + 1, pos.y + 0),
            new Vector2(pos.x + 0, pos.y + 1),
        };
    }
}
