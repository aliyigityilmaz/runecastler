using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // Düzlemi oluþturmak için kullanýlacak tile
    public GameObject[] objects; // Aðaç, taþ gibi objeler
    public int worldWidth = 10; // Dünyanýn geniþliði
    public int worldHeight = 10; // Dünyanýn yüksekliði
    public float tileSize = 1f; // Tile boyutu
    public float spacing = 0.1f; // Tilelar arasýndaki boþluk
    public float objectSpawnChance = 0.2f; // Objelerin spawn olma ihtimali
    public float objectHeightOffset = 0.5f; // Objelerin y ekseninde yerleþim düzeltmesi

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        // Dünya ortasýndaki 5x5 alaný boþ býrakmak için merkezi belirle
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;
        int centerRange = 2; // Merkezden 2 birim uzaklýktaki alanlarý boþ býrak

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
                // Tile oluþturma
                Vector3 tilePosition = new Vector3(x * (tileSize + spacing), 0, z * (tileSize + spacing));
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                // Tile'ý WorldGenerator objesinin child'ý yap
                tile.transform.SetParent(transform);

                tile.transform.localScale = new Vector3(tileSize, 1, tileSize); // Tile boyutunu ayarla

                // Merkezdeki 5x5 alaný boþ býrak
                if (x >= centerX - centerRange && x <= centerX + centerRange &&
                    z >= centerZ - centerRange && z <= centerZ + centerRange)
                {
                    continue;
                }

                // Objelerin rastgele spawn olma ihtimali
                if (Random.value < objectSpawnChance)
                {
                    // Objeyi rastgele seç
                    int randomIndex = Random.Range(0, objects.Length);
                    GameObject obj = objects[randomIndex];

                    // Objeyi tile'ýn tam merkezine yerleþtir ve y ekseninde offset uygula
                    Vector3 objectPosition = new Vector3(tile.transform.position.x, tile.transform.position.y + objectHeightOffset, tile.transform.position.z);

                    // Rastgele bir rotasyon belirle
                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    // Objeyi yerleþtir ve rotasyonu ayarla
                    GameObject placedObject = Instantiate(obj, objectPosition, randomRotation);

                    // Objeyi WorldGenerator objesinin child'ý yap
                    placedObject.transform.SetParent(tile.transform);
                }
            }
        }
    }
}
