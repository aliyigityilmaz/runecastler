using UnityEngine;
using UnityEngine.UI;

public class ResourceObject : MonoBehaviour
{
    public enum ResourceType { Wood, Stone }
    public ResourceType resourceType;
    public int resourcesAmount = 10; // Toplam kaynak miktarý
    public int maxHitsForResource; // Kaç vuruþta bir kaynak verilecek

    private int hitCounter = 0; // Vuruþ sayýsýný takip eder
    private Tile tile;
    private ResourceSystem resourceSystem;

    // UI için Image referansý
    public Image fillImage;
    public Canvas fillCanvas;

    void Start()
    {
        tile = GetComponentInParent<Tile>(); // Parent tile'ý al
        resourceSystem = FindObjectOfType<ResourceSystem>(); // Sahnedeki ResourceSystem'i bul

        // Doluluk barýný baþlat
        UpdateFillImage();

        // Ýlk baþta doluluk barýný gizle
        fillCanvas.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (hitCounter == 0)
        {
            // Ýlk vurduðumuzda doluluk barýný göster
            fillCanvas.gameObject.SetActive(true);
        }

        hitCounter++;
        PlayHitEffects();

        // Doluluk barýný güncelle
        UpdateFillImage();

        // Vuruþ baþýna kaynak toplama mantýðý
        if (hitCounter >= maxHitsForResource)
        {
            GatherResource(1); // Kaynak ver
            hitCounter = 0; // Vuruþ sayýsýný sýfýrla
            UpdateFillImage(); // Barý sýfýrla
            fillCanvas.gameObject.SetActive(false); // Barý gizle
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
        // Ses ve partikül efektlerini oynat
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
        // Yok olma sesini ve partikül efektlerini oynat
        // AudioSource.PlayClipAtPoint(destroySound, transform.position);
        // Instantiate(destroyParticles, transform.position, Quaternion.identity); // Yok olma efekti
    }

    void DestroyResource()
    {
        // Tile'ý boþalt
        tile.isOccupied = false;

        // Objeyi yok et
        Destroy(gameObject);
    }
}
