using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    [Header("XP Ayarlari")]
    public float xpAmount = 1f;       // Bu kristal kaç XP kazandıracak?
    public float pickupDistance = 3f; // Oyuncu kaç birim yaklaşırsa mıknatıs çekmeye başlasın?
    public float moveSpeed = 8f;      // Kristalin oyuncuya doğru uçuş hızı

    private Transform playerTransform;
    private bool isBeingPulled = false; // Mıknatıs çekmeye başladı mı?

    void Start()
    {
        // Sahnedeki oyuncuyu buluyoruz
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. Oyuncu ile aramızdaki mesafeyi ölçüyoruz
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 2. Eğer oyuncu mıknatıs alanına girdiyse çekimi başlat
        if (distance <= pickupDistance)
        {
            isBeingPulled = true;
        }

        // 3. Çekim başladıysa kristali oyuncuya doğru uçur
        if (isBeingPulled)
        {
            // MoveTowards: Bir noktadan diğerine sabit hızla kaydırır
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    // Karakterin içine girdiği an çalışır (Collider Trigger olduğu için)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Dokunduğumuz obje oyuncu mu?
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            // Oyuncuya XP ver (Bir sonraki adımda GainXP'yi yazacağız)
            player.GainXP(xpAmount);

            // Kristal toplandı, kendini yok et!
            Destroy(gameObject);
        }
    }
}