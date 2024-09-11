using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using static Tile;

public class WorldGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject greenlandPrefab;
    public GameObject rottenlandPrefab;
    public GameObject[] objects;
    public GameObject basePrefab;

    [Header("World Settings")]
    public int worldWidth = 10;
    public int worldHeight = 10;
    public float tileSize = 1f;
    public float spacing = 0.1f;
    public float objectSpawnChance = 0.2f;
    public float objectHeightOffset = 0.5f;
    public int greenlandRadius = 2;
    public Tile[,] tiles;

    [Header("NavMesh Settings")]
    public NavMeshSurface navMeshSurface;

    void Awake()
    {
        // Fog ayarlar�n� yap
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogDensity = 0.01f;

        tiles = new Tile[worldWidth, worldHeight];

        // 1. Ad�m: D�nya olu�tur
        GenerateWorld();

        // 2. Ad�m: Player base yerle�tir
        PlacePlayerBase();

        // 3. Ad�m: Obje spawnlanmas�
        SpawnObjectsAfterBase();

        // NavMesh Bake i�lemi
        //StartCoroutine(BakeNavMeshAfterGeneration());

        SetSpawnOccupied();
    }

    void GenerateWorld()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
                GameObject tilePrefab = rottenlandPrefab; // Varsay�lan olarak Rottenland
                TileType tileType = TileType.Rottenland;  // TileType enum'�n� burada kullan�yoruz
                bool isCleansed = false;

                if (Mathf.Abs(x - centerX) <= greenlandRadius && Mathf.Abs(z - centerZ) <= greenlandRadius)
                {
                    tilePrefab = greenlandPrefab;
                    tileType = TileType.Greenland;
                    isCleansed = true;
                }

                Vector3 tilePosition = new Vector3(x * (tileSize + spacing), 0, z * (tileSize + spacing));
                GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tileObject.transform.SetParent(transform);
                tileObject.transform.localScale = new Vector3(tileSize, 1, tileSize);

                Tile tile = tileObject.AddComponent<Tile>();
                tile.isOccupied = false;
                tile.isCleansed = isCleansed;
                tile.tileType = tileType;  // Enum'� burada kullan�yoruz
                tiles[x, z] = tile;

                // Objeler �u an spawnlanm�yor, sadece base yerle�tikten sonra spawnlanacak.
            }
        }
    }

    void SpawnObjectsOnTile(GameObject tileObject)
    {
        Tile tile = tileObject.GetComponent<Tile>();

        // E�er tile doluysa veya base alan�ndaysa obje spawn etme
        if (tile.isOccupied)
        {
            Debug.Log("Tile is already occupied or part of the base.");
            return;
        }

        Transform spawnPoint = tileObject.transform.Find("SpawnPoint");

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint eksik.");
            return;
        }

        int randomIndex = Random.Range(0, objects.Length);
        GameObject obj = objects[randomIndex];

        // Obje spawn edilirken SpawnPoint pozisyonunu kullan�yoruz
        Vector3 objectPosition = spawnPoint.position;
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        GameObject placedObject = Instantiate(obj, objectPosition, randomRotation);
        placedObject.transform.SetParent(tileObject.transform);

        // Bu tile art�k dolu olarak i�aretlenir
        tile.isOccupied = true;
    }

    void SetSpawnOccupied()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;

        if(tiles[centerX, centerZ]!= null)
        { 
            tiles[centerX, centerZ].isOccupied = true;
        }
        tiles[centerX - 1, centerZ].isOccupied = true;
        tiles[centerX + 1, centerZ].isOccupied = true;
        tiles[centerX, centerZ - 1].isOccupied = true;
        tiles[centerX, centerZ + 1].isOccupied = true;
        tiles[centerX - 1, centerZ - 1].isOccupied = true;
        tiles[centerX + 1, centerZ - 1].isOccupied = true;
        tiles[centerX + 1, centerZ + 1].isOccupied = true;
        tiles[centerX - 1, centerZ + 1].isOccupied = true;
    }


    void PlacePlayerBase()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;

        tiles[centerX, centerZ].isOccupied = true;
        tiles[centerX - 1, centerZ].isOccupied = true;
        tiles[centerX + 1, centerZ].isOccupied = true;
        tiles[centerX, centerZ - 1].isOccupied = true;
        tiles[centerX, centerZ + 1].isOccupied = true;
        tiles[centerX - 1, centerZ - 1].isOccupied = true;
        tiles[centerX + 1, centerZ - 1].isOccupied = true;
        tiles[centerX + 1, centerZ + 1].isOccupied = true;
        tiles[centerX - 1, centerZ + 1].isOccupied = true;

        // "+" �eklinde ortadaki kareyi ve ona biti�ik olanlar� yok et
        DestroyTile(centerX, centerZ);

        // Base objesini ortadaki kareye spawnla
        Vector3 basePosition = new Vector3(centerX * (tileSize + spacing), 2, centerZ * (tileSize + spacing));
        GameObject playerBase = Instantiate(basePrefab, basePosition, Quaternion.identity);

        // Base objesini parent yap
        playerBase.transform.SetParent(transform);
    }

    void SpawnObjectsAfterBase()
    {
        // Obje spawn i�lemi sadece Greenland tile'larda ve belirli bir olas�l�k ile ger�ekle�ecek
        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
                if (tiles[x, z] != null && tiles[x, z].tileType == TileType.Greenland && !tiles[x, z].isOccupied)
                {
                    // Rastgele olas�l�k ile objeleri spawnla
                    if (Random.value < objectSpawnChance)
                    {
                        // Greenland tile'lar �zerinde obje spawnlamaya �al��
                        SpawnObjectsOnTile(tiles[x, z].gameObject);
                    }
                }
            }
        }
    }

    void DestroyTile(int x, int z)
    {
        if (x >= 0 && x < worldWidth && z >= 0 && z < worldHeight && tiles[x, z] != null)
        {
            // Tile'� yok et ve grid'den kald�r
            Destroy(tiles[x, z].gameObject);
            tiles[x, z] = null;
        }
    }


    IEnumerator BakeNavMeshAfterGeneration()
    {
        // T�m d�nya tamamland�ktan sonra bir �er�eve bekle
        yield return new WaitForEndOfFrame();

        // NavMesh bake i�lemini ba�lat
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh successfully baked!");
        }
        else
        {
            Debug.LogError("NavMeshSurface component is not assigned!");
        }
    }

    public void PlaceBuildingOnTile(int x, int z, GameObject buildingPrefab)
    {
        if (x >= 0 && x < worldWidth && z >= 0 && z < worldHeight)
        {
            Vector3 buildingPosition = new Vector3(x * (tileSize + spacing), 2, z * (tileSize + spacing));
            GameObject building = Instantiate(buildingPrefab, buildingPosition, Quaternion.identity);

            // Parent the building to the tile
            building.transform.SetParent(tiles[x, z].transform);

            // Mark the tile as occupied
            tiles[x, z].isOccupied = true;
        }
    }

    public void CleanseTile(int x, int z)
    {
        Tile tile = tiles[x, z];

        if (tile == null)
        {
            Debug.LogError($"Tile at ({x}, {z}) is null.");
            return;
        }

        if (tile.tileType == Tile.TileType.Greenland)
        {
            Debug.Log($"Tile at ({x}, {z}) is already cleansed.");
            return;
        }

        // Tile'�n tipini Greenland olarak de�i�tir
        tile.tileType = Tile.TileType.Greenland;
        tile.isCleansed = true;

        // G�rsel temizleme i�lemi
        GameObject cleansedTile = Instantiate(greenlandPrefab, tile.transform.position, Quaternion.identity);
        cleansedTile.transform.SetParent(tile.transform.parent);
        cleansedTile.transform.localScale = tile.transform.localScale;

        // Eski tile'� yok et ve yenisiyle de�i�tir
        Destroy(tile.gameObject);
        tiles[x, z] = cleansedTile.GetComponent<Tile>();

        if (tiles[x, z] == null)
        {
            Debug.LogError($"Failed to get Tile component from the cleansed prefab at ({x}, {z})");
            return;
        }

        tiles[x, z].isCleansed = true;
        tiles[x, z].tileType = Tile.TileType.Greenland;

        // Temizlenen tile �zerinde a�a� ve ta�lar� tekrar spawn etme olas�l���n� kontrol et
        if (Random.value < objectSpawnChance)
        {
            SpawnObjectsOnTile(cleansedTile);
        }
    }

    public bool IsWithinBounds(int x, int z)
    {
        return x >= 0 && x < worldWidth && z >= 0 && z < worldHeight;
    }


}
