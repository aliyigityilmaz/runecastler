using UnityEngine;

public class Cleanser : Building
{
    public override void OnPlaced(Tile tile)
    {
        base.OnPlaced(tile);
        CleanseArea(tile);
    }

    protected override void PerformBuildingAction()
    {
        // Cleanse doesn't have a continuous action, so no action here
    }

    private void CleanseArea(Tile centerTile)
    {
        Vector3 centerPosition = centerTile.transform.position;
        int centerX = Mathf.RoundToInt(centerPosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int centerZ = Mathf.RoundToInt(centerPosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        for (int x = centerX - gatherRadius; x <= centerX + gatherRadius; x++)
        {
            for (int z = centerZ - gatherRadius; z <= centerZ + gatherRadius; z++)
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