using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOccupied;
    public bool isCleansed;
    public TileType tileType;
    public GameObject[] spawnableObjects; // Toprak �zerinde tekrar spawnlanabilecek objeler (a�a�, ta� vb.)
    public float spawnChance = 0.1f; // Obje spawnlanma ihtimali (�rne�in %10)
    private WorldGenerator worldGenerator;

    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        spawnableObjects = worldGenerator.objects;
        // E�er toprak cleansed ise, belli bir ihtimalle obje spawnla
        if (isCleansed && Random.value < spawnChance)
        {
            SpawnObject();
        }
    }

    public void Cleanse()
    {
        isCleansed = true;

        // Temizleme i�lemi sonras� isOccupied durumunu g�ncelle
        isOccupied = false;

        // Temizleme sonras� spawn �ans� i�in bir deneme yap
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
            isOccupied = true; // Yeni obje spawnland��� i�in toprak tekrar i�gal edildi
        }
    }
}
