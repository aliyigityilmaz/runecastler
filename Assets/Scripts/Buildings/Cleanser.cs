using UnityEngine;

public class Cleanser : Building
{
    public int cleanseRadius = 1;


    // Building yerle�tirildi�inde �a�r�l�r
    public override void OnPlaced(Tile tile)
    {
        base.OnPlaced(tile); // Temel yerle�tirme i�lemini yap
        CleanseNearbyTiles(tile); // Yak�ndaki fayanslar� temizle
    }

    // Yak�ndaki fayanslar� temizle
    private void CleanseNearbyTiles(Tile tile)
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        if (worldGenerator == null)
        {
            Debug.LogError("WorldGenerator not found.");
            return;
        }

        // Fayans�n pozisyonunu d�nya grid koordinatlar�na d�n��t�relim
        Vector3 tilePosition = tile.transform.position;
        int tileX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int tileZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        for (int x = -cleanseRadius; x <= cleanseRadius; x++)
        {
            for (int z = -cleanseRadius; z <= cleanseRadius; z++)
            {
                int targetX = tileX + x;
                int targetZ = tileZ + z;

                // Fayans�n harita s�n�rlar� i�inde olup olmad���n� kontrol et
                if (worldGenerator.IsWithinBounds(targetX, targetZ))
                {
                    Tile targetTile = worldGenerator.tiles[targetX, targetZ];

                    // E�er fayans Rottenland ise temizleyelim
                    if (targetTile.tileType == Tile.TileType.Rottenland)
                    {
                        worldGenerator.CleanseTile(targetX, targetZ);  // Temizleme i�lemi
                    }
                }
            }
        }
    }

    // Cleanser binas� i�in gather i�lemi yok, o y�zden buray� bo� b�rak�yoruz
    protected override void PerformBuildingAction()
    {
        // Cleanser bir gather i�lemi yapmaz
    }
}
