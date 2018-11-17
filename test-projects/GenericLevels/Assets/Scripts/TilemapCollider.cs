using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Collider that covers the painted tiles of a TileMap.
/// </summary>
public class TilemapCollider : MonoBehaviour
{
    [Tooltip("Create colliders for tiles in this tilemap.")]
    public Tilemap tilemap;

    private void Start()
    {
        Debug.Assert(tilemap != null);

        var tileAnchor = new Vector2(tilemap.tileAnchor.x, tilemap.tileAnchor.y);
        var bounds = tilemap.cellBounds;
        Debug.Assert(bounds.zMin == 0 && bounds.zMax == 1);
        for (int y = bounds.yMin; y < bounds.yMax; y++)
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile == null) continue;
                var box = gameObject.AddComponent<BoxCollider2D>();
                box.offset = new Vector2(x, y) + tileAnchor;
                box.size = Vector2.one;
            }
    }
}
