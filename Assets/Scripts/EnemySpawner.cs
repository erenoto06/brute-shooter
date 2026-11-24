using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
public class EnemySpawner : MonoBehaviour
{
    [Header("Düşman Kalibi")]
    public GameObject enemyPrefab; // Az önce Assets klasörüne attığımız Enemy prefab'ı

    [Header("Havuz Ayarlari (Object Pooling)")]
    public int poolSize = 50; // Oyun başında kaç adet düşman hazırda bekletilsin
    private List<GameObject> enemyPool = new List<GameObject>();
    
    
    
    [Header("Doğma Ayarlari")]
    public float spawnInterval = 1.5f; // Başlangıçta kaç saniyede bir düşman doğsun?
    public float minSpawnInterval = 0.3f; // Zorluk artsa bile düşmanlar bundan daha hızlı doğmasın (en yüksek zorluk sınırı)
    public float spawnRadius = 12f; // Oyuncunun kaç birim uzağında doğsunlar? (Kameranın hemen dışı)

    private Transform playerTransform;

     private PlayerMovement playerMovement;


    private float spawnTimer;      // Düşman doğma sayacı
    private float difficultyTimer; // Oyun süresi sayacı (zorluğu artırmak için)

    void Start()
    {
        // Oyuncuyu bul
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
        }

        spawnTimer = spawnInterval;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false); // Düşmanlar başta sahnede görünmesin
            enemyPool.Add(obj);   // Listeye ekle
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. Hayatta kalma süresini sayıyoruz
        difficultyTimer += Time.deltaTime;

        // ZORLUK ARTIŞI MANTIĞI:
        // Her 30 saniyede bir doğma süresini birazcık kısaltıyoruz (örn: 1.5s -> 1.4s -> 1.3s...)
        // Mathf.Max: Sürenin 'minSpawnInterval'ın altına inmesini engeller.
        float currentInterval = Mathf.Max(minSpawnInterval, spawnInterval - (difficultyTimer / 60f));

        // 2. Düşman doğma zamanı geldi mi?
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = currentInterval; // Yeni zorluk süresiyle sayacı sıfırla
        }
    }

        // Oyuncunun seviyesine göre sahnede aynı anda bulunabilecek maksimum düşman sayısı:
    int GetMaxAllowedEnemies()
    {
        int level = playerMovement != null ? playerMovement.level : 1;

        // 1. Faz (Seviye 1 - 9): 15 ile başlar, 40'a kadar artar
        if (level < 10)
        {
            return 15 + (level * 3); // Seviye 1: 18, Seviye 9: 42 düşman
        }
        // 2. Faz (Seviye 10 - 19): Sayı 20'ye düşer, 35'e kadar artar
        else if (level < 20)
        {
            return 20 + ((level - 10) * 2); // Seviye 10: 20, Seviye 19: 38 düşman
        }
        // 3. Faz (Seviye 20+): Dev tanklar geldiğinde sahnede en fazla 15 dev olsun!
        else
        {
            return 15;
        }
    }

        void SpawnEnemy()
    {
        // 1. ÖNCE SAHNEDEKİ CANLI DÜŞMAN SAYISINI SAYIYORUZ:
        int activeEnemyCount = 0;
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy.activeInHierarchy) activeEnemyCount++;
        }

        // Eğer sahnede şu an seviyemizin izin verdiğinden fazla düşman varsa, YENİSİNİ ÇIKARMA!
        if (activeEnemyCount >= GetMaxAllowedEnemies())
        {
            return;
        }

        // 2. DOĞMA NOKTASI HESAPLAMA:
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * spawnRadius;

        // 3. HAVUZDAN BİR DÜŞMAN BULUP SAHNEYE ÇIKARMA:
        int currentLevel = playerMovement != null ? playerMovement.level : 1;

        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.transform.position = spawnPosition;
                enemy.transform.rotation = Quaternion.identity;

                // İŞTE SİHİRLİ BAĞLANTI: Düşmana seviyeyi söyleyip evrimleştiriyoruz!
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.SetupEnemy(currentLevel);
                }

                enemy.SetActive(true);
                return;
            }
        }
    }
}