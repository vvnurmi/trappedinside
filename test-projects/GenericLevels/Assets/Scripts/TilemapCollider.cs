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
        {
            // To save on the number of box colliders, remember the box immediately
            // on the left and expand it instead of creating a new box for each tile.
            BoxCollider2D leftBox = null;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile == null)
                {
                    leftBox = null;
                    continue;
                }

                if (leftBox != null)
                {
                    // Expand box collider on the left.
                    leftBox.offset += Vector2.right / 2;
                    leftBox.size += Vector2.right;
                }
                else
                {
                    // Create a new box collider.
                    leftBox = gameObject.AddComponent<BoxCollider2D>();
                    leftBox.offset = new Vector2(x, y) + tileAnchor;
                    leftBox.size = Vector2.one;
                }
            }
        }
    }
}
