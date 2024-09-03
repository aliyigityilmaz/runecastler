using UnityEngine;

[System.Serializable]
public struct ResourceRequirement
{
    public string resourceName; // The name of the resource, e.g., "Wood", "Stone"
    public int amount; // The amount required for this building
}

public class Building : MonoBehaviour
{
    public int width = 1; // Building width
    public int height = 1; // Building height
    public ResourceRequirement[] resourceRequirements; // Resource requirements
    public string buildingName; // Building name

    // Called when the building is placed
    public virtual void OnPlaced(Tile tile)
    {
        // Mark the occupied area
        MarkOccupiedArea(tile);
    }

    void MarkOccupiedArea(Tile startingTile)
    {
        WorldGenerator worldGenerator = FindObjectOfType<WorldGenerator>();
        Vector3 tilePosition = startingTile.transform.position;
        int startX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int startZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        for (int x = startX; x < startX + width; x++)
        {
            for (int z = startZ; z < startZ + height; z++)
            {
                if (x >= 0 && x < worldGenerator.worldWidth && z >= 0 && z < worldGenerator.worldHeight)
                {
                    Tile tile = worldGenerator.tiles[x, z];
                    tile.isOccupied = true;
                }
            }
        }
    }
}
