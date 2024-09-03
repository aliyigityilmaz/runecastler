using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOccupied;
    public bool isCleansed;
    public TileType tileType;  // TileType enum'ý burada kullanýlýyor
    public GameObject[] spawnableObjects;
    public float spawnChance = 0.1f;
    private WorldGenerator worldGenerator;

    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        spawnableObjects = worldGenerator.objects;

        if (isCleansed && Random.value < spawnChance)
        {
            SpawnObject();
        }
    }

    public void Cleanse()
    {
        isCleansed = true;
        isOccupied = false;

        if (Random.value < spawnChance)
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        if (spawnableObjects.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnableObjects.Length);
            GameObject spawnedObject = Instantiate(spawnableObjects[randomIndex], transform.position, Quaternion.identity);
            spawnedObject.transform.SetParent(transform);
            isOccupied = true;
        }
    }

    public enum TileType
    {
        Greenland,
        Rottenland
    }

}
