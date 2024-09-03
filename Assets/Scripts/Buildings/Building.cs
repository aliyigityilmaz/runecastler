using UnityEngine;

[System.Serializable]
public struct ResourceRequirement
{
    public string resourceName; // The name of the resource, e.g., "Wood", "Stone"
    public int amount; // The amount required for this building
}

public class Building : MonoBehaviour
{
    public int width = 1; // Bina geniþliði
    public int height = 1; // Bina yüksekliði
    public ResourceRequirement[] resourceRequirements; // Kaynak gereksinimleri
    public string buildingName; // Bina ismi

    public void OnPlaced(Tile tile)
    {
        tile.Cleanse();
    }
}