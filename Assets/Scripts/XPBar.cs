using UnityEngine;
using UnityEngine.UI; // Image kullanmak için şart

public class XPBar : MonoBehaviour
{
    [Header("UI Görseli")]
    public Image fillImage; // Mavi XPBar_Fill görseli

    private PlayerMovement player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        if (player != null && fillImage != null)
        {
            // MATEMATİK MANTIĞI:
            // O anki XP / Gereken XP (Örn: 2 / 5 = 0.4 yani %40 dolu)
            fillImage.fillAmount = player.currentXP / player.xpToNextLevel;
        }
    }
}