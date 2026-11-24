using UnityEngine;
using UnityEngine.UI; // UI Image bileşenini kullanmak için şarttır!

public class HealthBar : MonoBehaviour
{
    [Header("UI Görseli")]
    public Image fillImage; // Az önce yaptığımız kırmızı HealthBar_Fill görseli

    private PlayerMovement player;

    void Start()
    {
        // Sahnedeki oyuncuyu buluyoruz
        player = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        if (player != null && fillImage != null)
        {
            // MATEMATİK MANTIĞI:
            // Fill Amount 0.0 ile 1.0 arasında çalışır.
            // O anki Can / Maksimum Can (Örn: 50 / 100 = 0.5 yani yarı dolu)
            fillImage.fillAmount = player.currentHp / player.maxHp;
        }
    }
}