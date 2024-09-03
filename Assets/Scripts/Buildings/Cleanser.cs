using UnityEngine;

public class Cleanser : Building
{
    public int cleanseRadius = 1; // Temizleyici binanýn etki alaný

    public void CleanseArea(Tile centerTile, WorldGenerator worldGenerator)
    {
        Vector3 centerPosition = centerTile.transform.position;
        int centerX = Mathf.RoundToInt(centerPosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int centerZ = Mathf.RoundToInt(centerPosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        for (int x = centerX - cleanseRadius; x <= centerX + cleanseRadius; x++)
        {
            for (int z = centerZ - cleanseRadius; z <= centerZ + cleanseRadius; z++)
            {
                if (x >= 0 && x < worldGenerator.worldWidth && z >= 0 && z < worldGenerator.worldHeight)
                {
                    Tile tile = worldGenerator.tiles[x, z];
                    if (!tile.isCleansed)
                    {
                        worldGenerator.CleanseTile(x, z);
                    }
                }
            }
        }
    }
}