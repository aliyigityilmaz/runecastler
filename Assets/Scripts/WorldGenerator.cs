using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject greenlandPrefab; // Greenland tile i�in prefab
    public GameObject rottenlandPrefab; // Rottenland tile i�in prefab
    public GameObject[] objects; // A�a�, ta� gibi objeler
    public GameObject basePrefab; // Oyuncu �ss� (base) i�in prefab

    [Header("World Settings")]
    public int worldWidth = 10; // D�nyan�n geni�li�i
    public int worldHeight = 10; // D�nyan�n y�ksekli�i
    public float tileSize = 1f; // Tile boyutu
    public float spacing = 0.1f; // Tilelar aras�ndaki bo�luk
    public float objectSpawnChance = 0.2f; // Objelerin spawn olma ihtimali
    public float objectHeightOffset = 0.5f; // Objelerin y ekseninde yerle�im d�zeltmesi
    public int greenlandRadius = 2; // Base etraf�ndaki Greenland alan�
    public Tile[,] tiles; // T�m tile'lar� saklamak i�in 2D dizi

    void Awake()
    {
        // Enable fog
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
                
                // Tile'�n t�r�n� belirle
                GameObject tilePrefab = rottenlandPrefab; // Varsay�lan olarak Rottenland
                TileType tileType = TileType.Rottenland;
                bool isCleansed = false;

                // Greenland i�in merkezi bir alan belirle
                if (Mathf.Abs(x - centerX) <= greenlandRadius && Mathf.Abs(z - centerZ) <= greenlandRadius)
                {
                    tilePrefab = greenlandPrefab;
                    tileType = TileType.Greenland;
                    isCleansed = true; // Ba�lang��ta Greenland olan tile'lar temizlenmi� olarak ba�lar
                }

                // Tile olu�turma
                Vector3 tilePosition = new Vector3(x * (tileSize + spacing), 0, z * (tileSize + spacing));
                GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                 // Tile'�n spawnlayabilece�i objeleri ayarla
                // Tile'� WorldGenerator objesinin child'� yap
                tileObject.transform.SetParent(transform);
                tileObject.transform.localScale = new Vector3(tileSize, 1, tileSize); // Tile boyutunu ayarla

                // Tile componentini ekleyip diziye kaydet
                Tile tile = tileObject.AddComponent<Tile>();
                tile.isOccupied = false;
                tile.isCleansed = isCleansed;
                tile.tileType = tileType;
                tiles[x, z] = tile;

                // Objelerin sadece Greenland'da spawn olma ihtimali
                if (tileType == TileType.Greenland && Random.value < objectSpawnChance)
                {
                    int randomIndex = Random.Range(0, objects.Length);
                    GameObject obj = objects[randomIndex];

                    Vector3 objectPosition = new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + objectHeightOffset, tileObject.transform.position.z);
                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    // Objeyi olu�tur ve tile'�n child'� yap
                    GameObject placedObject = Instantiate(obj, objectPosition, randomRotation);
                    placedObject.transform.SetParent(tileObject.transform);

                    // Tile'� dolu olarak i�aretle
                    tile.isOccupied = true;
                }
            }
        }
    }

    void PlacePlayerBase()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;

        // Base'i merkezde yerle�tir
        Vector3 basePosition = new Vector3(centerX * (tileSize + spacing), 0, centerZ * (tileSize + spacing));
        GameObject playerBase = Instantiate(basePrefab, new Vector3(basePosition.x, basePosition.y + objectHeightOffset, basePosition.z), Quaternion.identity);
        playerBase.transform.SetParent(transform); // Base'i WorldGenerator objesinin child'� yap
    }

    // Bu metodu kullanarak herhangi bir tile'a bina yerle�tirildi�inde isOccupied'� true yapabilirsiniz
    public void OccupyTile(int x, int z)
    {
        if (x >= 0 && x < worldWidth && z >= 0 && z < worldHeight)
        {
            tiles[x, z].isOccupied = true;
        }
    }

    // Bu metodu kullanarak herhangi bir tile'� temizleyip Greenland yapabilirsiniz
    public void CleanseTile(int x, int z)
    {
        if (x >= 0 && x < worldWidth && z >= 0 && z < worldHeight)
        {
            Tile tile = tiles[x, z];
            tile.isCleansed = true;
            tile.tileType = TileType.Greenland;

            // Rottenland prefab'�n� Greenland prefab'� ile de�i�tir
            GameObject tileObject = Instantiate(greenlandPrefab, tile.transform.position, Quaternion.identity);
            tileObject.transform.SetParent(transform);
            tileObject.transform.localScale = new Vector3(tileSize, 1, tileSize);
            Destroy(tile.gameObject); // Eski tile'� yok et
            tiles[x, z] = tileObject.AddComponent<Tile>(); // Yeni tile'� diziye kaydet
        }
    }
}

public enum TileType
{
    Greenland,
    Rottenland
}
