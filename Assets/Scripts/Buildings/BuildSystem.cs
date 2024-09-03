using UnityEngine;
using UnityEngine.UI;
using static Tile;

public class BuildSystem : MonoBehaviour
{
    public GameObject buildModeUI;
    public Button[] buildingButtons;
    public GameObject[] buildingPrefabs;
    public int[] playerResources = new int[3]; // Wood, Stone, Soul

    private WorldGenerator worldGenerator;
    private Camera mainCamera;
    private bool isBuildModeActive = false;
    private GameObject selectedBuildingPrefab;
    private Building selectedBuildingScript;

    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        mainCamera = Camera.main;
        buildModeUI.SetActive(false);
        InitializeBuildingButtons();

        Debug.Log("Build System Initialized");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }

        if (isBuildModeActive && selectedBuildingPrefab != null && Input.GetMouseButtonDown(0))
        {
            TryPlaceBuilding();
        }
    }

    void ToggleBuildMode()
    {
        isBuildModeActive = !isBuildModeActive;
        buildModeUI.SetActive(isBuildModeActive);

        if (!isBuildModeActive)
        {
            selectedBuildingPrefab = null;
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
        if (selectedBuildingPrefab != null)
        {
            selectedBuildingScript = selectedBuildingPrefab.GetComponent<Building>();
            if (selectedBuildingScript == null)
            {
                Debug.LogError("Selected prefab does not have a Building script attached.");
            }
        }
        else
        {
            Debug.LogError("Selected building prefab is null");
        }
    }

    void TryPlaceBuilding()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                Debug.Log($"Hit Tile at ({tile.transform.position.x}, {tile.transform.position.z})");
                if (CanPlaceBuilding(tile))
                {
                    PlaceBuilding(tile);
                }
            }
            else
            {
                Debug.Log("No Tile component found.");
            }
        }
    }

    bool CanPlaceBuilding(Tile startingTile)
    {
        // Check if the starting tile is of type Rottenland
        if (startingTile.tileType == TileType.Rottenland)
        {
            Debug.Log("Cannot place building on Rottenland.");
            return false;
        }

        // Check resource requirements
        foreach (var requirement in selectedBuildingScript.resourceRequirements)
        {
            if (playerResources[GetResourceIndex(requirement.resourceName)] < requirement.amount)
            {
                return false;
            }
        }

        // Check if the building fits and the tiles are not occupied
        Vector3 tilePosition = startingTile.transform.position;
        int startX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int startZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

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
        Vector3 buildPosition = new Vector3(startingTile.transform.position.x, 2, startingTile.transform.position.z);

        GameObject building = Instantiate(selectedBuildingPrefab, buildPosition, Quaternion.identity);

        PlaceBuildingOnTile(startingTile, building);

        Vector3 tilePosition = startingTile.transform.position;
        int startX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int startZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        for (int x = startX; x < startX + selectedBuildingScript.width; x++)
        {
            for (int z = startZ; z < startZ + selectedBuildingScript.height; z++)
            {
                if (x >= 0 && x < worldGenerator.worldWidth && z >= 0 && z < worldGenerator.worldHeight)
                {
                    Tile tile = worldGenerator.tiles[x, z];
                    if (tile != null)
                    {
                        tile.isOccupied = true;
                        selectedBuildingScript.OnPlaced(tile);
                    }
                    else
                    {
                        Debug.LogError($"Tile at ({x}, {z}) is null.");
                    }
                }
            }
        }

        foreach (var requirement in selectedBuildingScript.resourceRequirements)
        {
            playerResources[GetResourceIndex(requirement.resourceName)] -= requirement.amount;
        }

        selectedBuildingPrefab = null;
    }


    int GetResourceIndex(string resourceName)
    {
        switch (resourceName)
        {
            case "Wood": return 0;
            case "Stone": return 1;
            case "Soul": return 2;
            default: return -1; // Invalid resource name
        }
    }

    void PlaceBuildingOnTile(Tile tile, GameObject building)
    {
        // Set the building's position to match the tile's position
        building.transform.position = new Vector3(tile.transform.position.x, 2, tile.transform.position.z);

        // Make the building a child of the tile
        building.transform.SetParent(tile.transform);

        // Mark the tile as occupied
        tile.isOccupied = true;

        // If your building has specific logic to run when placed, call it here
        Building buildingScript = building.GetComponent<Building>();
        if (buildingScript != null)
        {
            buildingScript.OnPlaced(tile);
        }
    }
}
