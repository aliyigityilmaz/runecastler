using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOccupied;
    public bool isCleansed;
    public TileType tileType;  // TileType enum'� burada kullan�l�yor
    public GameObject[] spawnableObjects;
    public float spawnChance = 0.1f;
    private WorldGenerator worldGenerator;

    private Animator animator;

    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        spawnableObjects = worldGenerator.objects;
        animator = GetComponent<Animator>();

        // Rastgele bir zamanlay�c� ba�lat�yoruz.
        float randomDelay = Random.Range(0f, 1f); // 0-1 saniye aras�nda rastgele bir de�er se�iyoruz
        Invoke(nameof(PlaySpawnAnimation), randomDelay);

        if (isCleansed && Random.value < spawnChance && !isOccupied)
        {
            //SpawnObject();
            //isOccupied = true;
        }
    }

    void PlaySpawnAnimation()
    {
        if (animator != null)
        {
            animator.Play("Spawn"); // Animasyonun ismini ayarlay�n
        }
    }

    public void Cleanse()
    {
        isCleansed = true;
        isOccupied = false;

        // Rottenland'den Greenland'e �evir
        if (tileType == TileType.Rottenland)
        {
            tileType = TileType.Greenland;
            Debug.Log("Tile cleansed and converted to Greenland.");
        }

        // Rastgele nesne spawn et
        if (Random.value < spawnChance && !isOccupied)
        {
            //SpawnObject();
            //isOccupied = true;
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
