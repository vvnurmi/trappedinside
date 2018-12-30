using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
    public static BoundsInt GetTightCellBounds(this Tilemap tilemap)
    {
        var looseBounds = tilemap.cellBounds;
        var min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        var max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        for (int z = looseBounds.zMin; z < looseBounds.zMax; z++)
            for (int y = looseBounds.yMin; y < looseBounds.yMax; y++)
                for (int x = looseBounds.xMin; x < looseBounds.xMax; x++)
                    if (tilemap.GetTile(new Vector3Int(x, y, z)) != null)
                    {
                        min = Vector3Int.Min(min, new Vector3Int(x, y, z));
                        max = Vector3Int.Max(max, new Vector3Int(x, y, z));
                    }
        return new BoundsInt(
            xMin: min.x,
            yMin: min.y,
            zMin: min.z,
            sizeX: max.x - min.x + 1,
            sizeY: max.y - min.y + 1,
            sizeZ: max.z - min.z + 1);
    }
}
