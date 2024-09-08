using UnityEngine;

public class Cleanser : Building
{
    public int cleanseRadius = 1;


    // Building yerleþtirildiðinde çaðrýlýr
    public override void OnPlaced(Tile tile)
    {
        base.OnPlaced(tile); // Temel yerleþtirme iþlemini yap
        CleanseNearbyTiles(tile); // Yakýndaki fayanslarý temizle
    }

    // Yakýndaki fayanslarý temizle
    private void CleanseNearbyTiles(Tile tile)
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        if (worldGenerator == null)
        {
            Debug.LogError("WorldGenerator not found.");
            return;
        }

        // Fayansýn pozisyonunu dünya grid koordinatlarýna dönüþtürelim
        Vector3 tilePosition = tile.transform.position;
        int tileX = Mathf.RoundToInt(tilePosition.x / (worldGenerator.tileSize + worldGenerator.spacing));
        int tileZ = Mathf.RoundToInt(tilePosition.z / (worldGenerator.tileSize + worldGenerator.spacing));

        for (int x = -cleanseRadius; x <= cleanseRadius; x++)
        {
            for (int z = -cleanseRadius; z <= cleanseRadius; z++)
            {
                int targetX = tileX + x;
                int targetZ = tileZ + z;

                // Fayansýn harita sýnýrlarý içinde olup olmadýðýný kontrol et
                if (worldGenerator.IsWithinBounds(targetX, targetZ))
                {
                    Tile targetTile = worldGenerator.tiles[targetX, targetZ];

                    // Eðer fayans Rottenland ise temizleyelim
                    if (targetTile.tileType == Tile.TileType.Rottenland)
                    {
                        worldGenerator.CleanseTile(targetX, targetZ);  // Temizleme iþlemi
                    }
                }
            }
        }
    }

    // Cleanser binasý için gather iþlemi yok, o yüzden burayý boþ býrakýyoruz
    protected override void PerformBuildingAction()
    {
        // Cleanser bir gather iþlemi yapmaz
    }
}
