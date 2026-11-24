using UnityEngine;
using UnityEngine.SceneManagement; // Sahneyi yeniden başlatmak için bu kütüphane ŞARTTIR!
using TMPro;
public class GameManager : MonoBehaviour
{
    // Singleton mantığı: Diğer scriptlerden "GameManager.Instance" diyerek kolayca ulaşabilmek için
    public static GameManager Instance;

    [Header("UI Paneli")]
    public GameObject gameOverPanel; // Az önce yaptığımız GameOverPanel objesi

    public TextMeshProUGUI gameOverStatsText;
    void Awake()
    {
        Application.targetFrameRate = 60;
        // Singleton kurulumu
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Oyun her başladığında zamanın normal aktığından emin ol
        Time.timeScale = 1f;
    }

    // Karakter öldüğünde PlayerMovement bu fonksiyonu çağıracak!
    public void GameOver()
    {
        
        Time.timeScale = 0f;

        // 2. Gizlediğimiz Game Over panelini ekranda göster
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        int reachedLevel = 1;
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if(player != null)
        {
            reachedLevel = player.level;
        }
        int total = 0;

        if(TokenManager.Instance != null)
        {
            TokenManager.Instance.AddTokens(reachedLevel);
            total = TokenManager.Instance.GetTotalTokens();
        }

        if(gameOverStatsText != null)
        {
            gameOverStatsText.text = "Ulaşilan Seviye: " + reachedLevel + "\n" + "Kazanilan Token: +" + reachedLevel + "\n" + "Toplam Token: " + total;
        }
    }

    // "Yeniden Başla" butonuna tıklandığında çalışacak fonksiyon
    public void RestartGame()
    {
        // Zamanı tekrar normal akışına döndür
        Time.timeScale = 1f;

        // Şu an açık olan sahneyi baştan yükle (her şey sıfırlanır)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}