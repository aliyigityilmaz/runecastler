using UnityEngine;

public class Woodcutter : Building
{
    public int gatherRadius = 2; // Maximum radius to gather resources
    public int gatherAmountPerTick = 1; // Amount of wood gathered per tick
    public float gatherInterval = 5f; // Time between resource gathering in seconds

    private float gatherTimer = 0f;

    void Update()
    {
        gatherTimer -= Time.deltaTime;

        if (gatherTimer <= 0f)
        {
            GatherWood();
            gatherTimer = gatherInterval; // Reset the timer
        }
    }

    void GatherWood()
    {
        WorldGenerator worldGenerator = FindObjectOfType<WorldGenerator>();
        ResourceSystem resourceSystem = FindObjectOfType<ResourceSystem>();

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