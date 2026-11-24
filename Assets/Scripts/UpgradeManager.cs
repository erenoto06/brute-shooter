using UnityEngine;
using TMPro; // Butonların yazılarını değiştirmek için

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("UI Paneli")]
    public GameObject upgradePanel;

    private PlayerMovement player;

    void Awake()
    {
        // Singleton kurulumu
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }

    // Level atlandığında PlayerMovement bu fonksiyonu çağıracak!
    public void OpenUpgradePanel()
    {
        // 1. Oyunu dondur
        Time.timeScale = 0f;

        // 2. Paneli ekranda göster
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }
    }

    // 1. Butona tıklanınca çalışır (+1 Mermi)
    public void SelectExtraProjectile()
    {
        if (player != null)
        {
            player.extraProjectiles += 1;
            Debug.Log("Güçlendirme alindi: +1 Ekstra Mermi!");
        }
        ResumeGame();
    }

    // 2. Butona tıklanınca çalışır (+Hasar)
    public void SelectDamage()
    {
        if (player != null)
        {
            player.damage *= 1.25f; // Hasarı %25 artırır
            Debug.Log("Güçlendirme alindi: Hasar artti!");
        }
        ResumeGame();
    }

    // 3. Butona tıklanınca çalışır (+Hız)
    public void SelectMoveSpeed()
    {
        if (player != null)
        {
            player.moveSpeed += 1f; // Hızı 1 birim artırır
            Debug.Log("Güçlendirme alindi: Koşu Hizi arttı!");
        }
        ResumeGame();
    }

    // Güç seçildikten sonra oyunu devam ettir
    void ResumeGame()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false); // Paneli kapat
        }

        Time.timeScale = 1f; // Zamanı tekrar akıt!
    }
}