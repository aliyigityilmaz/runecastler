using UnityEngine;

[System.Serializable]
public struct ResourceRequirement
{
    public string resourceName; // The name of the resource, e.g., "Wood", "Stone"
    public int amount; // The amount required for this building
}

public class Building : MonoBehaviour
{
    public int width = 1; // The width of the building in tiles
    public int height = 1; // The height of the building in tiles
    public ResourceRequirement[] resourceRequirements; // Resources needed to build this building
    public string buildingName; // The name of the building
}