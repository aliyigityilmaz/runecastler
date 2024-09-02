using UnityEngine;
using UnityEngine.UI;

public class BuildSystem : MonoBehaviour
{
    public GameObject buildModeUI; // UI element to show when in build mode
    public Button[] buildingButtons; // Array of buttons representing each building
    public GameObject[] buildingPrefabs; // Array of building prefabs
    public int[] playerResources; // Array to store player's resources (e.g., wood, stone)

    private WorldGenerator worldGenerator;
    private Camera mainCamera;
    private bool isBuildModeActive = false;
    private GameObject selectedBuildingPrefab;
    private Building selectedBuildingScript;

    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        mainCamera = Camera.main;
        buildModeUI.SetActive(false); // Ensure the UI is hidden at the start
        InitializeBuildingButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // Toggle build mode with the B key
        {
            ToggleBuildMode();
        }

        if (isBuildModeActive && selectedBuildingPrefab != null && Input.GetMouseButtonDown(0)) // Left mouse button
        {
            TryPlaceBuilding();
        }
    }

    void ToggleBuildMode()
    {
        isBuildModeActive = !isBuildModeActive;
        buildModeUI.SetActive(isBuildModeActive); // Show or hide the build mode UI

        if (!isBuildModeActive)
        {
            selectedBuildingPrefab = null; // Deselect any building when leaving build mode
        }
    }

    void InitializeBuildingButtons()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            int index = i;
            buildingButtons[i].onClick.AddListener(() => SelectBuilding(index));
        }
    }

    void SelectBuilding(int index)
    {
        selectedBuildingPrefab = buildingPrefabs[index];
        selectedBuildingScript = selectedBuildingPrefab.GetComponent<Building>();
    }

    void TryPlaceBuilding()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null && CanPlaceBuilding(tile))
            {
                PlaceBuilding(tile);
            }
        }
    }

    bool CanPlaceBuilding(Tile startingTile)
    {
        // Check if the player has enough resources
        foreach (var requirement in selectedBuildingScript.resourceRequirements)
        {
            if (playerResources[GetResourceIndex(requirement.resourceName)] < requirement.amount)
            {
                return false;
            }
        }

        // Get tile position in the grid
        Vector3 tilePosition = startingTile.transform.position;
        int startX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int startZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        // Check if the building can fit in the grid and tiles are not occupied
        for (int x = startX; x < startX + selectedBuildingScript.width; x++)
        {
            for (int z = startZ; z < startZ + selectedBuildingScript.height; z++)
            {
                if (x < 0 || x >= worldGenerator.worldWidth || z < 0 || z >= worldGenerator.worldHeight || worldGenerator.tiles[x, z].isOccupied)
                {
                    return false;
                }
            }
        }

        return true;
    }

    void PlaceBuilding(Tile startingTile)
    {
        Vector3 buildPosition = startingTile.transform.position;
        GameObject building = Instantiate(selectedBuildingPrefab, buildPosition, Quaternion.identity);
        building.transform.SetParent(worldGenerator.transform);

        // Get tile position in the grid
        Vector3 tilePosition = startingTile.transform.position;
        int startX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int startZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        // Mark the occupied tiles
        for (int x = startX; x < startX + selectedBuildingScript.width; x++)
        {
            for (int z = startZ; z < startZ + selectedBuildingScript.height; z++)
            {
                if (x >= 0 && x < worldGenerator.worldWidth && z >= 0 && z < worldGenerator.worldHeight)
                {
                    worldGenerator.tiles[x, z].isOccupied = true;
                }
            }
        }

        // Deduct resources
        foreach (var requirement in selectedBuildingScript.resourceRequirements)
        {
            playerResources[GetResourceIndex(requirement.resourceName)] -= requirement.amount;
        }

        selectedBuildingPrefab = null; // Deselect the building after placement
    }

    int GetResourceIndex(string resourceName)
    {
        // Implement this function to return the correct index based on resource name
        // This can be improved by using a dictionary or another more flexible method
        return 0;
    }
}
