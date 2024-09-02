using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone,
    Soul
}

[System.Serializable]
public class Resource
{
    public ResourceType resourceType;
    public int amount;
}

public class ResourceSystem : MonoBehaviour
{
    public List<Resource> resources;
    public ResourceUIManager resourceUIManager;

    // Initialize resources with starting amounts (can be set in the Inspector)
    void Start()
    {
        foreach (Resource resource in resources)
        {
            resource.amount = 0; // Start with 0 or any initial value
        }
    }

    // Add resources when gathering
    public void AddResource(ResourceType resourceType, int amount)
    {
        Resource resource = resources.Find(r => r.resourceType == resourceType);
        if (resource != null)
        {
            resource.amount += amount;
            Debug.Log(resourceType + " collected! Total: " + resource.amount);
            resourceUIManager.UpdateResourceUI(); // Update the UI
        }
    }

    public bool SpendResource(ResourceType resourceType, int amount)
    {
        Resource resource = resources.Find(r => r.resourceType == resourceType);
        if (resource != null && resource.amount >= amount)
        {
            resource.amount -= amount;
            Debug.Log(amount + " " + resourceType + " spent. Remaining: " + resource.amount);
            resourceUIManager.UpdateResourceUI(); // Update the UI
            return true;
        }
        else
        {
            Debug.Log("Not enough " + resourceType + "!");
            return false;
        }
    }

    // Check if the player has enough resources
    public bool HasEnoughResource(ResourceType resourceType, int amount)
    {
        Resource resource = resources.Find(r => r.resourceType == resourceType);
        return resource != null && resource.amount >= amount;
    }
}