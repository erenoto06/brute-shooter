using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Bu etiket, Player objesinde Rigidbody2D yoksa Unity'nin otomatik eklemesini sağlar!
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlari")]
    public float moveSpeed = 5f;
    
    [Header("Geliştirilebilir Güçler")]
    public float damage = 10f;          // Mermilerin temel hasarı
    public float attackSpeed = 1f;       // Ateş hızı çarpanı (1.5 olursa %50 daha hızlı sıkar)
    public int extraProjectiles = 0;     // +1 Mermi güçlendirmesi aldıkça bu artacak!
    public float pickupRange = 3f;       // Mıknatıs alanı


    [Header("Can Ayarlari")]
    public float maxHp = 100f;
    public float currentHp;
    [Header("XP ve Seviye Ayarlari")]
    public int level = 1;              // Karakterin şu anki seviyesi
    public float currentXP = 0f;       // Topladığımız mevcut XP
    public float xpToNextLevel = 5f;   // Bir sonraki seviye için gereken XP (başta 5 olsun)
    
    

    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Awake()
    {
        // Karakterin üstündeki Rigidbody2D'yi alıyoruz
        rb = GetComponent<Rigidbody2D>();
        
        // Eğer sahnede eklemeyi unuttuysan kod otomatik ekler ve ayarlarını yapar:
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        currentHp = maxHp;
    }

    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        // Unity 6 Yeni Klavye Sistemi (New Input System)
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
        }
#else
        // Eski Unity Input Sistemi
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
#endif

        // Çapraz giderken hızı dengelemek için normalize ediyoruz
        movementInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = movementInput * moveSpeed;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHp -= damageAmount;
        Debug.Log("Oyuncu Cani: " + currentHp);
        if(currentHp <= 0)
        {
            currentHp = 0f;
            Die();
            
        }
    }
    void Die()
    {
        Debug.Log("Karakter öldü");
        if(GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
        // Kristal karaktere değdiğinde bu fonksiyon çalışır
    public void GainXP(float amount)
    {
        currentXP += amount;
        Debug.Log($"XP Alindi: {currentXP} / {xpToNextLevel} (Seviye {level})");

        // Yeterli XP toplandı mı? (Level Up kontrolü)
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;                           // Seviyeyi 1 artır
        currentXP -= xpToNextLevel;       // Fazla kalan XP'yi bir sonraki seviyeye aktar
        xpToNextLevel = Mathf.Round(xpToNextLevel * 1.3f); // Bir sonraki seviye %30 daha zor olsun!

        Debug.Log($"🎉 TEBRİKLER! SEVİYE ATLADIN! Yeni Seviye: {level}");
        // (8. Adımda buraya 3 kart seçme menüsünü bağlayacağız!)
        if(UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OpenUpgradePanel();
        }
    }
}
