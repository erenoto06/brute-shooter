using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Mermi Ayarlari")]
    public float speed = 10f;     // Merminin uçuş hızı
    public float damage = 10f;    // Düşmana vereceği hasar
    public float lifetime = 3f;   // 3 saniye sonra sahneden silinsin (harita dışına uçup oyunu kasmasın)

    private Vector2 moveDirection; // Hangi yöne uçacağı

    void Start()
    {
        // Doğduktan 'lifetime' (3 saniye) sonra bu mermiyi otomatik yok et
        Destroy(gameObject, lifetime);
    }

    // Silah bu fonksiyonu çağırıp mermiye "Şu yöne doğru uç!" diyecek
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Update()
    {
        // Mermiyi belirlenen yöne doğru her saniye hızla ilerlet
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    // Mermi "Is Trigger" olarak işaretli bir objeye temas ettiğinde bu fonksiyon otomatik çalışır!
    void OnTriggerEnter2D(Collider2D other)
    {
        // Çarptığımız objede "Enemy" scripti var mı?
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            // Düşmana hasar ver!
            enemy.TakeDamage(damage);

            // Mermi işini yaptı, kendini yok et!
            Destroy(gameObject);
        }
    }
}