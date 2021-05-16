using UnityEngine;

[CreateAssetMenu(fileName = "RaycastColliderConfig", menuName = "Collision/RaycastColliderConfig")]
public class RaycastColliderConfig : ScriptableObject
{
    [Tooltip("Which collision layers are to be hit.")]
    public LayerMask hitLayers;

    [Tooltip("Leeway around the box collider.")]
    public float skinWidth = 0.015f;

    [Tooltip("How far apart to shoot ground collision rays. The actual distance may differ slightly.")]
    public float approximateRaySpacing = 0.25f;
}
