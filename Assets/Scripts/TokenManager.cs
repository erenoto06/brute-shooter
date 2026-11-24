using UnityEngine;

public class TokenManager : MonoBehaviour
{
    // Singleton mantığı: Her yerden "TokenManager.Instance" diye kolayca ulaşabilmek için
    public static TokenManager Instance;

    private const string TOKEN_KEY = "TotalTokens"; // Bilgisayara kaydettiğimiz anahtar kelime

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Toplam token sayımızı verir
    public int GetTotalTokens()
    {
        return PlayerPrefs.GetInt(TOKEN_KEY, 0);
    }

    // Yeni token ekler ve bilgisayara kaydeder
    public void AddTokens(int amount)
    {
        int current = GetTotalTokens();
        current += amount;
        PlayerPrefs.SetInt(TOKEN_KEY, current);
        PlayerPrefs.Save();

        Debug.Log($"+{amount} Token eklendi! Yeni Toplam: {current}");
    }

    // İleride Mağazada harcama yaparken kullanacağımız fonksiyon:
    public bool SpendTokens(int cost)
    {
        int current = GetTotalTokens();

        // Yeterli parası var mı?
        if (current >= cost)
        {
            current -= cost;
            PlayerPrefs.SetInt(TOKEN_KEY, current);
            PlayerPrefs.Save();
            Debug.Log($"{cost} Token harcandi! Kalan: {current}");
            return true; // Satın alma başarılı
        }
        else
        {
            Debug.Log("Yetersiz Token!");
            return false; // Satın alma başarısız
        }
    }
}