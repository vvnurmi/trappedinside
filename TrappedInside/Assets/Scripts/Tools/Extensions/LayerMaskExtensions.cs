using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask mask, int layer) =>
        (mask & (1 << layer)) != 0;
}
