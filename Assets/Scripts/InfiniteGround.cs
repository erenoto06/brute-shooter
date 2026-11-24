using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    [Header("Zemin Ayarlari")]
    public Transform player;      // Oyuncumuz
    public float tileSize = 10f;  // Her bir zemin parçasının genişliği/yüksekliği
    public int gridCount = 5;     // 5x5 = 25 adet zemin karosu

    private GameObject[,] tiles;  // Karoları hafızada tutacağımız 2 boyutlu dizi

    void Start()
    {
        // Oyuncu atanmadıysa sahnede bul
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        tiles = new GameObject[gridCount, gridCount];

        // Zemin için basit, koyu gri-yeşil 2 farklı ton oluşturalım (dama tahtası deseni)
        Sprite darkTile = CreateTileSprite(new Color(0.18f, 0.22f, 0.18f));
        Sprite lightTile = CreateTileSprite(new Color(0.22f, 0.26f, 0.22f));

        int halfGrid = gridCount / 2;

        // 5x5 ızgara şeklinde zeminleri oluşturuyoruz
        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                GameObject tile = new GameObject($"Tile_{x}_{y}");
                tile.transform.parent = this.transform;

                // Konumlandırma
                float posX = (x - halfGrid) * tileSize;
                float posY = (y - halfGrid) * tileSize;
                tile.transform.position = new Vector3(posX, posY, 0f);

                // Görsel bileşen (SpriteRenderer)
                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                // Dama tahtası deseni veriyoruz ki hareket ederken çizgiler belirgin olsun:
                sr.sprite = ((x + y) % 2 == 0) ? darkTile : lightTile;
                // ÇOK ÖNEMLİ: Zemin her zaman karakterin arkasında kalsın diye Sorting Order = -10
                sr.sortingOrder = -10;

                tiles[x, y] = tile;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float limit = (gridCount * tileSize) / 2f;

        // Her bir karoyu kontrol et: Oyuncudan çok uzaklaştıysa oyuncunun önüne ışınla!
        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                Transform tile = tiles[x, y].transform;

                // X ekseninde mesafe kontrolü
                float diffX = player.position.x - tile.position.x;
                if (Mathf.Abs(diffX) > limit)
                {
                    tile.position += new Vector3(Mathf.Sign(diffX) * gridCount * tileSize, 0f, 0f);
                }

                // Y ekseninde mesafe kontrolü
                float diffY = player.position.y - tile.position.y;
                if (Mathf.Abs(diffY) > limit)
                {
                    tile.position += new Vector3(0f, Mathf.Sign(diffY) * gridCount * tileSize, 0f);
                }
            }
        }
    }

    // Kod içinden otomatik renkli kare sprite üreten yardımcı fonksiyon
    private Sprite CreateTileSprite(Color color)
    {
        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f / tileSize);
    }
}