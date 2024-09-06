using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

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

    [Header("Effects")]
    public AudioClip woodHitSound;
    public AudioClip stoneHitSound;
    public AudioClip destroySound;
    public ParticleSystem woodHitParticles;
    public ParticleSystem stoneHitParticles;
    public ParticleSystem destroyParticles;

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

        // Objeyi orijinal pozisyonuna geri d�nd�r
        transform.localPosition = originalPosition;
    }

    void PlayDestroyEffects()
    {
        // Yok olma sesini ve partik�l efektlerini oynat
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }
        if (destroyParticles != null)
        {
            // Yok olma partik�l�n� instantiate et ve bir s�re sonra yok et
            ParticleSystem instantiatedParticles = Instantiate(destroyParticles, transform.position, Quaternion.identity);
            Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration);
        }
    }

    void DestroyResource()
    {
        // Tile'� bo�alt
        tile.isOccupied = false;

        // Objeyi yok et
        Destroy(gameObject);
    }
}
