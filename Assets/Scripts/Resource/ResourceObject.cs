using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

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

    [Header("Effects")]
    public AudioClip woodHitSound;
    public AudioClip stoneHitSound;
    public AudioClip destroySound;
    public ParticleSystem woodHitParticles;
    public ParticleSystem stoneHitParticles;
    public ParticleSystem destroyParticles;

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
        if (resourceType == ResourceType.Wood)
        {
            if (woodHitSound != null)
            {
                AudioSource.PlayClipAtPoint(woodHitSound, transform.position);
            }
            if (woodHitParticles != null)
            {
                ParticleSystem instantiatedParticles = Instantiate(woodHitParticles, transform.position, Quaternion.identity);
                Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration);
            }
        }
        else if (resourceType == ResourceType.Stone)
        {
            if (stoneHitSound != null)
            {
                AudioSource.PlayClipAtPoint(stoneHitSound, transform.position);
            }
            if (stoneHitParticles != null)
            {
                ParticleSystem instantiatedParticles = Instantiate(stoneHitParticles, transform.position, Quaternion.identity);
                Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration);
            }
        }

        StartCoroutine(ShakeObject(0.1f, 0.1f));
    }

    IEnumerator ShakeObject(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Objeyi orijinal pozisyonuna geri döndür
        transform.localPosition = originalPosition;
    }

    void PlayDestroyEffects()
    {
        // Yok olma sesini ve partikül efektlerini oynat
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }
        if (destroyParticles != null)
        {
            // Yok olma partikülünü instantiate et ve bir süre sonra yok et
            ParticleSystem instantiatedParticles = Instantiate(destroyParticles, transform.position, Quaternion.identity);
            Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration);
        }
    }

    void DestroyResource()
    {
        // Tile'ý boþalt
        tile.isOccupied = false;

        // Objeyi yok et
        Destroy(gameObject);
    }
}
