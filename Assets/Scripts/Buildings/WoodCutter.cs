using UnityEngine;

public class Woodcutter : Building
{
    protected override void PerformBuildingAction()
    {
        GatherWood();
    }

    private void GatherWood()
    {
        int centerX = Mathf.RoundToInt(transform.position.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int centerZ = Mathf.RoundToInt(transform.position.z / (worldGenerator.tileSize + worldGenerator.spacing));

        for (int x = centerX - gatherRadius; x <= centerX + gatherRadius; x++)
        {
            for (int z = centerZ - gatherRadius; z <= centerZ + gatherRadius; z++)
            {
                if (x >= 0 && x < worldGenerator.worldWidth && z >= 0 && z < worldGenerator.worldHeight)
                {
                    Tile tile = worldGenerator.tiles[x, z];
                    ResourceObject resource = tile.GetComponentInChildren<ResourceObject>();

                    if (resource != null && resource.resourceType == ResourceObject.ResourceType.Wood)
                    {
                        resource.GatherResource(gatherAmountPerTick);
                        resourceSystem.AddResource(ResourceType.Wood, gatherAmountPerTick);
                    }
                }
            }
        }
    }
}