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
        // Fog ayarlar�n� yap
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

                // Obje spawn etme olas�l���n� sadece Greenland tiles i�in uygula
                if (tileType == TileType.Greenland && Random.value < objectSpawnChance)
                {
                    SpawnObjectsOnTile(tileObject);
                }
            }
        }
    }

    void SpawnObjectsOnTile(GameObject tileObject)
    {
        // Tile �zerindeki SpawnPoint objesini bul
        Transform spawnPoint = tileObject.transform.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogError("no spawn");
        }
        Debug.Log(spawnPoint);
        // E�er SpawnPoint objesi yoksa veya tile zaten doluysa obje spawn etmiyoruz
        if (spawnPoint == null || tileObject.GetComponent<Tile>().isOccupied)
        {
            return;
        }

        int randomIndex = Random.Range(0, objects.Length);
        GameObject obj = objects[randomIndex];

        // Obje spawn edilirken SpawnPoint pozisyonunu kullan�yoruz
        Vector3 objectPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        GameObject placedObject = Instantiate(obj, objectPosition, randomRotation);
        placedObject.transform.SetParent(tileObject.transform);

        // Bu tile art�k dolu olarak i�aretlenir
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

        // E�er tile null ise hata vermeden ��k
        if (tile == null)
        {
            Debug.LogError($"Tile at ({x}, {z}) is null.");
            return;
        }

        // Tile'�n tipini Greenland olarak de�i�tir
        tile.tileType = TileType.Greenland;
        tile.isCleansed = true;

        // Tile'� g�rsel olarak da temizlenmi� olarak i�aretleyebilirsiniz
        // �rne�in, prefab� de�i�tirmek veya rengi g�ncellemek i�in buraya kod ekleyin
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
        tiles[x, z].tileType = TileType.Greenland;

        // Temizlenen tile �zerinde a�a� ve ta�lar� tekrar spawn etme olas�l���n� kontrol et
        if (Random.value < objectSpawnChance)
        {
            SpawnObjectsOnTile(cleansedTile);
        }
    }

}
