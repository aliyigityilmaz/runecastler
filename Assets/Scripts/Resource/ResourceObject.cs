using UnityEngine;
using UnityEngine.UI;

public class ResourceObject : MonoBehaviour
{
    public enum ResourceType { Wood, Stone }
    public ResourceType resourceType;
    public int resourcesAmount = 10; // Toplam kaynak miktar�
    public int maxHitsForResource; // Ka� vuru�ta bir kaynak verilecek

    private int hitCounter = 0; // Vuru� say�s�n� takip eder
    private Tile tile;
    private ResourceSystem resourceSystem;

    // UI i�in Image referans�
    public Image fillImage;
    public Canvas fillCanvas;

    void Start()
    {
        tile = GetComponentInParent<Tile>(); // Parent tile'� al
        resourceSystem = FindObjectOfType<ResourceSystem>(); // Sahnedeki ResourceSystem'i bul

        // Doluluk bar�n� ba�lat
        UpdateFillImage();

        // �lk ba�ta doluluk bar�n� gizle
        fillCanvas.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (hitCounter == 0)
        {
            // �lk vurdu�umuzda doluluk bar�n� g�ster
            fillCanvas.gameObject.SetActive(true);
        }

        hitCounter++;
        PlayHitEffects();

        // Doluluk bar�n� g�ncelle
        UpdateFillImage();

        // Vuru� ba��na kaynak toplama mant���
        if (hitCounter >= maxHitsForResource)
        {
            GatherResource(1); // Kaynak ver
            hitCounter = 0; // Vuru� say�s�n� s�f�rla
            UpdateFillImage(); // Bar� s�f�rla
            fillCanvas.gameObject.SetActive(false); // Bar� gizle
        }
    }

    public void GatherResource(int amount)
    {
        if (resourcesAmount > 0)
        {
            resourcesAmount -= amount;
            resourceSystem.AddResource((global::ResourceType)resourceType, amount);

            if (resourcesAmount <= 0)
            {
                PlayDestroyEffects();
                DestroyResource();
            }
        }
    }

    void UpdateFillImage()
    {
        float fillValue = 1f - (float)hitCounter / maxHitsForResource;
        fillImage.fillAmount = fillValue;
    }

    void PlayHitEffects()
    {
        // Ses ve partik�l efektlerini oynat
        // if (resourceType == ResourceType.Wood)
        // {
        //     AudioSource.PlayClipAtPoint(woodHitSound, transform.position);
        //     woodHitParticles?.Play();
        // }
        // else if (resourceType == ResourceType.Stone)
        // {
        //     AudioSource.PlayClipAtPoint(stoneHitSound, transform.position);
        //     stoneHitParticles?.Play();
        // }
    }

    void PlayDestroyEffects()
    {
        // Yok olma sesini ve partik�l efektlerini oynat
        // AudioSource.PlayClipAtPoint(destroySound, transform.position);
        // Instantiate(destroyParticles, transform.position, Quaternion.identity); // Yok olma efekti
    }

    void DestroyResource()
    {
        // Tile'� bo�alt
        tile.isOccupied = false;

        // Objeyi yok et
        Destroy(gameObject);
    }
}
