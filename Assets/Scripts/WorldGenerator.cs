using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab; // Düzlemi oluþturmak için kullanýlacak tile
    public GameObject[] objects; // Aðaç, taþ gibi objeler
    public GameObject basePrefab; // Oyuncu üssü (base) için prefab

    [Header("World Settings")]
    public int worldWidth = 10; // Dünyanýn geniþliði
    public int worldHeight = 10; // Dünyanýn yüksekliði
    public float tileSize = 1f; // Tile boyutu
    public float spacing = 0.1f; // Tilelar arasýndaki boþluk
    public float objectSpawnChance = 0.2f; // Objelerin spawn olma ihtimali
    public float objectHeightOffset = 0.5f; // Objelerin y ekseninde yerleþim düzeltmesi
    public Tile[,] tiles; // Tüm tile'larý saklamak için 2D dizi

    void Awake()
    {
        // Enable fog
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential; // or FogMode.Linear for linear fog
        RenderSettings.fogColor = Color.gray; // Set the color of the fog
        RenderSettings.fogDensity = 0.01f; // Adjust the density for Exponential fog

        tiles = new Tile[worldWidth, worldHeight];
        GenerateWorld();
        PlacePlayerBase();
    }

    void GenerateWorld()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;
        int centerRange = 2; // Merkezden 2 birim uzaklýktaki alanlarý boþ býrak (5x5 alan için)

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
                // Tile oluþturma
                Vector3 tilePosition = new Vector3(x * (tileSize + spacing), 0, z * (tileSize + spacing));
                GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                // Tile'ý WorldGenerator objesinin child'ý yap
                tileObject.transform.SetParent(transform);

                tileObject.transform.localScale = new Vector3(tileSize, 1, tileSize); // Tile boyutunu ayarla

                // Tile componentini ekleyip diziye kaydet
                Tile tile = tileObject.AddComponent<Tile>();
                tile.isOccupied = false;
                tiles[x, z] = tile;

                // Merkezdeki 5x5 alaný boþ býrak
                if (x >= centerX - centerRange && x <= centerX + centerRange &&
                    z >= centerZ - centerRange && z <= centerZ + centerRange)
                {
                    continue;
                }

                // Objelerin rastgele spawn olma ihtimali
                if (Random.value < objectSpawnChance)
                {
                    int randomIndex = Random.Range(0, objects.Length);
                    GameObject obj = objects[randomIndex];

                    Vector3 objectPosition = new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + objectHeightOffset, tileObject.transform.position.z);

                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    // Objeyi oluþtur ve tile'ýn child'ý yap
                    GameObject placedObject = Instantiate(obj, objectPosition, randomRotation);
                    placedObject.transform.SetParent(tileObject.transform);

                    // Tile'ý dolu olarak iþaretle
                    tile.isOccupied = true;
                }
            }
        }
    }

    void PlacePlayerBase()
    {
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;

        // 3x3'lük alaný kontrol et ve base'i yerleþtir
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int z = centerZ - 1; z <= centerZ + 1; z++)
            {
                if (x >= 0 && x < worldWidth && z >= 0 && z < worldHeight)
                {
                    tiles[x, z].isOccupied = true; // Base'in altýnda kalan tile'larý dolu olarak iþaretle
                }
            }
        }

        // Base'i merkezde yerleþtir
        Vector3 basePosition = new Vector3(centerX * (tileSize + spacing), 0, centerZ * (tileSize + spacing));
        GameObject playerBase = Instantiate(basePrefab, new Vector3(basePosition.x, basePosition.y + objectHeightOffset, basePosition.z), Quaternion.identity);
        playerBase.transform.SetParent(transform); // Base'i WorldGenerator objesinin child'ý yap
    }

    // Bu metodu kullanarak herhangi bir tile'a bina yerleþtirildiðinde isOccupied'ý true yapabilirsiniz
    public void OccupyTile(int x, int z)
    {
        if (x >= 0 && x < worldWidth && z >= 0 && z < worldHeight)
        {
            tiles[x, z].isOccupied = true;
        }
    }
}

// Tile sýnýfý, her bir tile'ýn dolu olup olmadýðýný kontrol etmek için kullanýlýr
public class Tile : MonoBehaviour
{
    public bool isOccupied;
}
