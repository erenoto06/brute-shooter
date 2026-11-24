using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [Header("Mermi Şablonu")]
    public GameObject projectilePrefab;

    [Header("Silah Ayarları")]
    public float fireRate = 1f;       // Temel bekleme süresi
    public float attackRange = 8f;

    private float fireTimer;
    private PlayerMovement player;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FireAtNearestEnemy();

            float currentCooldown;
            if(player != null)
            {
                currentCooldown = fireRate / player.attackSpeed;
            }
            else
            {
                currentCooldown = fireRate;
            }
            fireTimer = currentCooldown;
        }
    }

    void FireAtNearestEnemy()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearestEnemy = null;
        float shortestDistance = attackRange;

        foreach (Enemy enemy in allEnemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && projectilePrefab != null)
        {
            Vector2 baseDirection = (nearestEnemy.transform.position - transform.position).normalized;

            // KAÇ MERMİ SIKACAĞIZ? (Temel 1 + Ekstra Mermiler)
            int totalProjectiles = 1;
            if(player != null)
            {
                totalProjectiles += player.extraProjectiles;
            }
            
            // Mermileri yelpaze gibi yayma açısı (örneğin her mermi arası 10 derece)
            float spreadAngle = 10f;
            float startAngle = -((totalProjectiles - 1) * spreadAngle) / 2f;

            for (int i = 0; i < totalProjectiles; i++)
            {
                float currentAngle = startAngle + (i * spreadAngle);
                
                // Ana yönü hafifçe döndürerek yelpaze açısı oluşturuyoruz:
                Vector2 shootDir = Quaternion.Euler(0, 0, currentAngle) * baseDirection;

                GameObject bulletObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Projectile bulletScript = bulletObj.GetComponent<Projectile>();
                if (bulletScript != null)
                {
                    bulletScript.SetDirection(shootDir);
                    // Merminin hasarını da karakterin damage değerine eşitliyoruz:
                    if (player != null) bulletScript.damage = player.damage;
                }
            }
        }
    }
}