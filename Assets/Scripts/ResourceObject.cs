using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    public int health = 100; // Starting health of the resource
    public int resourcesAmount = 10; // Amount of resources gathered when destroyed

    private Tile tile;

    void Start()
    {
        tile = GetComponentInParent<Tile>(); // Get the parent tile this resource is placed on
    }

    void OnMouseDown()
    {
        GatherResource(25); // Example: Reduce health by 25 on each click
    }

    public void GatherResource(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            DestroyResource();
        }
    }

    void DestroyResource()
    {
        // Free up the tile
        tile.isOccupied = false;

        // Optionally drop gathered resources to player's inventory
        // PlayerInventory.AddResources(resourcesAmount);

        // Destroy the object
        Destroy(gameObject);
    }
}