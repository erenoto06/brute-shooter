using UnityEngine;
using TMPro; // TextMeshPro yazılarını yönetmek için şarttır

public class GameHUD : MonoBehaviour
{
    [Header("UI Yazı Referansı")]
    public TextMeshProUGUI levelText; // Az önce yaptığımız LevelText objesi

    private PlayerMovement player;

    void Start()
    {
        // Oyuncuyu buluyoruz
        player = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        // Her karede oyuncunun seviyesini ekrana yazdırıyoruz
        if (player != null && levelText != null)
        {
            levelText.text = "LVL " + player.level;
        }
    }
}