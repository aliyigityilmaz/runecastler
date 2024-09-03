using UnityEngine;
using UnityEngine.UI;

public class BuildSystem : MonoBehaviour
{
    public GameObject buildModeUI;
    public Button[] buildingButtons;
    public GameObject[] buildingPrefabs;
    public int[] playerResources = new int[3]; // Odun, Taþ, Ruh sýrasýyla

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
            Debug.Log("Building selected: " + selectedBuildingPrefab.name);
        }
        else
        {
            Debug.LogError("Selected building prefab is null");
        }

        if (selectedBuildingScript == null)
        {
            Debug.LogError("Building script is missing on the selected prefab");
        }
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
        foreach (var requirement in selectedBuildingScript.resourceRequirements)
        {
            if (playerResources[GetResourceIndex(requirement.resourceName)] < requirement.amount)
            {
                return false;
            }
        }

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
        Vector3 buildPosition = startingTile.transform.position;
        GameObject building = Instantiate(selectedBuildingPrefab, buildPosition, Quaternion.identity);
        building.transform.SetParent(worldGenerator.transform);

        // Get tile position in the grid
        Vector3 tilePosition = startingTile.transform.position;
        int startX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int startZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        // Mark the occupied tiles and cleanse the starting tile
        for (int x = startX; x < startX + selectedBuildingScript.width; x++)
        {
            for (int z = startZ; z < startZ + selectedBuildingScript.height; z++)
            {
                if (x >= 0 && x < worldGenerator.worldWidth && z >= 0 && z < worldGenerator.worldHeight)
                {
                    Tile tile = worldGenerator.tiles[x, z];
                    tile.isOccupied = true;
                    selectedBuildingScript.OnPlaced(tile); // Temizleme iþlemi yapýlýr
                }
            }
        }

        // Kaynaklarý azalt
        foreach (var requirement in selectedBuildingScript.resourceRequirements)
        {
            playerResources[GetResourceIndex(requirement.resourceName)] -= requirement.amount;
        }

        selectedBuildingPrefab = null; // Bina yerleþtirildikten sonra seçimi kaldýr
    }

    int GetResourceIndex(string resourceName)
    {
        // Implement this function to return the correct index based on resource name
        // This can be improved by using a dictionary or another more flexible method
        return 0;
    }
}
