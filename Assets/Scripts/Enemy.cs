using Unity.Mathematics;
using UnityEngine;

// Düşmanın üzerinde Rigidbody2D yoksa Unity'nin otomatik eklemesini sağlıyoruz
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Düşman Özellikleri")]
    public float moveSpeed = 3f;  // Yürüme hızı
    public float maxHealth = 20f; // Canı
    public float currentHealth;
    public float damage = 10f;    // Oyuncuya vereceği hasar
    public float attackCooldown = 0.5f; // Oyuncuya yapışık kaldığında kaç saniyede bir vursun?

    [Header("Evrim ve Tip Ayarlari")]
    public float xpReward = 1f; // Bu düşman ölünce kaç XP düşürecek?
    private SpriteRenderer spriteRenderer;
    
    [Header("Düşecek Eşyalar")]
    public GameObject xpGemPrefab; // Düşman ölünce yere bırakacağı yeşil taş kalıbı
    
    
    
    private Rigidbody2D rb;
    private Transform playerTransform; // Oyuncunun konumu
    private float attackTimer;         // Vuruş bekleme sayacı

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Eğer sahnede ayarları yapılmadıysa kod otomatik yapsın:
        if (rb != null)
        {
            rb.gravityScale = 0f;       // Yerçekimi olmasın
            rb.freezeRotation = true;   // Çarpışınca dönmesin
        }
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        FindPlayer();
    }

        // Oyuncunun seviyesine göre düşmanı şekillendiren ana fonksiyon:
    public void SetupEnemy(int playerLevel)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // -------------------------------------------------------------
        // 1. FAZ: SEVİYE 1 - 9 (KIRMIZI MİNYONLAR)
        // -------------------------------------------------------------
        if (playerLevel < 10)
        {
            // Görsel: Kırmızı ve normal boyut
            spriteRenderer.color = Color.red;
            transform.localScale = new Vector3(1f, 1f, 1f);

            // Statlar: Her seviyede hafif canı artsın
            maxHealth = 20f + (playerLevel * 2f); // 22 HP -> 38 HP arası
            moveSpeed = 3f;
            damage = 10f;
            xpReward = 1f; // 1 XP verir
        }
        // -------------------------------------------------------------
        // 2. FAZ: SEVİYE 10 - 19 (MOR BÜYÜK DÜŞMANLAR)
        // -------------------------------------------------------------
        else if (playerLevel < 20)
        {
            // Görsel: Koyu Mor ve 1.4 kat daha büyük!
            spriteRenderer.color = new Color(0.6f, 0.1f, 0.9f); // Mor renk
            transform.localScale = new Vector3(1.4f, 1.4f, 1f);

            // Statlar: Canı sağlam, biraz daha ağır
            maxHealth = 60f + ((playerLevel - 10) * 5f); // 60 HP -> 105 HP arası
            moveSpeed = 2.6f;
            damage = 18f;
            xpReward = 3f; // 3 XP verir!
        }
        // -------------------------------------------------------------
        // 3. FAZ: SEVİYE 20 VE ÜZERİ (ALTIN-SİYAH DEV TANKLAR)
        // -------------------------------------------------------------
        else
        {
            // Görsel: Altın Sarısı ve 1.8 kat devasa boyut!
            spriteRenderer.color = new Color(1f, 0.8f, 0.1f); // Altın sarısı
            transform.localScale = new Vector3(1.8f, 1.8f, 1f);

            // Statlar: Tam bir boss! Çok can, yüksek hasar, yavaş hız
            maxHealth = 200f + ((playerLevel - 20) * 15f); // 200 HP+
            moveSpeed = 2.0f;
            damage = 30f;
            xpReward = 8f; // Tam 8 XP verir!
        }

        // Canını yeni hesaplanan maxHealth'e dolduruyoruz
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Vuruş sayacını geriye sayıyoruz
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Eğer oyuncu henüz bulunamadıysa tekrar bulmayı dene
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        // Oyuncuya doğru olan yön vektörü
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Düşmanı hareket ettir
        rb.linearVelocity = direction * moveSpeed;
    }

    // -------------------------------------------------------------
    // OYUNCUYA TEMAS ETTİĞİNDE HASAR VURMA KODU
    // -------------------------------------------------------------
    void OnCollisionStay2D(Collision2D collision)
    {
        // Süremiz dolduysa vurabiliriz
        if (attackTimer <= 0f)
        {
            // Dokunduğumuz obje oyuncu mu?
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage);
                attackTimer = attackCooldown; // Bir sonraki vuruş için 0.5 saniye bekle
            }
        }
    }

    // Oyuncuyu 3 farklı yolla arayan akıllı arama fonksiyonu:
    void FindPlayer()
    {
        // 1. Yol: "Player" etiketi (Tag) ile ara
        GameObject p = GameObject.FindWithTag("Player");

        // 2. Yol: Eğer etiketi unutulduysa, direkt objenin adına ("Player") bak
        if (p == null)
        {
            p = GameObject.Find("Player");
        }

        // 3. Yol: Sahnedeki PlayerMovement scriptine sahip objeyi bul
        if (p == null)
        {
            PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
            if (pm != null) p = pm.gameObject;
        }

        // Bulunduysa referansını al
        if (p != null)
        {
            playerTransform = p.transform;
        }
    }

    // Hasar alma fonksiyonu (Mermiler vurduğunda)
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

        void Die()
    {
        if (xpGemPrefab != null)
        {
            GameObject gem = Instantiate(xpGemPrefab, transform.position, Quaternion.identity);
            
            // Düşmanın tipine göre taşın vereceği XP'yi ayarlıyoruz!
            ExperienceGem gemScript = gem.GetComponent<ExperienceGem>();
            if (gemScript != null)
            {
                gemScript.xpAmount = xpReward;
            }
        }

        gameObject.SetActive(false);
    }
}