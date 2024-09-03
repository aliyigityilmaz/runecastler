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

    void Awake()
    {
        // Fog ayarlarýný yap
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogDensity = 0.01f;

        tiles = new Tile[worldWidth, worldHeight];
        GenerateWorld();
        PlacePlayerBase();
    }

    void GenerateWorld()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
                GameObject tilePrefab = rottenlandPrefab; // Varsayýlan olarak Rottenland
                TileType tileType = TileType.Rottenland;  // TileType enum'ýný burada kullanýyoruz
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
                tile.tileType = tileType;  // Enum'ý burada kullanýyoruz
                tiles[x, z] = tile;

                // Obje spawn etme olasýlýðýný sadece Greenland tiles için uygula
                if (tileType == TileType.Greenland && Random.value < objectSpawnChance)
                {
                    SpawnObjectsOnTile(tileObject);
                }
            }
        }
    }

    void SpawnObjectsOnTile(GameObject tileObject)
    {
        // Tile üzerindeki SpawnPoint objesini bul
        Transform spawnPoint = tileObject.transform.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogError("no spawn");
        }
        Debug.Log(spawnPoint);
        // Eðer SpawnPoint objesi yoksa veya tile zaten doluysa obje spawn etmiyoruz
        if (spawnPoint == null || tileObject.GetComponent<Tile>().isOccupied)
        {
            return;
        }

        int randomIndex = Random.Range(0, objects.Length);
        GameObject obj = objects[randomIndex];

        // Obje spawn edilirken SpawnPoint pozisyonunu kullanýyoruz
        Vector3 objectPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        GameObject placedObject = Instantiate(obj, objectPosition, randomRotation);
        placedObject.transform.SetParent(tileObject.transform);

        // Bu tile artýk dolu olarak iþaretlenir
        tileObject.GetComponent<Tile>().isOccupied = true;
    }




    void PlacePlayerBase()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;

        // Loop through a 3x3 area centered around the base
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int z = centerZ - 1; z <= centerZ + 1; z++)
            {
                if (x >= 0 && x < worldWidth && z >= 0 && z < worldHeight)
                {
                    // Place the base building only in the center
                    if (x == centerX && z == centerZ)
                    {
                        Vector3 basePosition = new Vector3(x * (tileSize + spacing), 2, z * (tileSize + spacing));
                        GameObject playerBase = Instantiate(basePrefab, basePosition, Quaternion.identity);

                        // Parent the base to the center tile
                        playerBase.transform.SetParent(tiles[x, z].transform);
                    }

                    // Mark the tile as occupied and prevent any spawning
                    tiles[x, z].isOccupied = true;
                }
            }
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

        // Eðer tile null ise hata vermeden çýk
        if (tile == null)
        {
            Debug.LogError($"Tile at ({x}, {z}) is null.");
            return;
        }

        // Tile'ýn tipini Greenland olarak deðiþtir
        tile.tileType = TileType.Greenland;
        tile.isCleansed = true;

        // Tile'ý görsel olarak da temizlenmiþ olarak iþaretleyebilirsiniz
        // Örneðin, prefabý deðiþtirmek veya rengi güncellemek için buraya kod ekleyin
        GameObject cleansedTile = Instantiate(greenlandPrefab, tile.transform.position, Quaternion.identity);
        cleansedTile.transform.SetParent(tile.transform.parent);
        cleansedTile.transform.localScale = tile.transform.localScale;

        // Eski tile'ý yok et ve yenisiyle deðiþtir
        Destroy(tile.gameObject);
        tiles[x, z] = cleansedTile.GetComponent<Tile>();

        if (tiles[x, z] == null)
        {
            Debug.LogError($"Failed to get Tile component from the cleansed prefab at ({x}, {z})");
            return;
        }

        tiles[x, z].isCleansed = true;
        tiles[x, z].tileType = TileType.Greenland;

        // Temizlenen tile üzerinde aðaç ve taþlarý tekrar spawn etme olasýlýðýný kontrol et
        if (Random.value < objectSpawnChance)
        {
            SpawnObjectsOnTile(cleansedTile);
        }
    }

}
