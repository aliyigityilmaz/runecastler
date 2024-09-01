using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // D�zlemi olu�turmak i�in kullan�lacak tile
    public GameObject[] objects; // A�a�, ta� gibi objeler
    public int worldWidth = 10; // D�nyan�n geni�li�i
    public int worldHeight = 10; // D�nyan�n y�ksekli�i
    public float tileSize = 1f; // Tile boyutu
    public float spacing = 0.1f; // Tilelar aras�ndaki bo�luk
    public float objectSpawnChance = 0.2f; // Objelerin spawn olma ihtimali
    public float objectHeightOffset = 0.5f; // Objelerin y ekseninde yerle�im d�zeltmesi

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        // D�nya ortas�ndaki 5x5 alan� bo� b�rakmak i�in merkezi belirle
        int centerX = worldWidth / 2;
        int centerZ = worldHeight / 2;
        int centerRange = 2; // Merkezden 2 birim uzakl�ktaki alanlar� bo� b�rak

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
                // Tile olu�turma
                Vector3 tilePosition = new Vector3(x * (tileSize + spacing), 0, z * (tileSize + spacing));
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                // Tile'� WorldGenerator objesinin child'� yap
                tile.transform.SetParent(transform);

                tile.transform.localScale = new Vector3(tileSize, 1, tileSize); // Tile boyutunu ayarla

                // Merkezdeki 5x5 alan� bo� b�rak
                if (x >= centerX - centerRange && x <= centerX + centerRange &&
                    z >= centerZ - centerRange && z <= centerZ + centerRange)
                {
                    continue;
                }

                // Objelerin rastgele spawn olma ihtimali
                if (Random.value < objectSpawnChance)
                {
                    // Objeyi rastgele se�
                    int randomIndex = Random.Range(0, objects.Length);
                    GameObject obj = objects[randomIndex];

                    // Objeyi tile'�n tam merkezine yerle�tir ve y ekseninde offset uygula
                    Vector3 objectPosition = new Vector3(tile.transform.position.x, tile.transform.position.y + objectHeightOffset, tile.transform.position.z);

                    // Rastgele bir rotasyon belirle
                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    // Objeyi yerle�tir ve rotasyonu ayarla
                    GameObject placedObject = Instantiate(obj, objectPosition, randomRotation);

                    // Objeyi WorldGenerator objesinin child'� yap
                    placedObject.transform.SetParent(tile.transform);
                }
            }
        }
    }
}
